using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TennisMatchmaker.Controllers;
using TennisMatchmaker.Data;
using TennisMatchmaker.Dtos;
using TennisMatchmaker.Models;
using Xunit;

namespace TennisMatchmaker.Tests;

public class PlayerControllerTests
{
    // A fresh, isolated in-memory database per test - the random name means
    // no test can see another test's data, so tests can run in any order
    // (or in parallel) without interfering with each other
    private static TennisDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TennisDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new TennisDbContext(options);
    }

    [Fact]
    public async Task CreatePlayer_AddsPlayerWithGroupMemberships()
    {
        // Arrange
        using var db = CreateContext();
        db.Groups.Add(new Group
        {
            Id = 1,
            Name = "Tuesday Night",
            LeaderId = 1
        });

        await db.SaveChangesAsync();
        var controller = new PlayersController(db);
        var dto = new CreatePlayerDto(
            "Alice", 
            "Female",
            3.5,
            new List<int> { 1 }
        );

        // Act
        var result = await controller.CreatePlayer(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var playerDto = Assert.IsType<PlayerDto>(okResult.Value);
        Assert.Equal("Alice", playerDto.Name);

        var membership = await db.GroupMemberships.SingleAsync();
        Assert.Equal(1, membership.GroupId);
        Assert.Equal(playerDto.Id, membership.PlayerId);

    }

    [Fact]
    public async Task GetPlayers_ReturnsOnlyPlayersInSpecifiedGroup()
    {
        // Arrange
        using var db = CreateContext();
        db.Groups.AddRange(
            new Group { 
                Id = 1, 
                Name = "Group A", 
                LeaderId = 1
            },
            new Group { 
                Id = 2, 
                Name = "Group B", 
                LeaderId = 1
            }
        );

        db.Players.AddRange(
            new Player { 
                Id = 1, 
                Name = "Alice", 
                Gender = Gender.Female, 
                SkillLevel = 3.5 
            },
            new Player { 
                Id = 2, 
                Name = "Bob", 
                Gender = Gender.Male, 
                SkillLevel = 4.0 
            }
        );

        db.GroupMemberships.AddRange(
            new GroupMembership { PlayerId = 1, GroupId = 1 },
            new GroupMembership { PlayerId = 2, GroupId = 2 }
        );
        await db.SaveChangesAsync();
        var controller = new PlayersController(db);

        // Act 
        var result = await controller.GetPlayers(1);

        // Assert - Bob is in group 2; only Alice should come back for group 1
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var players = Assert.IsAssignableFrom<List<PlayerDto>>(okResult.Value);
        Assert.Single(players);
        Assert.Equal("Alice", players[0].Name);
    }

    [Fact]
    public async Task UpdatePlayer_UpdatesFields()
    {
        // Arrange
        using var db = CreateContext();
        db.Players.Add(new Player
        {
            Id = 1, 
            Name = "Alice", 
            Gender = Gender.Female,
            SkillLevel = 3.0 
        });
        await db.SaveChangesAsync();
        var controller = new PlayersController(db);
        var dto = new UpdatePlayerDto(
            "Alicia", 
            "Female",
            4.0
        );

        // Act 
        var result = await controller.UpdatePlayer(1, dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var updated = Assert.IsType<PlayerDto>(okResult.Value);
        Assert.Equal("Alicia", updated.Name);
        Assert.Equal(4.0, updated.SkillLevel);
    }

    [Fact]
    public async Task UpdatePlayer_ReturnsNotFound_WhenPlayerMissing()
    {
        // Arrange
        using var db = CreateContext();
        var controller = new PlayersController(db);
        var dto = new UpdatePlayerDto(
            "Ghost",
            "Male",
            3.0
        );

        // Act
        var result = await controller.UpdatePlayer(999, dto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeletePlayer_RemovesPlayer()
    {
        // Arrange
        using var db = CreateContext();
        db.Players.Add(new Player
        {
            Id = 1,
            Name = "Alice",
            Gender = Gender.Female,
            SkillLevel = 3.0
        });
        await db.SaveChangesAsync();
        var controller = new PlayersController(db);

        // Act
        var result = await controller.DeletePlayer(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Empty(db.Players);
    }


}

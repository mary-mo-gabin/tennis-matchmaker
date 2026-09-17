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
            new Group { Id = 1, Name = "Group A", LeaderId = 1},
            new Group { Id = 2, Name = "Group B", LeaderId = 1}
        );

        db.Players.AddRange(
            new Player { Id = 1, Name = "Alice", Gender = Gender.Female, SkillLevel = 3.5 },
            new Player { Id = 2, Name = "Bob", Gender = Gender.Male, SkillLevel = 4.0 }
        );


    }
}

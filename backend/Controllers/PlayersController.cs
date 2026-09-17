using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TennisMatchmaker.Data;
using TennisMatchmaker.Dtos;
using TennisMatchmaker.Models;

namespace TennisMatchmaker.Controllers
{
    [ApiController]
    [Route("api/players")]

    public class PlayersController : ControllerBase
    {
        private readonly TennisDbContext _db;
        public PlayersController(TennisDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<System.Collections.Generic.List<PlayerDto>>> GetPlayers([FromQuery] int groupId)
        {
            var players = await _db.Players
                .Where(p => p.GroupMemberships.Any(gm => gm.GroupId == groupId))
                .Select(p => new PlayerDto(
                    p.Id, 
                    p.Name, 
                    p.Gender.ToString(), 
                    p.SkillLevel
                ))
                .ToListAsync();
            return Ok(players);
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> CreatePlayer(CreatePlayerDto dto)
        {
            var player = new Player
            {
                Name = dto.Name,
                Gender = Enum.Parse<Gender>(dto.Gender),
                SkillLevel = dto.SkillLevel
            };                      // not saved to DB yet - player.Id is still 0

            foreach (var groupId in dto.GroupIds)
                player.GroupMemberships.Add(new GroupMembership { GroupId = groupId});        // each GroupMembership is "attached" to this Player object in memory, via the collection - but nothing has PlayerId set yet, and player.Id is still 0

            _db.Players.Add(player);            // tells EF Core to track this whole object graph
            await _db.SaveChangesAsync();       // EF Core figures out the actual INSERT order and IDs
            
            return Ok(new PlayerDto(
                player.Id, 
                player.Name, 
                player.Gender.ToString(), 
                player.SkillLevel
            ));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PlayerDto>> UpdatePlayer(int id, UpdatePlayerDto dto)
        {
            var player = await _db.Players.FindAsync(id);
            if (player == null) return NotFound();

            player.Name = dto.Name;
            player.Gender = Enum.Parse<Gender>(dto.Gender);
            player.SkillLevel = dto.SkillLevel;
            await _db.SaveChangesAsync();

            return Ok(new PlayerDto(
                player.Id, 
                player.Name, 
                player.Gender.ToString(), 
                player.SkillLevel
            ));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var player = await _db.Players.FindAsync(id);
            if (player == null) return NotFound();

            _db.Players.Remove(player);
            await _db.SaveChangesAsync();
            
            return NoContent();   
        }

        [HttpGet("{id}/groups")]
        public async Task<ActionResult<List<GroupDto>>> GetPlayerGroups (int id)
        {
            var groups = await _db.GroupMemberships
                .Where(gm => gm.PlayerId == id)
                .Select(gm => new GroupDto(
                    gm.Group!.Id, 
                    gm.Group.Name,
                    gm.Group.LeaderId
                ))
                .ToListAsync();
            
            return Ok(groups);
        }

        [HttpPost("{id}/groups/{groupId}")]
        public async Task<IActionResult> AddToGroup(int id, int groupId)
        {
            var playerExists = await _db.Players.AnyAsync(p => p.Id == id);
            if (!playerExists) return NotFound();

            var alreadyMember = await _db.GroupMemberships
                .AnyAsync(gm => gm.PlayerId == id && gm.GroupId == groupId);
            if (alreadyMember) return Conflict("Player is already in this group.");

            _db.GroupMemberships.Add(new GroupMembership { PlayerId = id, GroupId = groupId });
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}/groups/{groupId}")]
        public async Task<IActionResult> RemoveFromGroup(int id, int groupId)
        {
            // FindAsync takes composite key values in the order HasKey defined them: (PlayerId, GroupId)
            var membership = await _db.GroupMemberships.FindAsync(id, groupId);
            if (membership == null) return NotFound();

            _db.GroupMemberships.Remove(membership);
            await _db.SaveChangesAsync();
            return NoContent();
        }
        
    }
}
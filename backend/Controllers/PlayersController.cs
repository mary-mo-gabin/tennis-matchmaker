using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult<List<PlayerDto>>> GetPlayers([FromQuery] int groupId, [FromQuery] bool includeInactive = false)
        {
            var query = _db.Players.Where(p => p.GroupMemberships.Any(gm => gm.GroupId == groupId));
            if (!includeInactive) // only show active players
                query =query.Where(p => p.IsActive);

            var players = await query
                .Select(p => new PlayerDto(
                    p.Id, 
                    p.Name, 
                    p.Gender.ToString(), 
                    p.SkillLevel,
                    p.IsActive
                ))
                .AsNoTracking()
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
                player.GroupMemberships.Add(new GroupMembership { GroupId = groupId });        // each GroupMembership is "attached" to this Player object in memory, via the collection - but nothing has PlayerId set yet, and player.Id is still 0

            _db.Players.Add(player);            // tells EF Core to track this whole object graph
            await _db.SaveChangesAsync();       // EF Core figures out the actual INSERT order and IDs
            
            return Ok(new PlayerDto(
                player.Id, 
                player.Name, 
                player.Gender.ToString(), 
                player.SkillLevel,
                player.IsActive
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
                player.SkillLevel,
                player.IsActive
            ));
        }

        // Soft delete: existing Matches/pairingHistory/SessionPlayer rows referencing 
        // this player stay resolvable (the Player row - and their name - still exists), 
        // they just won't appear when building a new session.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivatePlayer(int id)
        {
            var player = await _db.Players.FindAsync(id);
            if (player == null) return NotFound();

            player.IsActive = false;
            await _db.SaveChangesAsync();
            return NoContent();   
        }

        [HttpPost("{id}/reactivate")]
        public async Task<IActionResult> ReactivatePlayer(int id)
        {
            var player = await _db.Players.FindAsync(id);
            if (player == null) return NotFound();

            player.IsActive = true;
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
                    gm.Group.LeaderId,
                    gm.Group.IsActive
                ))
                .AsNoTracking()
                .ToListAsync();
            return Ok(groups);
        }

        [HttpPost("{id}/groups/{groupId}")]
        public async Task<IActionResult> AddToGroup(int id, int groupId)
        {
            var playerExists = await _db.Players.AnyAsync(p => p.Id == id);
            if (!playerExists) return NotFound();

            var group = await _db.Groups.FindAsync(groupId);
            if (group == null) return NotFound();
            if (!group.IsActive) return Conflict("Cannot add a player to an inactive group.");

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
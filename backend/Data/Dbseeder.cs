using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TennisMatchmaker.Models;
 
namespace TennisMatchmaker.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(TennisDbContext db)
        {
            // Idempotent: if this test group already exists, assume seeding
            // already happened and do nothing — safe to run on every startup.
            var alreadySeeded = await db.Groups.AnyAsync(g => g.Name == "Test Group");
            if (alreadySeeded) return;
 
            var group = new Group { Name = "Test Group", LeaderId = 1 };
            db.Groups.Add(group);
 
            // 6 male, 6 female — enough for men's doubles, women's doubles, and
            // mixed doubles to all be satisfiable in the same round. Skill levels
            // spread 2.0–5.0 so MatchGenerationService actually has imbalance to
            // optimize against, rather than everyone being interchangeable.
            var players = new[]
            {
                new Player { Name = "Alice Chen",     Gender = Gender.Female, SkillLevel = 3.5 },
                new Player { Name = "Brian Kim",       Gender = Gender.Male,   SkillLevel = 4.0 },
                new Player { Name = "Carla Diaz",      Gender = Gender.Female, SkillLevel = 3.0 },
                new Player { Name = "David Wong",      Gender = Gender.Male,   SkillLevel = 3.5 },
                new Player { Name = "Elena Petrova",   Gender = Gender.Female, SkillLevel = 4.5 },
                new Player { Name = "Frank Muller",    Gender = Gender.Male,   SkillLevel = 2.5 },
                new Player { Name = "Grace Lee",       Gender = Gender.Female, SkillLevel = 2.0 },
                new Player { Name = "Henry Osei",      Gender = Gender.Male,   SkillLevel = 5.0 },
                new Player { Name = "Isla Fraser",     Gender = Gender.Female, SkillLevel = 3.5 },
                new Player { Name = "Jack Turner",     Gender = Gender.Male,   SkillLevel = 3.0 },
                new Player { Name = "Karen Ito",       Gender = Gender.Female, SkillLevel = 4.0 },
                new Player { Name = "Liam Novak",      Gender = Gender.Male,   SkillLevel = 4.5 },
            };
 
            foreach (var player in players)
            {
                // Setting the Group navigation property (not GroupId) works the
                // same way CreatePlayer does — group.Id doesn't exist yet since
                // it hasn't been saved, so EF Core resolves both foreign keys
                // together via relationship fixup when SaveChangesAsync runs.
                player.GroupMemberships.Add(new GroupMembership { Group = group });
                db.Players.Add(player);
            }
 
            await db.SaveChangesAsync();
        }
    }
}
 
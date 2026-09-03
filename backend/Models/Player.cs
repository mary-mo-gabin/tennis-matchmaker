namespace TennisMatchmaker.Models;

public enum Gender { Male, Female }

public class Player
{
    public int Id { get; set; }
    
    public required string Name { get; set; }

    public Gender Gender { get; set; }

    public double SkillLevel { get; set; } // NTRP-style 1.0–5.5

    public ICollection<GroupMembership> GroupMemberships { get; set; } = new List<GroupMembership>();
}
namespace TennisMatchmaker.Models;

public class Group
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int LeaderId { get; set; }

    public ICollection<GroupMembership> PlayerMemberships { get; set; } = new List<GroupMembership>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
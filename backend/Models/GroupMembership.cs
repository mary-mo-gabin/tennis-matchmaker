namespace TennisMatchmaker.Models;

public class GroupMembership
{
    public int PlayerId { get; set; }
    public Player? Player { get; set; }
    public int GroupId { get; set; }
    public Group? Group { get; set; }
}
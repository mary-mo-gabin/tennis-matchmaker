namespace TennisMatchmaker.Models;

public class GroupMembership
{   // Joint entity - many-to-many relationship between Player and Group
    public int PlayerId { get; set; }
    public Player? Player { get; set; }
    public int GroupId { get; set; }
    public Group? Group { get; set; }
}
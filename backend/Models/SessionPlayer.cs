namespace TennisMatchmaker.Models;

// Join table: which players were selected for a given session
public class SessionPlayer
{   // join table - many-to-many relationship between Session and Player
    public int SessionId { get; set; }
    public Session? Session { get; set; }
    public int PlayerId { get; set; }
    public Player? Player { get; set; } 
}

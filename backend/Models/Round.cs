namespace TennisMatchmaker.Models;

public class Round
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public Session? Session { get; set; }
    public int RoundNumber { get; set; }
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}

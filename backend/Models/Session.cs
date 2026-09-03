namespace TennisMatchmaker.Models;

public class Session
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public Group? Group { get; set; }
    public DateTime Date { get; set; }

    public String RuleConfigJson { get; set; } = "{}"; // e.g. { "numGames": 3, "playersPerCourt": 4 }

    public ICollection<Round> Rounds { get; set; } = new List<Round>();
    public ICollection<SessionPlayer> SelectedPlayers { get; set; } = new List<SessionPlayer>();
}
namespace TennisMatchmaker.Models;

public enum MatchType{ MensDoubles, WomensDoubles, MixedDoubles }
 
public class Match
{
    public int Id { get; set; }
    public int RoundId { get; set; }
    public Round? Round { get; set; }
    public int CourtNumber { get; set; }
    public MatchType MatchType { get; set; }
    public int TeamAPlayer1Id { get; set; }
    public int TeamAPlayer2Id { get; set; }
    public int TeamBPlayer1Id { get; set; }
    public int TeamBPlayer2Id { get; set; }
}

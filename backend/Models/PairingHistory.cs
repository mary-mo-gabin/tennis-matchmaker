namespace TennisMatchmaker.Models;

// Denormalized pairing history for fast lookups during match generation.
// Updated after each session completes.
public class PairingHistory
{
    public int Id { get; set; }
    public int PlayerAId { get; set; }
    public int PlayerBId { get; set; }
    public int TimesPartnered { get; set; }
    public int TimesOpposed { get; set; }
    public DateTime LastPlayedDate { get; set; }
}

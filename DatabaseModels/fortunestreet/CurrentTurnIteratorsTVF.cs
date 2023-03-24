namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

public class CurrentTurnIteratorsTVF
{
    public long ID { get; set; }
    public long AnalyzerInstanceID { get; set; }
    public long CharacterID { get; set; }
    public int TurnResetCounter { get; set; }
    public byte TurnNumber { get; set; }
    public byte TurnOrder { get; set; }
    public DateTime TimestampAdded { get; set; }
}

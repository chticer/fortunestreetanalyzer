namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

public class GetPostRollsTVF
{
    public long CharacterID { get; set; }
    public long SpaceIDLandedOn { get; set; }
    public byte TurnNumber { get; set; }
    public byte DieRollValue { get; set; }
    public string Logs { get; set; }
}

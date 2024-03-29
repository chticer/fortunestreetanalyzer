﻿namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

public class GetPreRollsTVF
{
    public long TurnIteratorID { get; set; }
    public long CharacterID { get; set; }
    public byte TurnNumber { get; set; }
    public long SpaceIDCurrent { get; set; }
    public long? SpaceIDFrom { get; set; }
    public byte LayoutIndex { get; set; }
    public byte Level { get; set; }
    public byte Placing { get; set; }
    public int ReadyCash { get; set; }
    public int TotalShopValue { get; set; }
    public int TotalStockValue { get; set; }
    public int NetWorth { get; set; }
    public string OwnedShopIndices { get; set; }
    public byte TotalSuitCards { get; set; }
    public string CollectedSuits { get; set; }
    public byte ArcadeIndex { get; set; }
    public string DieRollRestrictions { get; set; }
}

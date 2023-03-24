namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of pre rolls within the program.
/// </summary>
public partial class PreRolls
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Turn iterator identifier reference.
    /// </summary>
    public long TurnIteratorID { get; set; }

    /// <summary>
    /// Space identifier reference of the current space.
    /// </summary>
    public long SpaceIDCurrent { get; set; }

    /// <summary>
    /// Space identifier reference of the space where the player came from.
    /// </summary>
    public long? SpaceIDFrom { get; set; }

    /// <summary>
    /// The index value of the board layout.
    /// </summary>
    public byte LayoutIndex { get; set; }

    /// <summary>
    /// The level of the player.
    /// </summary>
    public byte Level { get; set; }

    /// <summary>
    /// The placing of the player.
    /// </summary>
    public byte Placing { get; set; }

    /// <summary>
    /// The ready cash value of the player.
    /// </summary>
    public int ReadyCash { get; set; }

    /// <summary>
    /// The total value of all shops owned by the player.
    /// </summary>
    public int TotalShopValue { get; set; }

    /// <summary>
    /// The total value of all stocks owned by the player.
    /// </summary>
    public int TotalStockValue { get; set; }

    /// <summary>
    /// The net worth of the player.
    /// </summary>
    public int NetWorth { get; set; }

    /// <summary>
    /// List of indices of shops the player owns.
    /// </summary>
    public string OwnedShopIndices { get; set; }

    /// <summary>
    /// The total number of suit cards the player has.
    /// </summary>
    public byte TotalSuitCards { get; set; }

    /// <summary>
    /// List of suits the player has collected.
    /// </summary>
    public string CollectedSuits { get; set; }

    /// <summary>
    /// The index value of the arcade mini-game.
    /// </summary>
    public byte ArcadeIndex { get; set; }

    /// <summary>
    /// List of die roll values that the player is restricted to.
    /// </summary>
    public string DieRollRestrictions { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual Spaces _SpaceIDCurrent { get; set; }

    public virtual Spaces _SpaceIDFrom { get; set; }

    public virtual TurnIterators TurnIterator { get; set; }
}

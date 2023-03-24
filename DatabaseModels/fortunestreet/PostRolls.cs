namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of post rolls within the program.
/// </summary>
public partial class PostRolls
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
    /// Space identifier reference of the space landed on.
    /// </summary>
    public long SpaceIDLandedOn { get; set; }

    /// <summary>
    /// The value of the die roll.
    /// </summary>
    public byte DieRollValue { get; set; }

    /// <summary>
    /// JSON array of turn outcomes and events.
    /// </summary>
    public string Logs { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual Spaces _SpaceIDLandedOn { get; set; }

    public virtual TurnIterators TurnIterator { get; set; }
}

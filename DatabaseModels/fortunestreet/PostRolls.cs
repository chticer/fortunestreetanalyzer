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
    /// Analyzer instance identifier reference.
    /// </summary>
    public long AnalyzerInstanceID { get; set; }

    /// <summary>
    /// Character identifier reference.
    /// </summary>
    public long CharacterID { get; set; }

    /// <summary>
    /// Space identifier reference of the space landed on.
    /// </summary>
    public long SpaceIDLandedOn { get; set; }

    /// <summary>
    /// Whether the turn has been resetted.
    /// </summary>
    public bool TurnResetFlag { get; set; }

    /// <summary>
    /// The turn number value of the player.
    /// </summary>
    public byte TurnNumber { get; set; }

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

    public virtual AnalyzerInstances AnalyzerInstance { get; set; }

    public virtual Spaces SpaceIdLandedOnNavigation { get; set; }
}

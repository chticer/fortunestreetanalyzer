namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of turn iterators within the program.
/// </summary>
public partial class TurnIterators
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
    /// The turn reset counter value of the iteration.
    /// </summary>
    public int TurnResetCounter { get; set; }

    /// <summary>
    /// The turn number value of the player.
    /// </summary>
    public byte TurnNumber { get; set; }

    /// <summary>
    /// The turn order value of the player.
    /// </summary>
    public byte TurnOrder { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual AnalyzerInstances AnalyzerInstance { get; set; }

    public virtual ICollection<PostRolls> PostRolls { get; } = new List<PostRolls>();

    public virtual ICollection<PreRolls> PreRolls { get; } = new List<PreRolls>();
}

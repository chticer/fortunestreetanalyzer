namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of determining the turn order of characters within the program.
/// </summary>
public partial class TurnOrderDetermination
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
    /// The value of the number determined relative to the analyzer instance and character.
    /// </summary>
    public byte Value { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual AnalyzerInstances AnalyzerInstance { get; set; }
}

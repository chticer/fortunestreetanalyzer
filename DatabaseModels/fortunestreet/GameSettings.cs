namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of game settings within the program.
/// </summary>
public partial class GameSettings
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
    /// Rule identifier reference.
    /// </summary>
    public long RuleID { get; set; }

    /// <summary>
    /// Board identifier reference.
    /// </summary>
    public long BoardID { get; set; }

    /// <summary>
    /// Color identifier reference for the Mii character.
    /// </summary>
    public long MiiColorID { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual AnalyzerInstances AnalyzerInstance { get; set; }

    public virtual Boards Board { get; set; }

    public virtual Colors MiiColor { get; set; }

    public virtual Rules Rule { get; set; }
}

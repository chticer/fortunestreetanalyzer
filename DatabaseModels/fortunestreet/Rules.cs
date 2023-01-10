namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of the rules in Fortune Street.
/// </summary>
public partial class Rules
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// The name of the rule.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual ICollection<BoardCharacteristics> BoardCharacteristics { get; } = new List<BoardCharacteristics>();

    public virtual ICollection<GameSettings> GameSettings { get; } = new List<GameSettings>();

    public virtual ICollection<Spaces> Spaces { get; } = new List<Spaces>();
}

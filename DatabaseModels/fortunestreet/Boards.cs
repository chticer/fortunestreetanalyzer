namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of the boards in Fortune Street.
/// </summary>
public partial class Boards
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// The name of the board.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual ICollection<BoardCharacterCrosslist> BoardCharacterCrosslists { get; } = new List<BoardCharacterCrosslist>();

    public virtual ICollection<BoardCharacteristics> BoardCharacteristics { get; } = new List<BoardCharacteristics>();

    public virtual ICollection<GameSettings> GameSettings { get; } = new List<GameSettings>();

    public virtual ICollection<Spaces> Spaces { get; } = new List<Spaces>();
}

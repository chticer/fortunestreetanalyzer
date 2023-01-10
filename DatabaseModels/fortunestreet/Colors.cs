namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of the colors in Fortune Street.
/// </summary>
public partial class Colors
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Hex value given by the Nintendo Wii system.
    /// </summary>
    public string SystemColor { get; set; }

    /// <summary>
    /// Hex value given to a character in Fortune Street.
    /// </summary>
    public string CharacterColor { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual ICollection<CharacterColorCrosslist> CharacterColorCrosslists { get; } = new List<CharacterColorCrosslist>();

    public virtual ICollection<GameSettings> GameSettings { get; } = new List<GameSettings>();
}

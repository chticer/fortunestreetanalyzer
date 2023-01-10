namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of the characters in Fortune Street.
/// </summary>
public partial class Characters
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// The URI value of the character's portrait image.
    /// </summary>
    public string CharacterPortraitURI { get; set; }

    /// <summary>
    /// The name of the character.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The rank of the character.
    /// </summary>
    public string Rank { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }
}

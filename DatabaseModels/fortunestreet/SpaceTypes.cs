namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of the type of spaces in Fortune Street.
/// </summary>
public partial class SpaceTypes
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// The unique name of the type of space.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The Font Awesome icon value of the type of space.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// The title of the type of space.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The description of the type of space.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual ICollection<Spaces> Spaces { get; } = new List<Spaces>();
}

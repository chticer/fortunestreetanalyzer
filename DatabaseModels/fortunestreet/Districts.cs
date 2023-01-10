namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of the districts in Fortune Street.
/// </summary>
public partial class Districts
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// The name of the district.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Hex value given to a district in Fortune Street.
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual ICollection<Spaces> Spaces { get; } = new List<Spaces>();
}

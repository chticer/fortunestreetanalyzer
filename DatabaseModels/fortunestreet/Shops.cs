namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of the shops in Fortune Street.
/// </summary>
public partial class Shops
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// The name of the shop.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The value of the shop.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual ICollection<Spaces> Spaces { get; } = new List<Spaces>();
}

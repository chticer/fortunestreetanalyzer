namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of analyzer instances within the program.
/// </summary>
public partial class AnalyzerInstances
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
    /// Hash value of the user&apos;s IP address.
    /// </summary>
    public string IPAddress { get; set; }

    /// <summary>
    /// The type of the analyzer instance.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The name of the analyzer instance.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Indication of the analyzer instance&apos;s state.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual ICollection<GameSettings> GameSettings { get; } = new List<GameSettings>();

    public virtual ICollection<PostRolls> PostRolls { get; } = new List<PostRolls>();

    public virtual ICollection<PreRolls> PreRolls { get; } = new List<PreRolls>();

    public virtual ICollection<ShopOffers> ShopOffers { get; } = new List<ShopOffers>();

    public virtual ICollection<TurnOrderDetermination> TurnOrderDeterminations { get; } = new List<TurnOrderDetermination>();
}

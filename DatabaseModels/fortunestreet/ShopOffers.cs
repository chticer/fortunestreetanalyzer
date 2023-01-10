namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of shop offers within the program.
/// </summary>
public partial class ShopOffers
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
    /// The type of the shop offer.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Character identifier reference of the player initiating the shop offer.
    /// </summary>
    public long CharacterIDFrom { get; set; }

    /// <summary>
    /// List of shop indices that the player is giving to another player.
    /// </summary>
    public string ShopIndicesFrom { get; set; }

    /// <summary>
    /// Character identifier reference of the player receiving the shop offer.
    /// </summary>
    public long? CharacterIDTo { get; set; }

    /// <summary>
    /// List of shop indices that the player is receiving from another player.
    /// </summary>
    public string ShopIndicesTo { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual AnalyzerInstances AnalyzerInstance { get; set; }

    public virtual ICollection<AuctionBids> AuctionBids { get; } = new List<AuctionBids>();

    public virtual ICollection<ShopOfferNegotiations> ShopOfferNegotiations { get; } = new List<ShopOfferNegotiations>();
}

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of shop offer negotations within the program.
/// </summary>
public partial class ShopOfferNegotiations
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Shop offer identifier reference.
    /// </summary>
    public long ShopOfferID { get; set; }

    /// <summary>
    /// Character identifier reference.
    /// </summary>
    public long CharacterID { get; set; }

    /// <summary>
    /// Whether the shop offer negotiation succeeded.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Whether the shop offer negotiation was forced.
    /// </summary>
    public bool Forced { get; set; }

    /// <summary>
    /// The amount of the shop offer negotation.
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual ShopOffers ShopOffer { get; set; }
}

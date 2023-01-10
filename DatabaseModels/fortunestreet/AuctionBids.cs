namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of auction bids within the program.
/// </summary>
public partial class AuctionBids
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Shop offer negotiation identifier reference.
    /// </summary>
    public long ShopOfferNegotiationID { get; set; }

    /// <summary>
    /// Character identifier reference.
    /// </summary>
    public long CharacterID { get; set; }

    /// <summary>
    /// The bid of the auction.
    /// </summary>
    public int Bid { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual ShopOffers ShopOfferNegotiation { get; set; }
}

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of the constraint of spaces in Fortune Street.
/// </summary>
public partial class SpaceConstraints
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Space identifier reference.
    /// </summary>
    public long SpaceID { get; set; }

    /// <summary>
    /// Space identifier reference from the source.
    /// </summary>
    public long SpaceIDFrom { get; set; }

    /// <summary>
    /// Space identifier reference to the destination.
    /// </summary>
    public long SpaceIDTo { get; set; }

    /// <summary>
    /// The index value of the board layout.
    /// </summary>
    public byte LayoutIndex { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual Spaces Space { get; set; }

    public virtual Spaces _SpaceIDFrom { get; set; }

    public virtual Spaces _SpaceIDTo { get; set; }
}

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of the layout of spaces in Fortune Street.
/// </summary>
public partial class SpaceLayouts
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
    /// The index value of the board layout.
    /// </summary>
    public byte LayoutIndex { get; set; }

    /// <summary>
    /// The scale value of the horizontal center position of the space.
    /// </summary>
    public decimal CenterXFactor { get; set; }

    /// <summary>
    /// The scale value of the vertical center position of the space.
    /// </summary>
    public decimal CenterYFactor { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual Spaces Space { get; set; }
}

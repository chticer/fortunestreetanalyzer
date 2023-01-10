namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of the spaces in Fortune Street.
/// </summary>
public partial class Spaces
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Rule identifier reference.
    /// </summary>
    public long RuleID { get; set; }

    /// <summary>
    /// Board identifier reference.
    /// </summary>
    public long BoardID { get; set; }

    /// <summary>
    /// Space type identifier reference.
    /// </summary>
    public long SpaceTypeID { get; set; }

    /// <summary>
    /// Shop identifier reference.
    /// </summary>
    public long? ShopID { get; set; }

    /// <summary>
    /// District identifier reference.
    /// </summary>
    public long? DistrictID { get; set; }

    /// <summary>
    /// JSON value of any information pertaining to the space.
    /// </summary>
    public string AdditionalProperties { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual Boards Board { get; set; }

    public virtual Districts District { get; set; }

    public virtual ICollection<PostRolls> PostRolls { get; } = new List<PostRolls>();

    public virtual ICollection<PreRolls> PreRollsSpaceIDCurrent { get; } = new List<PreRolls>();

    public virtual ICollection<PreRolls> PreRollsSpaceIDFrom { get; } = new List<PreRolls>();

    public virtual Rules Rule { get; set; }

    public virtual Shops Shop { get; set; }

    public virtual SpaceTypes SpaceType { get; set; }

    public virtual ICollection<SpaceConstraints> SpaceConstraintsSpaceIDFrom { get; } = new List<SpaceConstraints>();

    public virtual ICollection<SpaceConstraints> SpaceConstraintsSpaceIDTo { get; } = new List<SpaceConstraints>();

    public virtual ICollection<SpaceConstraints> SpaceConstraints { get; } = new List<SpaceConstraints>();

    public virtual ICollection<SpaceLayouts> SpaceLayouts { get; } = new List<SpaceLayouts>();
}

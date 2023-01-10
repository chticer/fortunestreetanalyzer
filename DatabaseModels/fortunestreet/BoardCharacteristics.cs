namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Data of the characteristics of boards in Fortune Street.
/// </summary>
public partial class BoardCharacteristics
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
    /// Minimum standing value required to complete the board relative to the rule and board that the user selected.
    /// </summary>
    public byte StandingThreshold { get; set; }

    /// <summary>
    /// Minimum net worth value required to complete the board relative to the rule and board that the user selected.
    /// </summary>
    public short NetWorthThreshold { get; set; }

    /// <summary>
    /// Starting ready cash value relative to the rule and board that the user selected.
    /// </summary>
    public short ReadyCashStart { get; set; }

    /// <summary>
    /// Starting salary value relative to the rule and board that the user selected.
    /// </summary>
    public short SalaryStart { get; set; }

    /// <summary>
    /// The value that the salary increases by for every level up relative to the rule and board that the user selected.
    /// </summary>
    public byte SalaryIncrease { get; set; }

    /// <summary>
    /// Maximum value that a character can roll the die relative to the rule and board that the user selected.
    /// </summary>
    public byte MaxDieRoll { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual Boards Board { get; set; }

    public virtual Rules Rule { get; set; }
}

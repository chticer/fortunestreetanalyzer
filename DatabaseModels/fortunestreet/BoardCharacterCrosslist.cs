﻿namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Crosslist for boards and characters tables.
/// </summary>
public partial class BoardCharacterCrosslist
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Board identifier reference.
    /// </summary>
    public long BoardID { get; set; }

    /// <summary>
    /// Character identifier reference.
    /// </summary>
    public long CharacterID { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual Boards Board { get; set; }
}

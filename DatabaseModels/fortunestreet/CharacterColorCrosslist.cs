﻿namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

/// <summary>
/// Crosslist for characters and colors tables.
/// </summary>
public partial class CharacterColorCrosslist
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Character identifier reference.
    /// </summary>
    public long CharacterID { get; set; }

    /// <summary>
    /// Color identifier reference.
    /// </summary>
    public long ColorID { get; set; }

    /// <summary>
    /// The order value relative to the character and color.
    /// </summary>
    public byte Position { get; set; }

    /// <summary>
    /// The order value, in comparison with other characters, relative to the character and color.
    /// </summary>
    public byte? Priority { get; set; }

    /// <summary>
    /// Default UTC timestamp when record is added.
    /// </summary>
    public DateTime TimestampAdded { get; set; }

    public virtual Colors Color { get; set; }
}

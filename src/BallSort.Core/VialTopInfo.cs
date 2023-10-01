namespace BallSort.Core;

public record struct VialTopInfo
{
    /// <summary>
    /// How many slots are empty at the top of the vial
    /// </summary>
    public int EmptyCount;
    
    /// <summary>
    /// The color of the top of the vial
    /// </summary>
    public byte Color;
    
    /// <summary>
    /// How many balls of the top color are at the top of the vial
    /// </summary>
    public int Count;
}
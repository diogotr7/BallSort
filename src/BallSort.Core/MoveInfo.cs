namespace BallSort.Core;

public struct MoveInfo
{
    /// <summary>
    /// The source vial of a move.
    /// </summary>
    public byte Source;
    
    /// <summary>
    /// The destination vial of a move.
    /// </summary>
    public byte Destination;
    
    /// <summary>
    ///  no clue
    /// </summary>
    public bool Merged;
}
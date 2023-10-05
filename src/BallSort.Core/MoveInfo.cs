namespace BallSort.Core;

public record struct MoveInfo
{
    /// <summary>
    /// The source vial of a move.
    /// </summary>
    public byte Source { get; set; }
    
    /// <summary>
    /// The destination vial of a move.
    /// </summary>
    public byte Destination{ get; set; }
    
    /// <summary>
    ///  no clue
    /// </summary>
    public bool Merged { get; set; }
}
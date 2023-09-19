namespace PascalPort;

/// <summary>
/// Represents the options for a game
/// </summary>
public class GameSettings
{
    /// <summary>
    /// How many full vials the game starts with
    /// </summary>
    public int FilledVialCount { get; }
    
    /// <summary>
    /// How many empty vials the game starts with
    /// </summary>
    public int EmptyVialCount { get; }
    
    /// <summary>
    /// How many total vials the game starts with
    /// </summary>
    public int TotalVialCount => FilledVialCount + EmptyVialCount;
    
    /// <summary>
    /// How many balls each vial can hold
    /// </summary>
    public int VialDepth { get; }
    
    public GameSettings(int filledVialCount, int emptyVialCount, int vialDepth)
    {
        FilledVialCount = filledVialCount;
        EmptyVialCount = emptyVialCount;
        VialDepth = vialDepth;
    }
}
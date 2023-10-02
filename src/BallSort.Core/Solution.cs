namespace BallSort.Core;

public class Solution
{
    public bool SolutionFound { get; }
    public int Nodes { get; }
    public Move[] Moves { get; }

    public Solution(bool solutionFound, int nodes, Move[] moves)
    {
        SolutionFound = solutionFound;
        Nodes = nodes;
        Moves = moves;
    }
}
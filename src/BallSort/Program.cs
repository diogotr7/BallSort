using System.Diagnostics;

namespace BallSort;

internal class Program
{
    static void Main(string[] args)
    {
        var state = PuzzleGenerator.Generate(5, 4, 3, 9);
        var state2 = PuzzleGenerator.Generate(5, 4, 3, 9);
        var ts = Stopwatch.GetTimestamp();
        var moves = state.SolveGpt();
        ts = Stopwatch.GetTimestamp() - ts;

        foreach (var move in moves)
        {
            state2.PerformMove(move);
        }
        
        if (!state2.IsSolved())
            throw new Exception("Not solved");

        Console.WriteLine($"Solved in {moves.Count()} moves. {ts / (double)Stopwatch.Frequency} seconds.");
    }
}
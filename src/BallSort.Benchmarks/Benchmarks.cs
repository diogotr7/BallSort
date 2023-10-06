using BallSort.Core;
using BenchmarkDotNet.Attributes;

namespace BallSort.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class Benchmarks
{
    private readonly Puzzle[] _puzzles = {
        Puzzle.CreateRandom(new GameSettings(8, 2, 4), 0),
        Puzzle.CreateRandom(new GameSettings(10, 2, 4), 0),
        Puzzle.CreateRandom(new GameSettings(14, 2, 4), 0),
    };
    
    [Params(0, 1, 2)]
    public int PuzzleIndex { get; set; }
    
    [Benchmark]
    public void Solve()
    {
        var game = new Solver(_puzzles[PuzzleIndex]);
        game.solve_single();
    }
}
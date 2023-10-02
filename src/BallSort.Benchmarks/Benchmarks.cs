using BallSort.Core;
using BenchmarkDotNet.Attributes;

namespace BallSort.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class Benchmarks
{
    private readonly Puzzle _puzzle = Puzzle.CreateRandom(new GameSettings(8, 2, 4), 0);
    
    [Params(5, 10, 50, 100, 500, 1000)]
    public int N { get; set; }
    
    [Benchmark]
    public void Solve()
    {
        var game = new Solver(_puzzle, N);
        game.solve_single();
    }
}
using BallSort.Core;
using BenchmarkDotNet.Attributes;

namespace BallSort.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
public class Benchmarks
{
    [ParamsSource(nameof(Settings))]
    public GameSettings GameSettings { get; set; } 

    [Params(0, 1)] public int Seed { get; set; } = 0;

    [Benchmark]
    public void Solve()
    {
        var settings = GameSettings;
        var seed = Seed;
        var puzzle = VialsDef.CreateRandom(settings, seed);
        var game = new Solver(puzzle);
        game.solve_single();
    }
    
    public static IEnumerable<GameSettings> Settings()
    {
        yield return new GameSettings(2, 1, 4);
        yield return new GameSettings(6, 2, 4);
    }
}
using BenchmarkDotNet.Attributes;
using PascalPort;

namespace BallSort.ConsoleApp;

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
        var game = new Solver(settings);
        game.solve_single(puzzle);
    }
    
    public static IEnumerable<GameSettings> Settings()
    {
        yield return new GameSettings(4, 2, 4);
        yield return new GameSettings(6, 2, 4);
        yield return new GameSettings(6, 2, 6);
        yield return new GameSettings(6, 2, 8);
    }
}
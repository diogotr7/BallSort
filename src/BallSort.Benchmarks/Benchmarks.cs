﻿using BallSort.Core;
using BenchmarkDotNet.Attributes;

namespace BallSort.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
public class Benchmarks
{
    [ParamsSource(nameof(Seeds))] public int Seed { get; set; } = 0;
    
    [ParamsSource(nameof(VialCounts))] public int VialCount { get; set; } = 0;

    [Benchmark]
    public void Solve()
    {
        var settings = new GameSettings(VialCount, 2, 4);
        var seed = Seed;
        var puzzle = VialsDef.CreateRandom(settings, seed);
        var game = new Solver(puzzle);
        game.solve_single();
    }
    
    public static IEnumerable<int> Seeds()
    {
        yield return 0;
    }
    
    public static IEnumerable<int> VialCounts()
    {
        yield return 4;
        yield return 6;
        yield return 8;
    }
}
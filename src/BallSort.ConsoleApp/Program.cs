using System.Diagnostics;
using BallSort.OpenCv;
using BenchmarkDotNet.Running;
using PascalPort;

namespace BallSort.ConsoleApp;

public static class Program
{
    public static void Main(string[] args)
    {
        TestOpenCv();
        //BenchmarkRunner.Run<Benchmarks>();
        //TestSolve();
    }

    private static void TestSolve()
    {
        var settings = new GameSettings(4, 2, 4);
        //var newDef = VialsDef.Parse(File.ReadAllText("game.txt"));
        var puzzle = VialsDef.CreateRandom(settings);

        var game = new Solver(settings);

        var sw = Stopwatch.StartNew();
        game.solve_single(puzzle);
        sw.Stop();
        Console.WriteLine($"Solved puzzle in {sw.ElapsedMilliseconds} ms");
    }

    private static void TestOpenCv()
    {
        var sw2 = Stopwatch.StartNew();
        var recognized = PuzzleRecognizer.RecognizePuzzle("screenshot.png");
        sw2.Stop();
        Console.WriteLine($"Recognized puzzle in {sw2.ElapsedMilliseconds} ms");
    }
}
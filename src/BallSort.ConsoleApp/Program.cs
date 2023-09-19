using System.Diagnostics;
using BallSort.Core;
using BallSort.OpenCv;
using BenchmarkDotNet.Running;

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
        var recognize = Stopwatch.StartNew();
        var recognized = PuzzleRecognizer.RecognizePuzzle("screenshot.png");
        recognize.Stop();
        Console.WriteLine($"Recognized puzzle in {recognize.ElapsedMilliseconds} ms");
        
        //print recognized for debugging purposes
        for (var i = 0; i < recognized.Length; i++)
        {
            for (var j = 0; j < recognized[i].Length; j++)
            {
                //0123456789ABCDEF
                var n = (int)recognized[i][j];

                var s = n switch
                {
                    0 => "_",
                    < 10 => n.ToString(),
                    _ => ((char)(n - 10 + 'A')).ToString(),
                };
                Console.Write(s);
            }
            Console.WriteLine();
        }
        
        var solve = Stopwatch.StartNew();
        var solver = new Solver(recognized.GetSettings());
        solver.solve_single(recognized);
        solve.Stop();
        Console.WriteLine($"Solved puzzle in {solve.ElapsedMilliseconds} ms");
    }
}
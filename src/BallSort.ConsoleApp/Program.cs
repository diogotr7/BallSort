using System.Diagnostics;
using BallSort.Core;
using BallSort.OpenCv;

namespace BallSort.ConsoleApp;

public static class Program
{
    public static void Main(string[] args)
    {
        //TestOpenCv();
        TestSolve();
    }

    private static void TestSolve()
    {
        var settings = new GameSettings(8, 2, 4);
        //var newDef = VialsDef.Parse(File.ReadAllText("game.txt"));
        var puzzle = VialsDef.CreateRandom(settings);

        var game = new Solver(puzzle);

        var sw = Stopwatch.StartNew();
        var result = game.solve_single();
        sw.Stop();
        Console.WriteLine($"Solved puzzle in {sw.ElapsedMilliseconds} ms with {result.Moves.Length} moves");
    }

    private static void TestOpenCv()
    {
        var recognize = Stopwatch.StartNew();
        var recognized = PuzzleRecognizer.RecognizePuzzle("Screenshot_20230920_133424_Sorter_It_Puzzle.png");
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
        var solver = new Solver(recognized);
        var solution = solver.solve_single();
        solve.Stop();
        
        Console.WriteLine($"Solution: {solution.SolutionFound}");
        Console.WriteLine($"Nodes: {solution.Nodes}");
        foreach (var move in solution.Moves)
        {
            Console.WriteLine($"{move.From} -> {move.To}");
        }
        
        Console.WriteLine($"Solved puzzle in {solve.ElapsedMilliseconds} ms with {solution.Moves.Length} moves");
    }
}
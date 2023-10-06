using System.Diagnostics;
using System.Text.Json;
using BallSort.Core;
using BallSort.OpenCv;

namespace BallSort.ConsoleApp;

public static class Program
{
    public static void Main(string[] args)
    {
        TestSolve();
    }
    
    private static void TestSolve()
    {
        var puzzles = new[] {
            Puzzle.CreateRandom(new GameSettings(8, 2, 4), 0),
            Puzzle.CreateRandom(new GameSettings(10, 2, 4), 0),
            Puzzle.CreateRandom(new GameSettings(14, 2, 4), 0),
        };
        foreach (var puzzle in puzzles)
        {
            puzzle.Dump(Console.Out);
            
            var solver = new Solver(puzzle);
            
            var timer = Stopwatch.StartNew();
            var solution = solver.solve_single();
            timer.Stop();
            PrintResult(solution);
            Console.WriteLine($"Took {timer.ElapsedMilliseconds}ms ");
            
        }
    }
    
    private static void PrepareTestData()
    {
        foreach (var fileName in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "test-data"), "*.png"))
        {
            //TestOpenCv(file);
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var filenameText = filenameWithoutExtension + ".txt";
            var recognized = PuzzleRecognizer.RecognizePuzzle(fileName);
            var writer = File.CreateText(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "test-data", filenameText));
            recognized.Dump(writer);
            writer.Close();

            var filenameJson = filenameWithoutExtension + ".json";
            var sol = new Solver(recognized);
            var solution = sol.solve_single();
            var json = JsonSerializer.Serialize(solution);
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "test-data", filenameJson), json);
        }
    }

    private static void TestOpenCv()
    {
        foreach (var fileName in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "test-data"), "*.png"))
        {
            var recognize = Stopwatch.StartNew();
            var recognized = PuzzleRecognizer.RecognizePuzzle(fileName);
            recognize.Stop();
            Console.WriteLine($"Recognized puzzle in {recognize.ElapsedMilliseconds} ms");

            recognized.Dump(Console.Out);
            var solve = Stopwatch.StartNew();
            var solver = new Solver(recognized);
            var solution = solver.solve_single();
            solve.Stop();

            PrintResult(solution);
            Console.WriteLine($"Took {solve.ElapsedMilliseconds}ms ");
        }
    }

    private static void PrintResult(Solution solution)
    {
        Console.WriteLine($"Solution: {solution.SolutionFound}");
        Console.WriteLine($"Nodes: {solution.Nodes}");

        foreach (var move in solution.Moves)
        {
            Console.WriteLine($"{move.From + 1} -> {move.To + 1}");
        }

        Console.WriteLine($"Solved puzzle  with {solution.Moves.Length} moves");
    }
}
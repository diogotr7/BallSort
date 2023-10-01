using System.Diagnostics;
using System.Text.Json;
using BallSort.Core;
using BallSort.OpenCv;

namespace BallSort.ConsoleApp;

public static class Program
{
    public static void Main(string[] args)
    {
        var screenshots = Path.Combine(Environment.CurrentDirectory, "test-data");
        foreach (var file in Directory.EnumerateFiles(screenshots, "*.png"))
        {
            TestOpenCv(file);
            //PrepareTestData(file);
        }
        //TestSolve();
    }

    private static void TestSolve()
    {
        var settings = new GameSettings(8, 2, 4);
        //var newDef = VialsDef.Parse(File.ReadAllText("game.txt"));
        var puzzle = Puzzle.CreateRandom(settings);
        puzzle.Dump(Console.Out);

        var game = new Solver(puzzle);

        var solve = Stopwatch.StartNew();
        var solution = game.solve_single();
        solve.Stop();
        
        PrintResult(solution);
        Console.WriteLine($"Took {solve.ElapsedMilliseconds}ms ");
    }

    private static void TestOpenCv(string fileName)
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

    private static void PrepareTestData(string fileName)
    {
        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var filenameText = filenameWithoutExtension + ".txt";
        var recognized = PuzzleRecognizer.RecognizePuzzle(fileName);
        var writer = File.CreateText(Path.Combine(Environment.CurrentDirectory,"..", "..","..", "test-data", filenameText));
        recognized.Dump(writer);
        writer.Close();
        
        var filenameJson = filenameWithoutExtension + ".json";
        var sol = new Solver(recognized);
        var solution = sol.solve_single();
        var json = JsonSerializer.Serialize(solution);
        File.WriteAllText(Path.Combine(Environment.CurrentDirectory,"..", "..","..", "test-data", filenameJson), json);
        
    }

    private static void PrintResult(Solution solution)
    {
        Console.WriteLine($"Solution: {solution.SolutionFound}");
        Console.WriteLine($"Nodes: {solution.Nodes}");
        foreach (var move in solution.Moves)
        {
            Console.WriteLine($"{move.From} -> {move.To}");
        }
        
        Console.WriteLine($"Solved puzzle  with {solution.Moves.Length} moves");
    }
}
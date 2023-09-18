using System;
using System.Diagnostics;

namespace PascalPort;

internal class Program
{
    static void Main(string[] args)
    {
        var settings = new GameSettings(12, 2, 5);

        // var newDef = VialsDef.Parse(File.ReadAllText("game.txt"));
        var puzzle = VialsDef.CreateRandom(settings, 69);
        
        var game = new Global(settings);

        var sw = Stopwatch.StartNew();
        game.solve_single(puzzle);
        sw.Stop();
        Console.WriteLine($"Solved in {sw.ElapsedMilliseconds} ms");
    }
}
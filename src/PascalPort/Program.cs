using System;

namespace PascalPort;

internal class Program
{
    static void Main(string[] args)
    {
        var settings = new GameSettings(2, 1, 4);

        
        var newDef = VialsDef.Parse(File.ReadAllText("game.txt"));
        var puzzle = VialsDef.CreateRandom(settings);
        
        var game = new Global(settings);
        game.solve_single(newDef);
    }
}
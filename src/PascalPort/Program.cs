using System;

namespace PascalPort;

internal class Program
{
    static void Main(string[] args)
    {
        var vialsDef = Global.vialDefinition;
        var newDef = VialsDef.Parse(File.ReadAllText("game.txt"));
        if (vialsDef.Length != newDef.Length || vialsDef[0].Length != newDef[0].Length)
        {
            throw new Exception("VialsDef.Parse: vialsDef.Length <> newDef.Length");
        }
        Global.vialDefinition = newDef;
        Global.solve_single(Global.vialDefinition);
    }
}
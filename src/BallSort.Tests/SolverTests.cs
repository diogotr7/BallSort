using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using BallSort.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BallSort.Tests;

[TestClass]
public class SolverTests
{
    [TestMethod]
    public void SolverTest()
    {
        var puzzles = Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "TestData"), "*.txt");
        foreach (var puzzle in puzzles)
        {
            var p = Puzzle.Parse(File.ReadAllText(puzzle));
            var solver = new Solver(p);
            var solution = solver.solve_single();
            var expectedJson = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "TestData", Path.GetFileNameWithoutExtension(puzzle) + ".json"));
            var expected = JsonSerializer.Deserialize<Solution>(expectedJson);
            Assert.AreEqual(expected.SolutionFound, solution.SolutionFound);
            Assert.AreEqual(expected.Moves.Length, solution.Moves.Length);
            Assert.IsTrue(expected.Moves.SequenceEqual(solution.Moves));
        }
    }
}
using System;
using System.IO;
using BallSort.Core;
using BallSort.OpenCv;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BallSort.Tests;

[TestClass]
public class PuzzleRecognizerTests
{
    [TestMethod]
    public void PuzzleRecognizerTest()
    {
        var images = Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "TestData"), "*.png");
        foreach (var image in images)
        {
            var imageRecognized = PuzzleRecognizer.RecognizePuzzle(image);
            var parsed = Puzzle.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "TestData", Path.GetFileNameWithoutExtension(image) + ".txt")));
            
            Assert.AreEqual(parsed.VialCount, imageRecognized.VialCount);
            for (var i = 0; i < parsed.VialCount; i++)
            {
                Assert.AreEqual(parsed[i].Length, imageRecognized[i].Length);
                for (var j = 0; j < parsed[i].Length; j++)
                {
                    Assert.AreEqual(parsed[i][j], imageRecognized[i][j]);
                }
            }
        }
    }
}
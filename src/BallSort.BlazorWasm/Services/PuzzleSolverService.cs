using BallSort.Core;
using BallSort.OpenCv;
using OpenCvSharp;

namespace BallSort.BlazorWasm.Services;

public class PuzzleSolverService
{
    public Solution DetectAndSolve(Mat screenshot)
    {
        var puzzle = PuzzleRecognizer.RecognizePuzzle(screenshot);
        var solver = new Solver(puzzle);
        return solver.solve_single();
    }
}
using System.Diagnostics;
using System.Text;
using BallSort.Core;
using BallSort.OpenCv;

namespace BallSort.Api;

public static class Solve
{
    public static void UsePuzzleSolver(this WebApplication webApp)
    {
        webApp.MapPost("/solve", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(HttpRequest req, ILogger<int> logger)
    {
        var files = req.Form.Files;
        var sc = files.FirstOrDefault(x => x.FileName.EndsWith("png"));
        if (sc == null)
        {
            return Results.BadRequest();
        }

        //write file to temp
        var tempFile = Path.GetTempPath() + Guid.NewGuid() + ".png";
        await using var fs = File.OpenWrite(tempFile);
        await sc.CopyToAsync(fs);
        fs.Close();
    
        //recognize puzzle

        var sw = Stopwatch.StartNew();
        var puzzle = PuzzleRecognizer.RecognizePuzzle(tempFile);
        var recognizerTime = sw.ElapsedMilliseconds;
        var solver = new Solver(puzzle);
        var solution = solver.solve_single();
        var solveTime = sw.ElapsedMilliseconds - recognizerTime;
        sw.Stop();
        logger.LogInformation($"Recognized puzzle in {recognizerTime}ms, solved in {solveTime}ms");
        
        return GetHtmlForSolution(solution);
    }

    public static IResult GetHtmlForSolution(Solution solution)
    {
        var html = new StringBuilder();
    
        html.AppendLine("<html><body>");
        html.AppendLine("<h1>Solution</h1>");
        html.AppendLine($"<p>Number of moves: {solution.Moves.Length}</p>");
        html.AppendLine("<table>");
        html.AppendLine("<tr><th>From</th><th>To</th></tr>");
        foreach (var (from, to) in solution.Moves)
        {
            html.AppendLine($"<tr><td>{from + 1}</td><td>{to + 1}</td></tr>");
        }
    
        html.AppendLine("</table>");
        html.AppendLine("</body></html>");
    
        return Results.Content(html.ToString(), "text/html");
    }
}
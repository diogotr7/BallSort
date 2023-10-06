using System.Collections.ObjectModel;
using BallSort.Core;
using OpenCvSharp;

namespace BallSort.OpenCv;

/// <summary>
/// Reads puzzles from screenshots.
/// </summary>
public static class PuzzleRecognizer
{
    public static Puzzle RecognizePuzzle(string fileName)
    {
        using var src = new Mat(fileName);
        var topPortion = src.Height / 4;
        //crop off top and bottom 1/4 of image. This removes the top and bottom bars leaving only the vials.
        using var cropped = src[new Rect(0, topPortion, src.Width, src.Height - topPortion * 2)];
        
        //greyscale
        using var gray = new Mat();
        Cv2.CvtColor(cropped, gray, ColorConversionCodes.BGR2GRAY);
        

        //edge detection
        using var canny = new Mat();
        Cv2.Canny(gray, canny, 80, 160);        
        
        //detect vial rectangles
        Cv2.FindContours(
            canny,
            out var contours,
            out _,
            RetrievalModes.External,
            ContourApproximationModes.ApproxNone
        );
        
        var rectangles = contours.Select(Cv2.BoundingRect).ToList();
        
        //circle detection
        var circles = Cv2.HoughCircles(
            gray,
            HoughModes.Gradient,
            4,
            50,
            param1:100,
            param2: 100,
            minRadius: 30,
            maxRadius: 40
        );
        
        // Cv2.ImWrite("gray.png", gray);
        // Cv2.ImWrite("canny.png", canny);
        //
        // using var debug = cropped;
        // DrawCircles(circles, debug);
        // DrawRectangles(rectangles, debug);
        //
        // Cv2.ImWrite("debug.png", debug);
        
        return Process(cropped, rectangles, circles);
    }

    /// <summary>
    ///     Used after the image has been preprocessed and analyzed.
    ///     This will turn rects, points and radii into vials and balls.
    /// </summary>
    private static Puzzle Process(Mat cropped, IEnumerable<Rect> rectangles, IEnumerable<CircleSegment> circles)
    {
        var balls = ProcessBalls(cropped, circles);
        var vials = ProcessVials(rectangles.OrderBy(r => r.Top).ThenBy(r => r.Left), balls);
        var depth = vials.MaxBy(v => v.Count)!.Count;

        if (balls.Count % depth != 0)
            throw new Exception("unexpected number of balls, probably opencv failed to detect all balls");

        var d = new Puzzle(vials.Count, depth);
        for (var i = 0; i < vials.Count; i++)
        {
            var ballsInRect = vials[i];
            
            for (var j = 0; j < ballsInRect.Count; j++)
            {
                d[i, j] =  ballsInRect[j].ColorIndex;
            }
        }

        return d;
    }

    private static IReadOnlyList<IReadOnlyList<BallInfo>> ProcessVials(IEnumerable<Rect> rectangles, IReadOnlyList<BallInfo> balls)
    {
        var vials = new List<ReadOnlyCollection<BallInfo>>();
        foreach (var rectangle in rectangles)
        {
            var ballsInRect = balls.Where(b => rectangle.Contains(b.Circle.Center.ToPoint()))
                .OrderBy(b => b.Circle.Center.Y)
                .ToList();
            
            //if ballsInRect is empty, this is an empty vial
            vials.Add(ballsInRect.AsReadOnly());
        }

        return vials.AsReadOnly();
    }

    private static IReadOnlyList<BallInfo> ProcessBalls(Mat cropped, IEnumerable<CircleSegment> circles)
    {
        var knownColors = new List<Color>();
        var balls2 = new List<BallInfo>();
        
        foreach (var circle in circles)
        {
            var color = GetMeanColorInCircle(cropped, circle);
            if (!knownColors.Contains(color))
                knownColors.Add(color);

            balls2.Add(new(circle, (byte)(knownColors.IndexOf(color) + 1)));
        }

        return balls2.AsReadOnly();

        static Color GetMeanColorInCircle(Mat mat, CircleSegment cs)
        {
            using var circleMat = new Mat(mat.Size(), MatType.CV_8UC1, Scalar.Black);
            Cv2.Circle(circleMat, (int)cs.Center.X, (int)cs.Center.Y, (int)cs.Radius / 2, Scalar.White, 2);
            return Color.FromScalar(Cv2.Mean(mat, circleMat));
        }
    }

    private static void DrawRectangles(IEnumerable<Rect> rectangles, Mat cropped)
    {
        foreach (var rectangle in rectangles)
        {
            Cv2.Rectangle(cropped, rectangle, Scalar.Red, 2);
        }
    }

    private static void DrawCircles(IEnumerable<CircleSegment> circles, Mat cropped)
    {
        foreach (var circle in circles)
        {
            Cv2.Circle(cropped, (int)circle.Center.X, (int)circle.Center.Y, (int)circle.Radius, Scalar.LimeGreen, 4);
        }
    }
}

internal class BallInfo
{
    public BallInfo(CircleSegment circle, byte indexOf)
    {
        Circle = circle;
        ColorIndex = indexOf;
    }

    public CircleSegment Circle { get; set; }
    public byte ColorIndex { get; set; }
}
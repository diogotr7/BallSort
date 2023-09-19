using BallSort.Core;
using OpenCvSharp;
using Point = OpenCvSharp.Point;

namespace BallSort.OpenCv;

/// <summary>
/// Reads puzzles from screenshots.
/// </summary>
public static class PuzzleRecognizer
{
    public static VialsDef RecognizePuzzle(string fileName)
    {
        using var src = new Mat(fileName);
        using var cropped = src[new Rect(0, 800, src.Width, src.Height - 800 - 700)];
        using var grey = new Mat();
        
        //greyscale
        Cv2.CvtColor(cropped, grey, ColorConversionCodes.BGR2GRAY);
        
        //edge detection
        Cv2.Canny(grey, grey, 80, 160);        
        
        //detect vial rectangles
        Cv2.FindContours(
            grey,
            out var contours,
            out var hierarchy,
            RetrievalModes.External,
            ContourApproximationModes.ApproxNone
        );

        //convert contours to actual rectangles, ignore small ones
        var rectangles = contours.Where(contour => Cv2.ContourArea(contour) > 1000)
            .Select(Cv2.BoundingRect)
            .ToList();
        
        //circle detection
        var circles = Cv2.HoughCircles(
            grey,
            HoughModes.Gradient,
            3,
            68,
            param1:100,
            param2: 100,
            minRadius: 32,
            maxRadius: 39
        );
        
        #if DEBUG
        DrawCircles(circles, cropped);
        DrawRectangles(rectangles, cropped);
        
        //save to new file
        Cv2.ImWrite("circles.png", cropped);
        #endif
        
        return Process(cropped, rectangles, circles);
    }
    
    /// <summary>
    ///     Used after the image has been preprocessed and analyzed.
    ///     This will turn rects, points and radii into vials and balls.
    /// </summary>
    private static VialsDef Process(Mat cropped, IEnumerable<Rect> rectangles, IEnumerable<CircleSegment> circles)
    {
        var knownColors = new List<Color>();
        var balls = circles.Select(circle =>
        {
            var color = GetMeanColorInCircle(cropped, circle);
            if (!knownColors.Contains(color))
                knownColors.Add(color);
            
            return new BallInformation
            {
                Circle = circle,
                Color = color,
                ColorIndex = knownColors.IndexOf(color) + 1
            };
        }).ToArray();

        var sortedRectangles = rectangles.OrderBy(r => r.Top).ThenBy(r => r.Left).ToArray();
        var depth = balls.Count(b => sortedRectangles[0].Contains(b.Circle.Center.ToPoint()));

        var d = new VialsDef(sortedRectangles.Length, depth);
        for (var i = 0; i < sortedRectangles.Length; i++)
        {
            var rect = sortedRectangles[i];
            var ballsInRect = balls.Where(b => rect.Contains(b.Circle.Center.ToPoint()))
                .OrderByDescending(b => b.Circle.Center.Y)
                .ToArray();
            
            //if ballsInRect is empty, this is an empty vial
            for (var j = 0; j < ballsInRect.Length; j++)
            {
                d[i, j] =  (Ball)ballsInRect[j].ColorIndex;
            }
        }

        return d;
    }

    private static Color GetMeanColorInCircle(Mat mat, CircleSegment cs)
    {
        using var circleMat = new Mat(mat.Size(), MatType.CV_8UC1, Scalar.Black);
        Cv2.Circle(circleMat, (int)cs.Center.X, (int)cs.Center.Y, (int)cs.Radius / 2, Scalar.White, 2);
        return Color.FromScalar(Cv2.Mean(mat, circleMat));
    }

    private static void DrawRectangles(IEnumerable<Rect> rectangles, Mat cropped)
    {
        foreach (var rectangle in rectangles)
        {
            Cv2.Rectangle(cropped, rectangle, Scalar.LimeGreen, 2);
        }
    }

    private static void DrawCircles(IEnumerable<CircleSegment> circles, Mat cropped)
    {
        foreach (var circle in circles)
        {
            Cv2.Circle(cropped, (int)circle.Center.X, (int)circle.Center.Y, (int)circle.Radius, Scalar.LimeGreen, 2);
        }
    }
}
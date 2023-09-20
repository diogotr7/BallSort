using System.Collections.ObjectModel;
using BallSort.Core;
using OpenCvSharp;

namespace BallSort.OpenCv;

/// <summary>
/// Reads puzzles from screenshots.
/// </summary>
public static class PuzzleRecognizer
{
    public static VialsDef RecognizePuzzle(string fileName)
    {
        using var src = new Mat(fileName);
        var topPortion = src.Height / 4;
        //crop off top and bottom 1/4 of image. This removes the top and bottom bars leaving only the vials.
        using var cropped = src[new Rect(0, topPortion, src.Width, src.Height - topPortion * 2)];
        using var grey = new Mat();
        
        //greyscale
        Cv2.CvtColor(cropped, grey, ColorConversionCodes.BGR2GRAY);
        
        //edge detection
        Cv2.Canny(grey, grey, 80, 160);        
        
        //detect vial rectangles
        Cv2.FindContours(
            grey,
            out var contours,
            out _,
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
        var balls = ProcessBalls(cropped, circles);
        var vials = ProcessVials(rectangles.OrderBy(r => r.Top).ThenBy(r => r.Left), balls);
        var depth = vials.MaxBy(v => v.Count)!.Count;

        var d = new VialsDef(vials.Count, depth);
        for (var i = 0; i < vials.Count; i++)
        {
            var ballsInRect = vials[i];
            
            for (var j = 0; j < ballsInRect.Count; j++)
            {
                d[i, j] =  (Ball)ballsInRect[j].ColorIndex;
            }
        }

        return d;
    }

    private static IReadOnlyList<IReadOnlyList<(CircleSegment Circle, int ColorIndex)>> ProcessVials(IEnumerable<Rect> rectangles, IReadOnlyList<(CircleSegment Circle, int ColorIndex)> balls)
    {
        var vials = new List<ReadOnlyCollection<(CircleSegment Circle, int ColorIndex)>>();
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

    private static IReadOnlyList<(CircleSegment Circle, int ColorIndex)> ProcessBalls(Mat cropped, IEnumerable<CircleSegment> circles)
    {
        var knownColors = new List<Color>();
        var balls2 = new List<(CircleSegment Circle, int ColorIndex)>();
        
        foreach (var circle in circles)
        {
            var color = GetMeanColorInCircle(cropped, circle);
            if (!knownColors.Contains(color))
                knownColors.Add(color);

            balls2.Add((circle, knownColors.IndexOf(color) + 1));
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
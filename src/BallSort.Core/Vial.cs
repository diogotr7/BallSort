namespace BallSort.Core;

public class Vial
{
    public readonly Ball[] Balls;
    public readonly byte Position;

    public Vial(IEnumerable<Ball> b, byte position)
    {
        Balls = b.ToArray();
        Position = position;
    }

    public VialTopInfo GetTopInfo()
    {
        if (Balls[^1] == Ball.Empty)
        {
            return new VialTopInfo
            {
                Color = 0,
                EmptyCount = Balls.Length,
                Count = 0
            };
        }

        //starting from top (0):
        //empty = how many slots before the first ball
        //color = color of the first ball
        //count = how many balls of the same color are there
        var empty = 0;
        var previousBall = Balls[0];
        var count = 0;

        foreach (var ball in Balls)
        {
            //if the ball is empty, increment empty and continue
            if (ball == Ball.Empty)
            {
                empty++;
                continue;
            }

            //if the previous color was empty, but the new color is not, set the previous color to the new color and increment count
            if (previousBall == Ball.Empty)
            {
                previousBall = ball;
                count++;
                continue;
            }

            //if the new color is the same as the previous color, increment count
            if (ball == previousBall)
            {
                count++;
                continue;
            }

            break;
        }

        return new VialTopInfo
        {
            Color = previousBall,
            EmptyCount = empty,
            Count = count
        };
    }

    /// <summary>
    ///     Returns the number of blocks in the vial.
    /// </summary>
    /// <returns></returns>
    public int VialBlocks()
    {
        var res = 1;
        for (var i = 0; i < Balls.Length - 1; i++)
        {
            if (Balls[i + 1] != Balls[i])
            {
                res++;
            }
        }

        if (Balls[0] == Ball.Empty)
        {
            res--;
        }

        return res;
    }
}
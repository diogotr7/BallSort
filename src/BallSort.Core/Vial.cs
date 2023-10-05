using System.Text;

namespace BallSort.Core;

public sealed class Vial : IComparable<Vial>
{
    public readonly byte[] Balls;
    public readonly byte Position;

    public Vial(IEnumerable<byte> b, byte position)
    {
        Balls = b.ToArray();
        Position = position;
    }

    public VialTopInfo GetTopInfo()
    {
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
            if (ball == 0)
            {
                empty++;
                continue;
            }

            //if the previous color was empty, but the new color is not, set the previous color to the new color and increment count
            if (previousBall == 0)
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

        return new VialTopInfo(previousBall, empty, count);
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

        if (Balls[0] == 0)
        {
            res--;
        }

        return res;
    }
    
    public bool IsEmpty() => Balls[^1] == 0;

    public int CompareTo(Vial? other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));
        
        for (var i = 0; i < Balls.Length; i++)
        {
            if (Balls[i] < other.Balls[i])
                return -1;
            if (Balls[i] > other.Balls[i])
                return 1;
        }

        return 0;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        
        foreach (var ball in Balls)
        {
            sb.Append(Utilities.RenderBall(ball));
        }

        return sb.ToString();
    }
}
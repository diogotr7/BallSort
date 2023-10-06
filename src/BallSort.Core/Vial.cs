using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace BallSort.Core;

public struct Vial : IComparable<Vial>
{
    public readonly byte Position;
    public readonly byte VialDepth;
    
    private VialInternal _balls;
    public Span<byte> Balls => _balls.AsSpan(VialDepth);
    
    public Vial(Span<byte> b, byte position)
    {
        if (b.Length > 4)
            throw new ArgumentException("Vial can only hold 4 balls.");
        
        VialDepth = (byte) b.Length;
        Position = position;
        b.CopyTo(Balls);
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

    public bool IsFull() => Balls[0] != 0;

    public int CompareTo(Vial other)
    {
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

public record struct VialInternal
{
    public byte Child0;
    public byte Child1;
    public byte Child2;
    public byte Child3;
    
    public Span<byte> AsSpan(int length) => MemoryMarshal.CreateSpan(ref Unsafe.AsRef(Child0), length);
}
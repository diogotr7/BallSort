using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace BallSort;

//Display the vial from bottom to top
public sealed record Vial
{
    //bottom...top
    public readonly int[] Balls;
    private int _topIndex = -1;

    public Vial(int capacity)
    {
        Balls = new int[capacity];
        Array.Fill(Balls, -1);
    }
    
    public int Capacity => Balls.Length;
    
    public bool AllSameColor()
    {
        if (Balls.Length == 0)
            return true;
        
        var color = Balls[0];
        for (var index = 0; index < Balls.Length; index++)
        {
            if (Balls[index] != color)
                return false;
        }

        return true;
    }
    
    public bool IsFull() => _topIndex == Balls.Length - 1;
    
    public bool IsEmpty() => _topIndex == -1;

    public void Push(int ball)
    {
        _topIndex++;
        Balls[_topIndex] = ball;
    }
    
    public int Pop()
    {
        var ball = Balls[_topIndex];
        Balls[_topIndex] = -1;
        _topIndex--;
        return ball;
    }
    
    public int Peek() => Balls[_topIndex];

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            
            for (var i = 0; i < Balls.Length; i++)
            {
                hash = hash * 23 + Balls[i];
            }

            return hash;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Balls.Length; i++)
        {
            if (Balls[i] == -1)
                sb.Append(' ');
            else
                sb.Append(Balls[i]);
            sb.Append(' ');
        }

        return sb.ToString();
    }
}
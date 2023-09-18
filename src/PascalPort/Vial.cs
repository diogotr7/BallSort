namespace PascalPort;

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
        var res = new VialTopInfo
        {
            TopCol = 0,
            Empty = Balls.Length,
            TopVol = 0
        };
        
        if (Balls[^1] == Ball.Empty)
            return res;//empty vial

        var cl = Ball.Empty;
        for (var i = 0; i < Balls.Length; i++)
        {
            if (Balls[i] == Ball.Empty) continue;
            
            cl = Balls[i];
            res.TopCol = cl;
            res.Empty = i;
            break;
        }

        res.TopVol = 1;
        for (var i = res.Empty + 1; i < Balls.Length; i++)
        {
            if (cl == Balls[i])
            {
                res.TopVol++;
            }
            else
            {
                break;
            }
        }

        return res;
    }
    
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
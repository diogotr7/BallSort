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
    
    public VialTopInfo getTopInfo()
    {
        var Result = new VialTopInfo
        {
            TopCol = 0,
            Empty = Global.NVOLUME,
            TopVol = 0
        };
        if (Balls[Global.NVOLUME - 1] == Ball.Empty)
        {
            return Result;//empty vial
        }

        Ball cl = Ball.Empty;
        for (var i = 0; i <= Global.NVOLUME - 1; i++)
        {
            if (Balls[i] != Ball.Empty)
            {
                cl = Balls[i];
                Result.TopCol = cl;
                Result.Empty = i;
                break;
            }
        }

        Result.TopVol = 1;
        for (var i = Result.Empty + 1; i <= Global.NVOLUME - 1; i++)
        {
            if (cl == Balls[i])
            {
                Result.TopVol++;
            }
            else
            {
                break;
            }
        }

        return Result;
    }
    
    public int vialBlocks()
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
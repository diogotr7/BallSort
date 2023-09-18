namespace PascalPort;

public class Vial
{
    public Vial(IReadOnlyList<Ball> c, byte pos)
    {
        if (c.Count != Global.NVOLUME)
        {
            throw new Exception("Vial: c.Length <> NVOLUME");
        }
        
        Balls = new Ball[c.Count];
        for (var i = 0; i <= Global.NVOLUME - 1; i++)
        {
            Balls[i] = c[i];
        }
        this.pos = pos;
    }

    public Ball[] Balls; //colors starting from top of vial
    public byte pos; //Index of vial
    
    public VialTopInfo getTopInfo()
    {
        var Result = new VialTopInfo
        {
            topcol = 0,
            empty = Global.NVOLUME,
            topvol = 0
        };
        if (Balls[Global.NVOLUME - 1] == Ball.Empty)
        {
            return Result;//empty vial
        }

        var cl = 0;
        for (var i = 0; i <= Global.NVOLUME - 1; i++)
        {
            if (Balls[i] != Ball.Empty)
            {
                cl = (int)Balls[i];
                Result.topcol = cl;
                Result.empty = i;
                break;
            }
        }

        Result.topvol = 1;
        for (var i = Result.empty + 1; i <= Global.NVOLUME - 1; i++)
        {
            if (cl == (int)Balls[i])
            {
                Result.topvol++;
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
        var Result = 1;
        for (var i = 0; i <= Global.NVOLUME - 2; i++)
        {
            if (Balls[i + 1] != Balls[i])
            {
                Result++;
            }
        }

        if (Balls[0] == Ball.Empty)
        {
            Result--;
        }

        return Result;
    }
}
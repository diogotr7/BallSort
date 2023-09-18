namespace PascalPort;

public class Vial
{
    public Vial(IReadOnlyList<Colors> c, byte pos)
    {
        if (c.Count != Global.NVOLUME)
        {
            throw new Exception("Vial: c.Length <> NVOLUME");
        }
        
        color = new Colors[c.Count];
        for (var i = 0; i <= Global.NVOLUME - 1; i++)
        {
            color[i] = c[i];
        }
        this.pos = pos;
    }

    public Colors[] color; //colors starting from top of vial
    public byte pos; //Index of vial
    
    public VialTopInfo getTopInfo()
    {
        var Result = new VialTopInfo
        {
            topcol = 0,
            empty = Global.NVOLUME,
            topvol = 0
        };
        if (color[Global.NVOLUME - 1] == Colors.EMPTY)
        {
            return Result;//empty vial
        }

        var cl = 0;
        for (var i = 0; i <= Global.NVOLUME - 1; i++)
        {
            if (color[i] != Colors.EMPTY)
            {
                cl = (int)color[i];
                Result.topcol = cl;
                Result.empty = i;
                break;
            }
        }

        Result.topvol = 1;
        for (var i = Result.empty + 1; i <= Global.NVOLUME - 1; i++)
        {
            if (cl == (int)color[i])
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
            if (color[i + 1] != color[i])
            {
                Result++;
            }
        }

        if (color[0] == Colors.EMPTY)
        {
            Result--;
        }

        return Result;
    }
}
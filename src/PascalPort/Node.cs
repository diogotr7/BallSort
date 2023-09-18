namespace PascalPort;

public class Node
{
    public Vial[] vial;
    public uint hash;
    public MoveInfo mvInfo;
    
    public Node(VialsDef t)
    {
        vial = new Vial[t.Length];
        for (var i = 0; i < t.Length; i++)
        {
            vial[i] = new Vial(t[i], (byte)i);
        }

        hash = getHash();
        mvInfo = new MoveInfo();
    }
    
    public Node(Node node)
    {
        vial = new Vial[node.vial.Length];
        for (var i = 0; i < vial.Length; i++)
        {
            vial[i] = new Vial(node.vial[i].color, node.vial[i].pos);
        }

        hash = node.hash;
        mvInfo = node.mvInfo;
    }
    
    public uint getHash()
    {
        var Result = 0u;
        for (var v = 0; v <= Global.NVIALS-1; v++)
        {
            for (var p = 0; p <= Global.NVOLUME-1; p++)
            {
                Result ^= Global.hash[(int)vial[v].color[p], p, v];
            }
        }

        return Result;
    }

    //99% confidence this is correct
    public void writeHashbit()
    {
        var base_ = hash / 64;
        var offset = hash % 64;
        var x = 1ul << (int)offset;
        //TODO: is this cast correct?
        Global.hashbits[base_] |= x;
    }

    //99% confidence this is correct
    public bool isHashedQ()
    {
        var base_ = hash / 64;
        var offset = hash % 64;
        var x = 1ul << (int)offset;
        return (Global.hashbits[base_] & x) != 0;
    }
    
    public int nodeBlocks()
    {
        var Result = 0;
        for (var i = 0; i <= Global.NVIALS - 1; i++)
        {
            Result += vial[i].vialBlocks();
        }

        return Result;
    }
    
    public bool equalQ(Node node)
    {
        for (var i = 0; i <= Global.NVIALS - 1; i++)
        {
            for (var j = 0; j <= Global.NVOLUME - 1; j++)
            {
                if (vial[i].color[j] != node.vial[i].color[j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public string lastmoves()
    {
        var Result = "";
        var j = 0;
        for (var i = 1; i <= Global.NCOLORS; i++)
        {
            j = Global.NVIALS - 1;
            while (vial[j].getTopInfo().topcol != i)
            {
                j--;
            }

            if (vial[j].getTopInfo().empty == 0)
            {
                continue; //vial with this color is full
            }

            for (var k = 0; k <= j - 1; k++)
            {
                if (vial[k].getTopInfo().topcol == i)
                {
                    for (var n = 0; n <= vial[k].getTopInfo().topvol - 1; n++)
                    {
                        Result += $"{vial[k].pos + 1}->{vial[j].pos + 1}  ";
                    }
                }
            }
        }

        if (Result == "")
        {
            Result = "Puzzle is solved!";
        }

        return Result;
    }

    public int Nlastmoves()
    {
        var Result = 0;

        for (var i = 0; i <= Global.NEMPTYVIALS - 1; i++)
        {
            Result += vial[i].getTopInfo().topvol;
        }

        return Result;
    }
    
    public int emptyVials()
    {
        var Result = 0;
        for (var i = 0; i <= Global.NVIALS - 1; i++)
        {
            if (vial[i].color[Global.NVOLUME - 1] == Colors.EMPTY)
            {
                Result++;
            }
        }

        return Result;
    }
}
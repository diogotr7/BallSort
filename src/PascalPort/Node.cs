namespace PascalPort;

public class Node
{
    public readonly Vial[] Vials;
    public uint hash;
    public MoveInfo mvInfo;

    public Node(VialsDef t, Hash h)
    {
        Vials = new Vial[t.Length];
        for (var i = 0; i < t.Length; i++)
        {
            Vials[i] = new Vial(t[i], (byte)i);
        }

        hash = getHash(h);
        mvInfo = new MoveInfo();
    }

    public Node(Node node)
    {
        Vials = new Vial[node.Vials.Length];
        for (var i = 0; i < Vials.Length; i++)
        {
            Vials[i] = new Vial(node.Vials[i].Balls, node.Vials[i].Position);
        }

        hash = node.hash;
        mvInfo = node.mvInfo;
    }

    public uint getHash(Hash h)
    {
        var Result = 0u;
        for (var v = 0; v < Vials.Length; v++)
        {
            for (var p = 0; p < Vials[0].Balls.Length; p++)
            {
                Result ^= h[(int)Vials[v].Balls[p], p, v];
            }
        }

        return Result;
    }

    //99% confidence this is correct
    public void writeHashbit(Dictionary<uint, ulong> hashbits)
    {
        var base_ = hash / 64;
        var offset = hash % 64;
        var x = 1ul << (int)offset;
        if (!hashbits.TryGetValue(base_, out var val))
            hashbits.Add(base_, 0);

        hashbits[base_] = val | x;
    }

    //99% confidence this is correct
    public bool isHashedQ(Dictionary<uint, ulong> hashbits)
    {
        var b = hash / 64;
        var offset = hash % 64;
        var x = 1ul << (int)offset;
        if (!hashbits.TryGetValue(b, out var y))
            hashbits.Add(b, 0);

        return (y & x) != 0;
    }

    public int nodeBlocks()
    {
        var Result = 0;
        for (var i = 0; i < Vials.Length; i++)
        {
            Result += Vials[i].VialBlocks();
        }

        return Result;
    }

    public bool equalQ(Node node)
    {
        for (var i = 0; i < Vials.Length; i++)
        {
            for (var j = 0; j < Vials[i].Balls.Length; j++)
            {
                if (Vials[i].Balls[j] != node.Vials[i].Balls[j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public string lastmoves(int ncolors)
    {
        var Result = "";
        var j = 0;
        for (var i = 1; i <= ncolors; i++)
        {
            j = Vials.Length - 1;
            while ((int)Vials[j].GetTopInfo().TopCol != i)
            {
                j--;
            }

            if (Vials[j].GetTopInfo().Empty == 0)
            {
                continue; //vial with this color is full
            }

            for (var k = 0; k <= j - 1; k++)
            {
                if ((int)Vials[k].GetTopInfo().TopCol == i)
                {
                    for (var n = 0; n <= Vials[k].GetTopInfo().TopVol - 1; n++)
                    {
                        Result += $"{Vials[k].Position + 1}->{Vials[j].Position + 1}  ";
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

    public int Nlastmoves(int nemptyvials)
    {
        var Result = 0;

        for (var i = 0; i < nemptyvials; i++)
        {
            Result += Vials[i].GetTopInfo().TopVol;
        }

        return Result;
    }

    public int emptyVials()
    {
        var Result = 0;
        for (var i = 0; i < Vials.Length; i++)
        {
            if (Vials[i].Balls[^1] == Ball.Empty)
            {
                Result++;
            }
        }

        return Result;
    }

    public void Sort()
    {
        Array.Sort(Vials, Compare);
    }
    
    private static int Compare(Vial v1, Vial v2)
    {
        for (var i = 0; i < v1.Balls.Length; i++)
        {
            if (v1.Balls[i] < v2.Balls[i])
                return -1;
            if (v1.Balls[i] > v2.Balls[i])
                return 1;
        }

        return 0;
    }
}
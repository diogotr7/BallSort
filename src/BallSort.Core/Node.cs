namespace PascalPort;

public class Node
{
    public readonly Vial[] Vials;
    public uint Hash;
    public MoveInfo MoveInfo;

    public Node(VialsDef def, uint[,,] hash)
    {
        Vials = new Vial[def.Length];
        for (var i = 0; i < def.Length; i++)
        {
            Vials[i] = new Vial(def[i], (byte)i);
        }

        Hash = getHash(hash);
        MoveInfo = new MoveInfo();
    }

    public Node(Node node)
    {
        Vials = new Vial[node.Vials.Length];
        for (var i = 0; i < Vials.Length; i++)
        {
            Vials[i] = new Vial(node.Vials[i].Balls, node.Vials[i].Position);
        }

        Hash = node.Hash;
        MoveInfo = node.MoveInfo;
    }

    public uint getHash(uint[,,] h)
    {
        var Result = 0u;
        for (var vial = 0; vial < Vials.Length; vial++)
        {
            for (var ball = 0; ball < Vials[0].Balls.Length; ball++)
            {
                Result ^= h[(int)Vials[vial].Balls[ball], ball, vial];
            }
        }

        return Result;
    }

    //99% confidence this is correct
    public void writeHashbit(Dictionary<uint, ulong> hashbits)
    {
        var base_ = Hash / 64;
        var offset = Hash % 64;
        var x = 1ul << (int)offset;
        if (!hashbits.TryGetValue(base_, out var val))
            hashbits.Add(base_, 0);

        hashbits[base_] = val | x;
    }

    //99% confidence this is correct
    public bool isHashedQ(Dictionary<uint, ulong> hashbits)
    {
        var b = Hash / 64;
        var offset = Hash % 64;
        var x = 1ul << (int)offset;
        if (!hashbits.TryGetValue(b, out var y))
            hashbits.Add(b, 0);

        return (y & x) != 0;
    }

    /// <summary>
    ///     
    /// </summary>
    /// <returns></returns>
    public int NodeBlocks()
    {
        return Vials.Sum(t => t.VialBlocks());
    }

    /// <summary>
    ///     Returns true if the node is equal to the current node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool Equals(Node node)
    {
        for (var vial = 0; vial < Vials.Length; vial++)
        {
            for (var ball = 0; ball < Vials[vial].Balls.Length; ball++)
            {
                if (Vials[vial].Balls[ball] != node.Vials[vial].Balls[ball])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public string LastMoves(int ncolors)
    {
        string? r = null;
        for (var i = 1; i <= ncolors; i++)
        {
            var j = Vials.Length - 1;
            while ((int)Vials[j].GetTopInfo().Color != i)
            {
                j--;
            }

            if (Vials[j].GetTopInfo().EmptyCount == 0)
            {
                continue; //vial with this color is full
            }

            for (var k = 0; k <= j - 1; k++)
            {
                if ((int)Vials[k].GetTopInfo().Color == i)
                {
                    for (var n = 0; n <= Vials[k].GetTopInfo().Count - 1; n++)
                    {
                        r += $"{Vials[k].Position + 1}->{Vials[j].Position + 1}  ";
                    }
                }
            }
        }

        return r ?? "Puzzle is solved!";
    }

    /// <summary>
///         
    /// </summary>
    /// <param name="nemptyvials"></param>
    /// <returns></returns>
    public int NLastMoves(int nemptyvials)
    {
        var r = 0;

        for (var i = 0; i < nemptyvials; i++)
        {
            r += Vials[i].GetTopInfo().Count;
        }

        return r;
    }

    /// <summary>
    ///     Returns the number of empty vials.
    /// </summary>
    /// <returns></returns>
    public int EmptyVials()
    {
        return Vials.Count(vial => vial.Balls[^1] == Ball.Empty);
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
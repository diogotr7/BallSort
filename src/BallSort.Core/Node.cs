namespace BallSort.Core;

public sealed class Node
{
    public readonly Vial[] Vials;
    public int Hash;
    public MoveInfo MoveInfo;

    public Node(Puzzle def)
    {
        Vials = new Vial[def.VialCount];
        for (byte i = 0; i < def.VialCount; i++)
        {
            Vials[i] = new Vial(def[i], i);
        }

        Hash = GetHashCode();
        MoveInfo = new MoveInfo();
    }

    public Node Clone() => new(this);

    private Node(Node node)
    {
        Vials = new Vial[node.Vials.Length];
        for (var i = 0; i < Vials.Length; i++)
        {
            Vials[i] = new Vial(node.Vials[i].Balls, node.Vials[i].Position);
        }

        Hash = node.Hash;
        MoveInfo = node.MoveInfo;
    }
    
    public void WriteHashbit(IDictionary<int, long> hashbits)
    {
        var @base = Hash / 64;
        var offset = Hash % 64;
        var x = 1L << offset;
        if (!hashbits.TryGetValue(@base, out var val))
            hashbits.Add(@base, 0);

        hashbits[@base] = val | x;
    }

    public bool IsHashedQ(IDictionary<int, long> hashbits)
    {
        var @base = Hash / 64;
        var offset = Hash % 64;
        var x = 1L << offset;
        if (!hashbits.TryGetValue(@base, out var y))
            hashbits.Add(@base, 0);

        return (y & x) != 0;
    }
    
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

    public Move[] LastMoves(int ncolors)
    {
        List<Move> moves = new();
        for (var color = 1; color <= ncolors; color++)
        {
            var j = Vials.Length - 1;
            while (Vials[j].GetTopInfo().Color != color)
            {
                j--;
            }

            if (Vials[j].GetTopInfo().EmptyCount == 0)
            {
                continue; //vial with this color is full
            }

            for (var k = 0; k < j; k++)
            {
                var topInfo = Vials[k].GetTopInfo();
                if (topInfo.Color == color)
                {
                    for (var n = 0; n < topInfo.Count; n++)
                    {
                        moves.Add(new Move(Vials[k].Position, Vials[j].Position));
                    }
                }
            }
        }

        return moves.ToArray();
    }
    
    public int NLastMoves(int nemptyvials)
    {
        var r = 0;

        for (var i = 0; i < nemptyvials; i++)
        {
            r += Vials[i].GetTopInfo().Count;
        }

        return r;
    }
    
    public int NodeBlocks() => Vials.Sum(t => t.VialBlocks());

    public int EmptyVials() => Vials.Count(vial => vial.IsEmpty());

    public void Sort() => Array.Sort(Vials);

    public override int GetHashCode()
    {
        var vialCount = Vials.Length;
        var vialDepth = Vials[0].Balls.Length;
        
        var hash = 0;
        for (var vialIndex = 0; vialIndex < vialCount; vialIndex++)
        {
            for (var ballIndex = 0; ballIndex < vialDepth; ballIndex++)
            {
                hash = hash * 31 + Vials[vialIndex].Balls[ballIndex];
            }
        }

        return hash;
    }
}
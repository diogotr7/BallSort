namespace BallSort.Core;

public sealed class Node
{
    public readonly Vial[] Vials;
    public readonly int Hash;
    public readonly MoveInfo MoveInfo;

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

    public Node PerformMove(
        int sourceVialIndex,
        int destVialIndex,
        int srcEmptyCount,
        int destEmptyCount,
        bool something)
    {
        var vials = new Vial[Vials.Length];
        Vials.CopyTo(vials, 0);
        
        var temp = vials[sourceVialIndex].Balls[srcEmptyCount];
        vials[destVialIndex].Balls[destEmptyCount - 1] = temp;
        vials[sourceVialIndex].Balls[srcEmptyCount] = 0;
        
        Array.Sort(vials);
        var moveInfo = new MoveInfo(Vials[sourceVialIndex].Position, Vials[destVialIndex].Position, something);
        return new Node(vials, moveInfo);
    }
    
    private Node(Vial[] vials, MoveInfo moveInfo)
    {
        Vials = vials;
        MoveInfo = moveInfo;
        Hash = GetHashCode();
    }
    
    public void WriteHashbit(Dictionary<int, long> hashbits)
    {
        var @base = Hash / 64;
        var offset = Hash % 64;
        var x = 1L << offset;
        if (!hashbits.TryGetValue(@base, out var val))
            hashbits.Add(@base, 0);

        hashbits[@base] = val | x;
    }

    public bool IsHashedQ(Dictionary<int, long> hashbits)
    {
        var @base = Hash / 64;
        var offset = Hash % 64;
        var x = 1L << offset;
        if (!hashbits.TryGetValue(@base, out var y))
            hashbits.Add(@base, 0);

        return (y & x) != 0;
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
    
    public bool Equals(Node node) => Vials.SequenceEqual(node.Vials);
    
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
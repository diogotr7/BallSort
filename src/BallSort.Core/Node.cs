using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NoAlloq;

namespace BallSort.Core;

public struct Node
{
    private VialCollection _vialCollection;
    public Span<Vial> Vials => _vialCollection.AsSpan(VialCount); 
    public readonly int Hash;
    public readonly MoveInfo MoveInfo;
    public readonly int VialCount;

    public Node(Puzzle def)
    {
        _vialCollection = new VialCollection();
        VialCount = def.VialCount;
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
        var copyStruct = _vialCollection;
        var vials = copyStruct.AsSpan(VialCount);
        
        var temp = vials[sourceVialIndex].Balls[srcEmptyCount];
        vials[destVialIndex].Balls[destEmptyCount - 1] = temp;
        vials[sourceVialIndex].Balls[srcEmptyCount] = 0;
        
        vials.Sort();
        var moveInfo = new MoveInfo(Vials[sourceVialIndex].Position, Vials[destVialIndex].Position, something);
        return new Node(in vials, moveInfo);
    }
    
    private Node(in Span<Vial> vials, MoveInfo moveInfo)
    {
        VialCount = vials.Length;
        vials.CopyTo(Vials);
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

    public void Sort() => Vials.Sort();

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

public record struct VialCollection
{
    public Vial Child00;
    public Vial Child01;
    public Vial Child02;
    public Vial Child03;
    public Vial Child04;
    public Vial Child05;
    public Vial Child06;
    public Vial Child07;
    public Vial Child08;
    public Vial Child09;
    public Vial Child10;
    public Vial Child11;
    public Vial Child12;
    public Vial Child13;
    public Vial Child14;
    public Vial Child15;
    
    public Span<Vial> AsSpan(int length) => MemoryMarshal.CreateSpan(ref Unsafe.AsRef(Child00), length);
}
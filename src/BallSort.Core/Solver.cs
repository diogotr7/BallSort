// ReSharper disable InconsistentNaming

namespace BallSort.Core;

public sealed class Solver
{
    private readonly Random random = new(0);
    public readonly int NCOLORS;
    public readonly int NEMPTYVIALS;
    public readonly int NVOLUME;
    public readonly int NVIALS;

    public readonly Dictionary<uint, ulong> hashbits;
    public readonly List<Node>[,] state;
    public readonly uint[,,] hash;
    public readonly Puzzle puzzle;

    public Solver(Puzzle def)
    {
        puzzle = def;
        var settings = def.GetSettings();
        
        NCOLORS = settings.FilledVialCount;
        NEMPTYVIALS = settings.EmptyVialCount;
        NVOLUME = settings.VialDepth;
        NVIALS = NCOLORS + NEMPTYVIALS;

        //We allow N_NOTDECREASE moves which do not decrease total block number???????
        const int N_NOTDECREASE = 1000;
        state = new List<Node>[NCOLORS * (NVOLUME - 1) + 1,N_NOTDECREASE + 1];
        for (var i = 0; i <= NCOLORS * (NVOLUME - 1); i++)
        {
            for (var j = 0; j <= N_NOTDECREASE; j++)
            {
                state[i, j] = new List<Node>();
            }
        }
        
        hash = new uint[NCOLORS + 1, NVOLUME, NVIALS];
        for (var i = 0; i <= NCOLORS; i++)
        {
            for (var j = 0; j <= NVOLUME - 1; j++)
            {
                for (var k = 0; k <= NVIALS - 1; k++)
                {
                    hash[i, j, k] = (uint)random.Next(int.MinValue, int.MaxValue);
                }
            }
        }

        hashbits = new Dictionary<uint, ulong>();
    }

    private bool nearoptimalSolution_single(int nblock, int y0, out Move[] moves1)
    {
        moves1 = Array.Empty<Move>();
        if (state[nblock - NCOLORS, y0].Count == 0)
            return false;

        if (nblock == NCOLORS) //puzzle is almost solved
        {
            moves1 =  state[0, 0][0].LastMoves(NCOLORS);
            return moves1.Length == 0;
        }

        var moves = new List<Move>();

        var x = nblock - NCOLORS;
        var y = y0;
        var nd = state[x, y][0];
        var mv2 = nd.LastMoves(NCOLORS);

        var src = nd.MoveInfo.Source;
        var dst = nd.MoveInfo.Destination;
        moves.Add(new Move(src + 1, dst + 1));
        if (nd.MoveInfo.Merged)
            x--;
        else
            y--;

        var solLength = 1;
        while (x != 0 || y != 0)
        {
            var ndlist = state[x, y];
            for (var i = 0; i <= ndlist.Count - 1; i++)
            {
                //keep
                var ndcand = new Node(ndlist[i]);

                var ks = 0;
                while (ndcand.Vials[ks].Position != src)
                {
                    ks++;
                }

                var kd = 0;
                while (ndcand.Vials[kd].Position != dst)
                {
                    kd++;
                }

                var viS = ndcand.Vials[ks].GetTopInfo();
                var viD = ndcand.Vials[kd].GetTopInfo();
                if (viS.EmptyCount == NVOLUME)
                {
                    continue; //source is empty vial
                }

                if (viD.EmptyCount == 0 ||
                    (viD.EmptyCount < NVOLUME && viS.Color != viD.Color) ||
                    (viD.EmptyCount == NVOLUME && viS.EmptyCount == NVOLUME - 1))
                {
                    continue;
                }

                ndcand.Vials[kd].Balls[viD.EmptyCount - 1] = viS.Color;
                ndcand.Vials[ks].Balls[viS.EmptyCount] = 0;

                ndcand.Sort();
                if (!nd.Equals(ndcand)) continue;
                
                nd = ndlist[i];
                src = nd.MoveInfo.Source;
                dst = nd.MoveInfo.Destination;
                moves.Add(new Move(src + 1, dst + 1));
                solLength++;
                if (nd.MoveInfo.Merged)
                {
                    x--;
                }
                else
                {
                    y--;
                }

                break;
            }
        }

        moves.Reverse();
        moves.AddRange(mv2);
        moves1 = moves.ToArray();
        return moves1.Length != 0;
    }

    public Solution solve_single()
    {
        var nd = new Node(puzzle, hash);
        nd.Sort();

        var y = 0;
        var nblockV = nd.NodeBlocks() + nd.EmptyVials() - NEMPTYVIALS;
        for (var i = 0; i <= nblockV - NCOLORS; i++)
            state[i, y] = new List<Node>();

        state[0, 0].Add(nd);
        nd.writeHashbit(hashbits);
        var total = 1;

        var solutionFound = false;
        int newnodes;

        do
        {
            newnodes = 0;
            for (var i = 0; i <= nblockV - NCOLORS; i++)
                state[i, y + 1] = new List<Node>(); //prepare next column

            for (var x = 0; x <= nblockV - NCOLORS - 1; x++)
            {
                //if stop
                //application.processmessages

                var ndlist = state[x, y];
                for (var i = 0; i <= ndlist.Count - 1; i++)
                {
                    nd = ndlist[i];
                    for (var ks = 0; ks <= NVIALS - 1; ks++)
                    {
                        var viS = nd.Vials[ks].GetTopInfo();
                        if (viS.EmptyCount == NVOLUME)
                        {
                            //Console.WriteLine("empty vial");
                            continue; //source is empty vial
                        }

                        for (var kd = 0; kd <= NVIALS - 1; kd++)
                        {
                            if (kd == ks)
                            {
                                //Console.WriteLine("same vial");
                                continue;
                            }

                            var viD = nd.Vials[kd].GetTopInfo();
                            if (viD.EmptyCount == 0 || //dest vial full
                                (viD.EmptyCount < NVOLUME && viS.Color != viD.Color) || //dest vial not empty and colors not equal 
                                (viD.EmptyCount == NVOLUME && viS.EmptyCount == NVOLUME - 1)) //dest vial empty and source vial has only one block
                            {
                                //Console.WriteLine("invalid move");
                                continue;
                            }

                            bool blockdecreaseQ;
                            if (viS.Count == 1 && viS.EmptyCount != NVOLUME - 1)
                                blockdecreaseQ = true;
                            else
                                blockdecreaseQ = false;
                            var ndnew = new Node(nd);
                            ndnew.Vials[kd].Balls[viD.EmptyCount - 1] = viS.Color;
                            ndnew.Vials[ks].Balls[viS.EmptyCount] = 0;
                            ndnew.Sort();
                            ndnew.Hash = ndnew.getHash(hash);
                            if (ndnew.isHashedQ(hashbits))
                            {
                                //Console.WriteLine("hash collision");
                                continue;
                            }

                            ndnew.writeHashbit(hashbits);
                            total++;
                            const int N_MAXNODES = 100000000;
                            if (total > N_MAXNODES)
                            {
                                //Form1.Memo1.Lines.Add('');
                                //Form1.Memo1.Lines.Add(Format('Node limit %d exceeded!', [N_MAXNODES]));
                                return new Solution(false, total, Array.Empty<Move>());
                            }

                            ndnew.MoveInfo.Source = nd.Vials[ks].Position;
                            ndnew.MoveInfo.Destination = nd.Vials[kd].Position;
                            if (blockdecreaseQ)
                            {
                                ndnew.MoveInfo.Merged = true;
                                state[x + 1, y].Add(ndnew);
                            }
                            else
                            {
                                ndnew.MoveInfo.Merged = false;
                                state[x, y + 1].Add(ndnew);
                                newnodes++;
                            }
                        }
                    }
                }
            }

            if (state[nblockV - NCOLORS, y].Count > 0)
                solutionFound = true;
            y++;
        } while (!(solutionFound || newnodes == 0));

        if (solutionFound)
        {
            var lmin = 99999;
            var kmin = 0;
            for (var k = 0; k <= state[nblockV - NCOLORS, y - 1].Count - 1; k++)
            {
                var j = state[nblockV - NCOLORS, y - 1][k].NLastMoves(NEMPTYVIALS);
                if (j < lmin)
                {
                    kmin = k;
                    lmin = j;
                }
            }

            (state[nblockV - NCOLORS, y - 1][0], state[nblockV - NCOLORS, y - 1][kmin]) =
                (state[nblockV - NCOLORS, y - 1][kmin], state[nblockV - NCOLORS, y - 1][0]);
        }

        var solved = nearoptimalSolution_single(nblockV, y - 1, out var moves);
        
        return new Solution(solved, total, moves);
    }
}

public class Solution
{
    public bool SolutionFound { get; }
    public int Nodes { get;  }
    public Move[] Moves { get; }
    
    public Solution(bool solutionFound, int nodes, Move[] moves)
    {
        SolutionFound = solutionFound;
        Nodes = nodes;
        Moves = moves;
    }
}

public readonly record struct Move(int From, int To);
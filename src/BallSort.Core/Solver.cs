// ReSharper disable InconsistentNaming

namespace BallSort.Core;

public sealed class Solver
{
    public readonly int NCOLORS;
    public readonly int NEMPTYVIALS;
    public readonly int NVOLUME;
    public readonly int NVIALS;

    public readonly Dictionary<int, long> hashbits;
    public readonly List<Node>[,] state;
    public readonly Puzzle puzzle;

    public Solver(Puzzle def)
    {
        puzzle = def;
        var settings = def.Settings;

        NCOLORS = settings.FilledVialCount;
        NEMPTYVIALS = settings.EmptyVialCount;
        NVOLUME = settings.VialDepth;
        NVIALS = NCOLORS + NEMPTYVIALS;

        //We allow N_NOTDECREASE moves which do not decrease total block number???????
        int N_NOTDECREASE = 1000;
        state = new List<Node>[NCOLORS * (NVOLUME - 1) + 1, N_NOTDECREASE + 1];
        for (var i = 0; i < state.GetLength(0); i++)
        {
            for (var j = 0; j < state.GetLength(1); j++)
            {
                state[i, j] = new();
            }
        }

        hashbits = new Dictionary<int, long>();
    }

    private bool nearoptimalSolution_single(int nblock, int y0, out Move[] moves1)
    {
        moves1 = Array.Empty<Move>();
        if (state[nblock - NCOLORS, y0].Count == 0)
            return false;

        if (nblock == NCOLORS) //puzzle is almost solved
        {
            moves1 = state[0, 0][0].LastMoves(NCOLORS);
            return moves1.Length == 0;
        }

        var moves = new List<Move>();

        var x = nblock - NCOLORS;
        var y = y0;
        var nd = state[x, y][0];
        var mv2 = nd.LastMoves(NCOLORS);

        var src = nd.MoveInfo.Source;
        var dst = nd.MoveInfo.Destination;
        moves.Add(new Move(src, dst));
        if (nd.MoveInfo.Merged)
            x--;
        else
            y--;

        while (x != 0 || y != 0)
        {
            var nodes = state[x, y];
            for (var i = 0; i < nodes.Count; i++)
            {
                var testNode = nodes[i];
                
                var sourceIndex = 0;
                while (testNode.Vials[sourceIndex].Position != src)
                {
                    sourceIndex++;
                }

                var destIndex = 0;
                while (testNode.Vials[destIndex].Position != dst)
                {
                    destIndex++;
                }

                var sourceVial = testNode.Vials[sourceIndex].GetTopInfo();
                var destVial = testNode.Vials[destIndex].GetTopInfo();
                if (sourceVial.EmptyCount == NVOLUME)
                {
                    continue; //source is empty vial
                }

                if (destVial.EmptyCount == 0 ||
                    (destVial.EmptyCount < NVOLUME && sourceVial.Color != destVial.Color) ||
                    (destVial.EmptyCount == NVOLUME && sourceVial.EmptyCount == NVOLUME - 1))
                {
                    continue;
                }

                var newNode = testNode.Clone();

                //move the ball
                newNode.Vials[destIndex].Balls[destVial.EmptyCount - 1] = sourceVial.Color;
                newNode.Vials[sourceIndex].Balls[sourceVial.EmptyCount] = 0;

                newNode.Sort();
                if (!nd.Equals(newNode)) 
                    continue;

                nd = testNode;
                src = nd.MoveInfo.Source;
                dst = nd.MoveInfo.Destination;
                moves.Add(new Move(src, dst));
                if (nd.MoveInfo.Merged)
                    x--;
                else
                    y--;

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
        var nd = new Node(puzzle);
        nd.Sort();

        var y = 0;
        var nblockV = nd.NodeBlocks() + nd.EmptyVials() - NEMPTYVIALS;
        state[0, 0].Add(nd);
        nd.WriteHashbit(hashbits);
        var total = 1;

        var solutionFound = false;
        int newnodes;

        do
        {
            newnodes = 0;
            for (var x = 0; x < nblockV - NCOLORS; x++)
            {
                var nodeList = state[x, y];
                for (var i = 0; i < nodeList.Count; i++)
                {
                    var node = nodeList[i];
                    for (var ks = 0; ks < NVIALS; ks++)
                    {
                        var viS = node.Vials[ks].GetTopInfo();
                        if (viS.EmptyCount == NVOLUME)
                            continue; //source is empty vial

                        for (var kd = 0; kd < NVIALS; kd++)
                        {
                            if (kd == ks)
                                continue; //same vial

                            var viD = node.Vials[kd].GetTopInfo();
                            if (viD.EmptyCount == 0 || //dest vial full
                                (viD.EmptyCount < NVOLUME && viS.Color != viD.Color) || //dest vial not empty and colors not equal 
                                (viD.EmptyCount == NVOLUME && viS.EmptyCount == NVOLUME - 1)) //dest vial empty and source vial has only one block
                                continue; //invalid move

                            var blockdecreaseQ = viS.Count == 1 && viS.EmptyCount != NVOLUME - 1;
                            var ndnew = node.Clone();
                            ndnew.Vials[kd].Balls[viD.EmptyCount - 1] = viS.Color;
                            ndnew.Vials[ks].Balls[viS.EmptyCount] = 0;
                            ndnew.Sort();
                            ndnew.Hash = ndnew.GetHashCode();
                            if (ndnew.IsHashedQ(hashbits))
                                continue; //hash collision

                            total++;
                            ndnew.WriteHashbit(hashbits);
                            if (blockdecreaseQ)
                            {
                                ndnew.MoveInfo = new MoveInfo(node.Vials[ks].Position, node.Vials[kd].Position, true);
                                state[x + 1, y].Add(ndnew);
                            }
                            else
                            {
                                ndnew.MoveInfo = new MoveInfo(node.Vials[ks].Position, node.Vials[kd].Position, false);
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
            for (var k = 0; k < state[nblockV - NCOLORS, y - 1].Count; k++)
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
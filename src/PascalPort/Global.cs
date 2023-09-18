// ReSharper disable InconsistentNaming

namespace PascalPort;

public class Global
{
    private readonly Random random = new(0);
    public readonly int NCOLORS;
    public readonly int NEMPTYVIALS;
    public readonly int NVOLUME;
    public readonly int NVIALS;

    public readonly Dictionary<uint, ulong> hashbits;
    public readonly State state;
    public readonly Hash hash;

    public Global(GameSettings settings)
    {
        NCOLORS = settings.FilledVialCount;
        NEMPTYVIALS = settings.EmptyVialCount;
        NVOLUME = settings.VialDepth;
        NVIALS = NCOLORS + NEMPTYVIALS;

        //We allow N_NOTDECREASE moves which do not decrease total block number???????
        const int N_NOTDECREASE = 1000;
        state = new State(NCOLORS * (NVOLUME - 1) + 1, N_NOTDECREASE + 1);
        hash = new Hash(NCOLORS + 1, NVOLUME, NVIALS);
        for (var i = 0; i <= NCOLORS; i++)
        {
            for (var j = 0; j <= NVOLUME - 1; j++)
            {
                for (var k = 0; k <= NVIALS - 1; k++)
                {
                    hash[i, j, k] = random.NextUInt32();
                }
            }
        }

        hashbits = new Dictionary<uint, ulong>();
    }

    public static int Compare(Vial v1, Vial v2)
    {
        for (var i = 0; i < v1.Balls.Length; i++)
        {
            if (v1.Balls[i] < v2.Balls[i])
                return 1;
            if (v1.Balls[i] > v2.Balls[i])
                return -1;
        }

        return 0;
    }

    public static void SortNode(Node node, int iLo, int iHi)
    {
        var Lo = iLo;
        var Hi = iHi;
        var Pivot = node.Vials[(Lo + Hi) / 2];
        do
        {
            while (Compare(node.Vials[Lo], Pivot) == 1)
                Lo++;
            while (Compare(node.Vials[Hi], Pivot) == -1)
                Hi--;
            if (Lo <= Hi)
            {
                (node.Vials[Lo], node.Vials[Hi]) = (node.Vials[Hi], node.Vials[Lo]);
                Lo++;
                Hi--;
            }
        } while (Lo <= Hi);

        if (Hi > iLo)
            SortNode(node, iLo, Hi);
        if (Lo < iHi)
            SortNode(node, Lo, iHi);
    }

    public string nearoptimalSolution_single(int nblock, int y0)
    {
        var i = 0;
        var x = 0;
        var y = 0;
        var src = 0;
        var dst = 0;
        var ks = 0;
        var kd = 0;
        var vmin = 0;
        var solLength = 0;
        var addmove = 0;
        Node nd = null;
        Node ndcand = null;
        var ndlist = new List<Node>();
        VialTopInfo viS;
        VialTopInfo viD;
        var resback = "";
        var mv2 = "";

        if (state[nblock - NCOLORS, y0].Count == 0)
            return "No solution. Undo moves or create new puzzle.";

        if (nblock == NCOLORS) //puzzle is almost solved
            return new Node(state[0, 0][0]).lastmoves(NCOLORS);

        var Result = "";

        x = nblock - NCOLORS;
        y = y0;
        nd = new Node(state[x, y][0]);
        addmove = nd.Nlastmoves(NEMPTYVIALS); //add last moves seperate
        mv2 = nd.lastmoves(NCOLORS);

        src = nd.mvInfo.Source;
        dst = nd.mvInfo.Destination;
        Result += $"{src + 1}->{dst + 1},";
        if (nd.mvInfo.Merged)
            x--;
        else
            y--;

        solLength = 1;
        while (x != 0 || y != 0)
        {
            ndlist = state[x, y];
            for (i = 0; i <= ndlist.Count - 1; i++)
            {
                ndcand = new Node(ndlist[i]);

                ks = 0;
                while (ndcand.Vials[ks].Position != src)
                {
                    ks++;
                }

                kd = 0;
                while (ndcand.Vials[kd].Position != dst)
                {
                    kd++;
                }

                viS = ndcand.Vials[ks].GetTopInfo();
                viD = ndcand.Vials[kd].GetTopInfo();
                if (viS.Empty == NVOLUME)
                {
                    continue; //source is empty vial
                }

                if (viD.Empty == 0 ||
                    (viD.Empty < NVOLUME && viS.TopCol != viD.TopCol) ||
                    (viD.Empty == NVOLUME && viS.Empty == NVOLUME - 1))
                {
                    continue;
                }

                ndcand.Vials[kd].Balls[viD.Empty - 1] = (Ball)viS.TopCol;
                ndcand.Vials[ks].Balls[viS.Empty] = Ball.Empty;

                SortNode(ndcand, 0, NVIALS - 1);
                if (nd.equalQ(ndcand))
                {
                    nd = new Node(ndlist[i]);
                    src = nd.mvInfo.Source;
                    dst = nd.mvInfo.Destination;
                    Result += $"{src + 1}->{dst + 1},";
                    solLength++;
                    if (nd.mvInfo.Merged)
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
        }

        Console.WriteLine($"Near-Optimal solution in {solLength + addmove} moves");
        var reversed = string.Join(" | ", Result.Split(',').Reverse());
        reversed += $" | {mv2}";
        return reversed;
    }

    public void solve_single(VialsDef vialDefinition)
    {
        Node nd;
        Node ndnew;
        int nblockV;
        int i;
        int j;
        int k;
        int lmin;
        int kmin;
        int x;
        int y;
        int ks;
        int kd;
        int newnodes;
        int total;
        List<Node> ndlist;
        VialTopInfo viS;
        VialTopInfo viD;
        bool blockdecreaseQ;
        bool solutionFound;

        //TODO: throw if vialDefinition has wrong size

        nd = new Node(vialDefinition, hash);
        SortNode(nd, 0, NVIALS - 1);

        y = 0;
        nblockV = nd.nodeBlocks() + nd.emptyVials() - NEMPTYVIALS;
        for (i = 0; i <= nblockV - NCOLORS; i++)
            state[i, y] = new List<Node>();

        state[0, 0].Add(nd);
        nd.writeHashbit(hashbits);
        total = 1;

        solutionFound = false;
        do
        {
            newnodes = 0;
            for (i = 0; i <= nblockV - NCOLORS; i++)
                state[i, y + 1] = new List<Node>(); //prepare next column

            for (x = 0; x <= nblockV - NCOLORS - 1; x++)
            {
                //if stop
                //application.processmessages

                ndlist = state[x, y];
                for (i = 0; i <= ndlist.Count - 1; i++)
                {
                    nd = new Node(ndlist[i]);
                    for (ks = 0; ks <= NVIALS - 1; ks++)
                    {
                        viS = nd.Vials[ks].GetTopInfo();
                        if (viS.Empty == NVOLUME)
                        {
                            //Console.WriteLine("empty vial");
                            continue; //source is empty vial
                        }

                        for (kd = 0; kd <= NVIALS - 1; kd++)
                        {
                            if (kd == ks)
                            {
                                //Console.WriteLine("same vial");
                                continue;
                            }

                            viD = nd.Vials[kd].GetTopInfo();
                            if (viD.Empty == 0 || //dest vial full
                                (viD.Empty < NVOLUME && viS.TopCol != viD.TopCol) || //dest vial not empty and colors not equal 
                                (viD.Empty == NVOLUME && viS.Empty == NVOLUME - 1)) //dest vial empty and source vial has only one block
                            {
                                //Console.WriteLine("invalid move");
                                continue;
                            }

                            if (viS.TopVol == 1 && viS.Empty != NVOLUME - 1)
                                blockdecreaseQ = true;
                            else
                                blockdecreaseQ = false;
                            ndnew = new Node(nd);
                            ndnew.Vials[kd].Balls[viD.Empty - 1] = (Ball)viS.TopCol;
                            ndnew.Vials[ks].Balls[viS.Empty] = Ball.Empty;
                            SortNode(ndnew, 0, NVIALS - 1);
                            ndnew.hash = ndnew.getHash(hash);
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
                                return;
                            }

                            ndnew.mvInfo.Source = nd.Vials[ks].Position;
                            ndnew.mvInfo.Destination = nd.Vials[kd].Position;
                            if (blockdecreaseQ)
                            {
                                ndnew.mvInfo.Merged = true;
                                state[x + 1, y].Add(ndnew);
                            }
                            else
                            {
                                ndnew.mvInfo.Merged = false;
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
            lmin = 99999;
            kmin = 0;
            for (k = 0; k <= state[nblockV - NCOLORS, y - 1].Count - 1; k++)
            {
                j = new Node(state[nblockV - NCOLORS, y - 1][k]).Nlastmoves(NEMPTYVIALS);
                if (j < lmin)
                {
                    kmin = k;
                    lmin = j;
                }
            }

            (state[nblockV - NCOLORS, y - 1][0], state[nblockV - NCOLORS, y - 1][kmin]) =
                (state[nblockV - NCOLORS, y - 1][kmin], state[nblockV - NCOLORS, y - 1][0]);
        }

        Console.WriteLine($"{total} nodes generated");
        Console.WriteLine(nearoptimalSolution_single(nblockV, y - 1));
    }
}
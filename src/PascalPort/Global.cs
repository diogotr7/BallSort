// ReSharper disable InconsistentNaming
namespace PascalPort;

public static class Global
{
    private static readonly Random random = new();
    public const int NCOLORS = 2;
    public const int NEMPTYVIALS = 1;
    public const int NVOLUME = 4;
    public const int NVIALS = NCOLORS + NEMPTYVIALS;
    
    public static readonly ulong[] hashbits;
    public static readonly State state;
    public static readonly Hash hash;
    public static VialsDef vialDefinition;

    static Global()
    {
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

        //idfk why this is this big
        hashbits = new ulong[67108864];

        vialDefinition = new VialsDef(NVIALS, NVOLUME);
        //full vials
        for (var i = 0; i <= NCOLORS - 1; i++)
        {
            for (var j = 0; j <= NVOLUME - 1; j++)
            {
                vialDefinition[i, j] = (Colors)(i + 1);
            }
        }

        //empty vials
        for (var i = NCOLORS; i <= NVIALS - 1; i++)
        {
            for (var j = 0; j <= NVOLUME - 1; j++)
            {
                vialDefinition[i, j] = Colors.EMPTY;
            }
        }
    }

    public static int Compare(Vial v1, Vial v2)
    {
        for (var i = 0; i <= NVOLUME - 1; i++)
        {
            if (v1.color[i] < v2.color[i])
                return 1;
            if (v1.color[i] > v2.color[i])
                return -1;
        }

        return 0;
    }

    public static void SortNode(Node node, int iLo, int iHi)
    {
        var Lo = iLo;
        var Hi = iHi;
        var Pivot = node.vial[(Lo + Hi) / 2];
        do
        {
            while (Compare(node.vial[Lo], Pivot) == 1)
                Lo++;
            while (Compare(node.vial[Hi], Pivot) == -1)
                Hi--;
            if (Lo <= Hi)
            {
                (node.vial[Lo], node.vial[Hi]) = (node.vial[Hi], node.vial[Lo]);
                Lo++;
                Hi--;
            }
        } while (Lo <= Hi);

        if (Hi > iLo)
            SortNode(node, iLo, Hi);
        if (Lo < iHi)
            SortNode(node, Lo, iHi);
    }

    public static void Shuffle()
    {
        for (var i = NVOLUME * NCOLORS - 1; i > 1; i--)
        {
            var j = random.Next(i + 1);
            (vialDefinition[j / NVOLUME, j % NVOLUME], vialDefinition[i / NVOLUME, i % NVOLUME]) =
                (vialDefinition[i / NVOLUME, i % NVOLUME], vialDefinition[j / NVOLUME, j % NVOLUME]);
        }
    }

    public static string nearoptimalSolution_single(int nblock, int y0)
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

        if (nblock == NCOLORS)//puzzle is almost solved
            return new Node(state[0, 0][0]).lastmoves();

        var Result = "";

        x = nblock - NCOLORS;
        y = y0;
        nd = new Node(state[x, y][0]);
        addmove = nd.Nlastmoves(); //add last moves seperate
        mv2 = nd.lastmoves();
        
        src = nd.mvInfo.srcVial;
        dst = nd.mvInfo.dstVial;
        Result += $"{src + 1}->{dst + 1},";
        if (nd.mvInfo.merged)
            x--;
        else
            y--;

        solLength = 1;
        while (x != 0 || y != 0)
        {
            ndlist = state[x, y];
            if (ndlist.Count == 0)
            {
                break;
            }
            Console.WriteLine($"x={x}, y={y}, ndlist.Count={ndlist.Count}");
            for (i = 0; i <= ndlist.Count - 1; i++)
            {
                ndcand = new Node(ndlist[i]);
                
                ks = 0;
                while (ndcand.vial[ks].pos != src)
                {
                    ks++;
                }

                kd = 0;
                while (ndcand.vial[kd].pos != dst)
                {
                    kd++;
                }

                viS = ndcand.vial[ks].getTopInfo();
                viD = ndcand.vial[kd].getTopInfo();
                if (viS.empty == NVOLUME)
                {
                    continue; //source is empty vial
                }

                if (viD.empty == 0 ||
                    (viD.empty < NVOLUME && viS.topcol != viD.topcol) ||
                    (viD.empty == NVOLUME && viS.empty == NVOLUME - 1))
                {
                    continue;
                }

                ndcand.vial[kd].color[viD.empty - 1] = (Colors)viS.topcol;
                ndcand.vial[ks].color[viS.empty] = Colors.EMPTY;

                SortNode(ndcand, 0, NVIALS - 1);
                if (nd.equalQ(ndcand))
                {
                    nd = new Node(ndlist[i]);
                    src = nd.mvInfo.srcVial;
                    dst = nd.mvInfo.dstVial;
                    Result += $"{src + 1}->{dst + 1},";
                    solLength++;
                    if (nd.mvInfo.merged)
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
    
    public static void solve_single(VialsDef def)
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
        
        //InitFalse
        
        nd = new Node(def);
        SortNode(nd, 0, NVIALS - 1);
        
        y = 0;
        nblockV = nd.nodeBlocks() + nd.emptyVials() - NEMPTYVIALS;
        for (i = 0; i <= nblockV - NCOLORS; i++)
            state[i, y] = new List<Node>();
        
        state[0, 0].Add(nd);
        nd.writeHashbit();
        total = 1;
        
        solutionFound = false;
        do
        {
            newnodes = 0;
            for (i = 0; i <= nblockV - NCOLORS; i++)
                state[i, y + 1] = new List<Node>();//prepare next column
            
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
                        viS = nd.vial[ks].getTopInfo();
                        if (viS.empty == NVOLUME)
                        {
                            //Console.WriteLine("empty vial");
                            continue;//source is empty vial
                        }

                        for (kd = 0; kd <= NVIALS - 1; kd++)
                        {
                            if (kd == ks)
                            {
                                //Console.WriteLine("same vial");
                                continue;
                            }

                            viD = nd.vial[kd].getTopInfo();
                            if (viD.empty == 0 || //dest vial full
                                (viD.empty < NVOLUME && viS.topcol != viD.topcol) || //dest vial not empty and colors not equal 
                                (viD.empty == NVOLUME && viS.empty == NVOLUME - 1)) //dest vial empty and source vial has only one block
                            {
                                //Console.WriteLine("invalid move");
                                continue;
                            }

                            if (viS.topvol == 1 && viS.empty != NVOLUME - 1)
                                blockdecreaseQ = true;
                            else
                                blockdecreaseQ = false;
                            ndnew = new Node(nd);
                            ndnew.vial[kd].color[viD.empty - 1] = (Colors)viS.topcol;
                            ndnew.vial[ks].color[viS.empty] = Colors.EMPTY;
                            SortNode(ndnew, 0, NVIALS - 1);
                            ndnew.hash = ndnew.getHash();
                            if (ndnew.isHashedQ())
                            {
                                ndnew = null;
                                //Console.WriteLine("hash collision");
                                continue;
                            }
                            ndnew.writeHashbit();
                            total++;
                            const int N_MAXNODES = 100000000;
                            if (total > N_MAXNODES)
                            {
                                //Form1.Memo1.Lines.Add('');
                                //Form1.Memo1.Lines.Add(Format('Node limit %d exceeded!', [N_MAXNODES]));
                                return;
                            }
                            ndnew.mvInfo.srcVial = nd.vial[ks].pos;
                            ndnew.mvInfo.dstVial = nd.vial[kd].pos;
                            if (blockdecreaseQ)
                            {
                                ndnew.mvInfo.merged = true;
                                state[x + 1, y].Add(ndnew);
                            }
                            else
                            {
                                ndnew.mvInfo.merged = false;
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
                j = new Node(state[nblockV - NCOLORS, y - 1][k]).Nlastmoves();
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
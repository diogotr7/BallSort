namespace BallSort;

public sealed record GameState(Vial[] Vials)
{
    public readonly Vial[] Vials = Vials;

    public bool IsSolved()
    {
        for (var i = 0; i < Vials.Length; i++)
        {
            var vial = Vials[i];
            
            //empty vials are ok
            if (vial.IsEmpty())
                continue;
            
            //all balls within a vial must be the same color
            if (!vial.AllSameColor())
                return false;
        }

        return true;
    }

    public bool IsMoveLegal(Move move)
    {
        var fromVial = Vials[move.FromVial];
        var toVial = Vials[move.ToVial];
        
        if (fromVial.IsEmpty())
            return false;

        if (toVial.IsFull())
            return false;

        if (!toVial.IsEmpty() && fromVial.Peek() != toVial.Peek())
            return false;
        
        return true;
    }
    
    public void PerformMove(Move move)
    {
        var fromVial = Vials[move.FromVial];
        var toVial = Vials[move.ToVial];
        
        var ball = fromVial.Pop();
        toVial.Push(ball);
    }
    
    public void UndoMove(Move move)
    {
        var fromVial = Vials[move.FromVial];
        var toVial = Vials[move.ToVial];
        
        var ball = toVial.Pop();
        fromVial.Push(ball);
    }

    public List<Move> Solve()
    {
        //keep track of done moves so we can undo them if we hit a dead end
        var moves = new List<Move>();
        
        //keep track of visited states so we don't visit them again (hashCode is used to compare states)
        var visitedStates = new HashSet<int>();
        
        //keep track of the current state
        visitedStates.Add(GetHashCode());
        int iteration = 0;
        while (!IsSolved())
        {
            iteration++;
            var moveFound = false;
            for (var fromVial = 0; fromVial < Vials.Length; fromVial++)
            {
                for (var toVial = 0; toVial < Vials.Length; toVial++)
                {
                    if (fromVial == toVial)
                        continue;
                    
                    var move = new Move(fromVial, toVial);
                    if (!IsMoveLegal(move))
                        continue;
                    //try the move
                    PerformMove(move);
                    var thisHash = GetHashCode();

                    //if we've already visited this state, undo the move and try again
                    if (visitedStates.Contains(thisHash))
                    {
                        UndoMove(move);
                        continue;
                    }
                    
                    //we found a new state, add it to the visited states and continue
                    visitedStates.Add(thisHash);
                    moves.Add(move);
                    moveFound = true;
                    break;
                }

                if (moveFound)
                    break;
            }

            if (!moveFound)
            {
                //no move found, we hit a dead end
                //undo the last move and try again
                var lastMove = moves[^1];
                moves.RemoveAt(moves.Count - 1);
                UndoMove(lastMove);
            }
            
        }
        Console.WriteLine($"Solved in {iteration} iterations");
        return moves;
    }

    private List<Move> GeneratePossibleMoves()
    {
        List<Move> moves = new List<Move>();

        for (int from = 0; from < Vials.Length; from++)
        {
            for (int to = 0; to < Vials.Length; to++)
            {
                if (from == to)
                    continue;
                moves.Add(new Move(from, to));
            }
        }

        return moves;
    }
    private List<Move> solution = new List<Move>();

    public List<Move> SolveGpt()
    {
        if (IsSolved())
        {
            return solution; // The initial state is already solved
        }

        Queue<GameState> stateQueue = new Queue<GameState>();
        HashSet<int> visitedStates = new HashSet<int>();

        stateQueue.Enqueue(this);
        visitedStates.Add(GetHashCode());

        while (stateQueue.Count > 0)
        {
            GameState currentState = stateQueue.Dequeue();

            foreach (Move move in GeneratePossibleMoves())
            {
                if (currentState.IsMoveLegal(move))
                {
                    GameState nextState = currentState.CloneYes();
                    nextState.PerformMove(move);

                    if (!visitedStates.Contains(nextState.GetHashCode()))
                    {
                        stateQueue.Enqueue(nextState);
                        visitedStates.Add(nextState.GetHashCode());

                        if (nextState.IsSolved())
                        {
                            return nextState.solution;
                        }
                    }
                }
            }
        }

        throw new InvalidOperationException("No solution found");
    }

    private GameState CloneYes()
    {
        Vial[] newVials = new Vial[Vials.Length];
        for (int i = 0; i < Vials.Length; i++)
        {
            newVials[i] = new Vial(Vials[i].Capacity);
            for (int j = 0; j < Vials[i].Balls.Length; j++)
            {
                var ball = Vials[i].Balls[j];
                if (ball == -1)
                    break;
                newVials[i].Push(Vials[i].Balls[j]);
            }
        }

        return new GameState(newVials);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            foreach (var vial in Vials)
                hash = hash * 23 + vial.GetHashCode();
            return hash;
        }
    }
}
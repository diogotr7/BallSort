namespace BallSort;

public static class PuzzleGenerator
{
    public static GameState Generate(int numVials, int numBalls, int numColors, int seed = 0)
    {
        //generate random solvable initial state. Enough empty vials to solve the puzzle
        var initialBalls = new List<int>(numBalls * numColors);
        for (var i = 0; i < numBalls; i++)
        {
            for (var j = 0; j < numColors; j++)
            {
                initialBalls.Add(j);
            }
        }
        
        //shuffle the balls
        var r = new Random(seed);
        var balls = new Stack<int>(initialBalls.OrderBy(x => r.NextInt64()).ToArray());
        
        //create the vials
        var vials = new Vial[numVials];
        for (var i = 0; i < numVials; i++)
        {
            vials[i] = new Vial(numBalls);
        }
        
        //fill the vials
        for (var i = 0; i < numVials; i++)
        {
            for (var j = 0; j < numBalls; j++)
            {
                if(balls.Count > 0)
                    vials[i].Push(balls.Pop());
            }
        }
        
        return new GameState(vials);
    }
}
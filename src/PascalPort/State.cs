namespace PascalPort;

public class State
{
    private readonly List<Node>[][] _state;
    
    public List<Node> this[int i, int j]
    {
        get => _state[i][j];
        set => _state[i][j] = value;
    }

    public State(int x, int y)
    {
        _state = new List<Node>[x][];
        for (var i = 0; i < x; i++)
        {
            _state[i] = new List<Node>[y];
            for (var j = 0; j < y; j++)
            {
                _state[i][j] = new List<Node>();
            }
        }
    }
}
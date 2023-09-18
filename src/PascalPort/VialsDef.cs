namespace PascalPort;

public class VialsDef
{
    //array of array of TColors
    private readonly Ball[][] _vialsDef;
    
    public int Length => _vialsDef.Length;
    
    public Ball this[int i, int j]
    {
        get => _vialsDef[i][j];
        set => _vialsDef[i][j] = value;
    }
    
    public Ball[] this[int i] => _vialsDef[i];

    public VialsDef(int x, int y)
    {
        _vialsDef = new Ball[x][];
        for (var i = 0; i < x; i++)
        {
            _vialsDef[i] = new Ball[y];
        }
    }
    
    public static VialsDef Parse(string s)
    {
        var lines = s.Split(Environment.NewLine);
        var result = new VialsDef(lines.Length, lines[0].Length);
        for (var i = 0; i < lines.Length; i++)
        {
            for (var j = 0; j < lines[i].Length; j++)
            {
                result[i, j] = (Ball)int.Parse(lines[i][j].ToString());
            }
        }

        return result;
    }
}
namespace PascalPort;

public class VialsDef
{
    //array of array of TColors
    private readonly Colors[][] _vialsDef;
    
    public int Length => _vialsDef.Length;
    
    public Colors this[int i, int j]
    {
        get => _vialsDef[i][j];
        set => _vialsDef[i][j] = value;
    }
    
    public Colors[] this[int i] => _vialsDef[i];

    public VialsDef(int x, int y)
    {
        _vialsDef = new Colors[x][];
        for (var i = 0; i < x; i++)
        {
            _vialsDef[i] = new Colors[y];
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
                result[i, j] = (Colors)int.Parse(lines[i][j].ToString());
            }
        }

        return result;
    }
}
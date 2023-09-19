namespace PascalPort;

public class VialsDef
{
    //array of array of TColors
    private readonly Ball[][] _vials;
    
    public int Length => _vials.Length;
    
    public Ball this[int i, int j]
    {
        get => _vials[i][j];
        set => _vials[i][j] = value;
    }
    
    public Ball[] this[int i] => _vials[i];

    public VialsDef(int x, int y)
    {
        _vials = new Ball[x][];
        for (var i = 0; i < x; i++)
        {
            _vials[i] = new Ball[y];
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
    
    public static VialsDef CreateRandom(GameSettings gameSettings, int? seed = null)
    {
        var random = new Random(seed ?? Environment.TickCount);
        
        var fullVialCount = gameSettings.FilledVialCount;
        var emptyVialCount = gameSettings.EmptyVialCount;
        var vialDepth = gameSettings.VialDepth;
        var totalVialCount = fullVialCount + emptyVialCount;
        
        var vialDefinition = new VialsDef(totalVialCount, vialDepth);

        //full vials
        for (var i = 0; i < fullVialCount; i++)
        {
            for (var j = 0; j < vialDepth; j++)
            {
                vialDefinition[i, j] = (Ball)(i + 1);
            }
        }

        //empty vials
        for (var i = fullVialCount; i < totalVialCount; i++)
        {
            for (var j = 0; j < vialDepth; j++)
            {
                vialDefinition[i, j] = Ball.Empty;
            }
        }

        //shuffle
        for (var i = vialDepth * fullVialCount - 1; i > 1; i--)
        {
            var j = random.Next(i + 1);
            (vialDefinition[j / vialDepth, j % vialDepth], vialDefinition[i / vialDepth, i % vialDepth]) =
                (vialDefinition[i / vialDepth, i % vialDepth], vialDefinition[j / vialDepth, j % vialDepth]);
        }

        return vialDefinition;
    }
}
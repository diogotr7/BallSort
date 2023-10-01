namespace BallSort.Core;

public sealed class Puzzle
{
    private readonly byte[][] _vials;

    public int VialCount => _vials.Length;

    public GameSettings GetSettings()
    {
        return new GameSettings
        (
            _vials.Count(v => v.Any(b => b != 0)),
            _vials.Count(v => v.All(b => b == 0)),
            _vials[0].Length
        );
    }

    public byte this[int i, int j]
    {
        get => _vials[i][j];
        set => _vials[i][j] = value;
    }

    public byte[] this[int i] => _vials[i];

    public Puzzle(int vialCount, int vialDepth)
    {
        _vials = new byte[vialCount][];
        for (var i = 0; i < vialCount; i++)
        {
            _vials[i] = new byte[vialDepth];
        }
    }

    public static Puzzle Parse(string s)
    {
        var lines = s.Split(Environment.NewLine);
        var result = new Puzzle(lines.Length, lines[0].Length);
        for (var i = 0; i < lines.Length; i++)
        {
            for (var j = 0; j < lines[i].Length; j++)
            {
                result[i, j] = GetInt(lines[i][j]);
            }
        }

        return result;
    }

    public void Dump(TextWriter writer)
    {
        for (var i = 0; i < VialCount; i++)
        {
            for (var j = 0; j < this[i].Length; j++)
            {
                writer.Write(GetChar(this[i][j]));
            }

            if (i != VialCount - 1)
                writer.WriteLine();
        }
    }

    private static char GetChar(int n)
    {
        //0123456789ABCDEF etc
        return n switch
        {
            0 => '_',
            < 10 => (char)(n + '0'),
            _ => (char)(n - 10 + 'A'),
        };
    }

    private static byte GetInt(char c)
    {
        //0123456789ABCDEF etc
        return c switch
        {
            '_' => 0,
            < 'A' => (byte)(c - '0'),
            _ => (byte)(c - 'A' + 10),
        };
    }

    public static Puzzle CreateRandom(GameSettings gameSettings, int? seed = null)
    {
        var random = new Random(seed ?? Environment.TickCount);

        var fullVialCount = gameSettings.FilledVialCount;
        var emptyVialCount = gameSettings.EmptyVialCount;
        var vialDepth = gameSettings.VialDepth;
        var totalVialCount = fullVialCount + emptyVialCount;

        var vialDefinition = new Puzzle(totalVialCount, vialDepth);

        //full vials
        for (var i = 0; i < fullVialCount; i++)
        {
            for (var j = 0; j < vialDepth; j++)
            {
                vialDefinition[i, j] = (byte)(i + 1);
            }
        }

        //empty vials
        for (var i = fullVialCount; i < totalVialCount; i++)
        {
            for (var j = 0; j < vialDepth; j++)
            {
                vialDefinition[i, j] = 0;
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
namespace PascalPort;

public sealed class Hash
{
    private readonly uint[][][] _hash;
    
    public uint this[int i, int j, int k]
    {
        get => _hash[i][j][k];
        set => _hash[i][j][k] = value;
    }

    public Hash(int x, int y, int z)
    {
        _hash = new uint[x][][];
        for (var i = 0; i < x; i++)
        {
            _hash[i] = new uint[y][];
            for (var j = 0; j < y; j++)
            {
                _hash[i][j] = new uint[z];
            }
        }
    }
}
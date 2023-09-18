namespace PascalPort;

public class Hash
{
    //array of array of array of uint32
    
    private readonly uint[][][] _hash;
    
    public uint this[int i, int j, int k]
    {
        get => _hash[i][j][k];
        set => _hash[i][j][k] = value;
    }

    public Hash(int x, int y, int z)
    {
        _hash = new uint[x][][];
        for (var i = 0; i <= x - 1; i++)
        {
            _hash[i] = new uint[y][];
            for (var j = 0; j <= y - 1; j++)
            {
                _hash[i][j] = new uint[z];
            }
        }
    }
}
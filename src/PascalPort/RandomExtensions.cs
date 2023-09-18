namespace PascalPort;

public static class RandomExtensions
{
    public static uint NextUInt32(this Random random)
    {
        return (uint)random.Next(int.MinValue, int.MaxValue);
    }
}
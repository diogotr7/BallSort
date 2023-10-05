namespace BallSort.Core;

public static class Utilities
{
    public static char RenderBall(byte ball) => ball switch
    {
        0 => '_',
        < 10 => (char)(ball + '0'),
        _ => (char)(ball - 10 + 'A'),
    };
    
    public static byte ParseBall(char c) => c switch
    {
        '_' => 0,
        < 'A' => (byte)(c - '0'),
        _ => (byte)(c - 'A' + 10),
    };
}
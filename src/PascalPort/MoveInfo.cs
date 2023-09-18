namespace PascalPort;

public struct MoveInfo
{
    public byte srcVial; //source and destination of a move
    public byte dstVial;
    public bool merged; //move reduced number of blocks or keeps number
}
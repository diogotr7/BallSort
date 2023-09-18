namespace BallSort;

public readonly record struct Move(int FromVial, int ToVial)
{
    public readonly int FromVial = FromVial;
    public readonly int ToVial = ToVial;
}
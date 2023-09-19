using OpenCvSharp;

namespace BallSort.OpenCv;

public readonly struct Color : IEquatable<Color>
{
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }

    public Color(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }
    
    public static Color FromScalar(Scalar scalar) => new((byte)scalar[2], (byte)scalar[1], (byte)scalar[0]);
    
    public static implicit operator Color(Scalar color) => new((byte)color[2], (byte)color[1], (byte)color[0]);

    public override string ToString()
    {
        return $"R: {R}, G: {G}, B: {B}";
    }

    private const int Tolerance = 5;

    public bool Equals(Color other)
    {
        return Math.Abs(R - other.R) < Tolerance && Math.Abs(G - other.G) < Tolerance && Math.Abs(B - other.B) < Tolerance;
    }

    public override bool Equals(object? obj)
    {
        return obj is Color other && Equals(other);
    }

    public override int GetHashCode()
    {
        return 1;
    }

    public static bool operator ==(Color left, Color right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Color left, Color right)
    {
        return !(left == right);
    }
}
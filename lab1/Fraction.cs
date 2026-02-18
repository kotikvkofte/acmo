namespace lab1;

public struct Fraction
{
    public long Numerator { get; private set; }
    public long Denominator { get; private set; }
    public Fraction Abs => new(Math.Abs(Numerator), Denominator);
    public bool IsZero => Numerator == 0;    
    public Fraction(long numerator, long denominator = 1)
    {
        if (denominator == 0)
            throw new DivideByZeroException("Denominator cannot be 0");
        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        var gcd = Gcd(Math.Abs(numerator), denominator);
        Numerator = numerator / gcd;
        Denominator = denominator / gcd;
    }

    private static long Gcd(long a, long b)
    {
        while (b != 0)
            (a, b) = (b, a % b);

        return a;
    }


    public static Fraction operator +(Fraction a, Fraction b) =>
        new(a.Numerator * b.Denominator + a.Denominator * b.Numerator, a.Denominator * b.Denominator);

    public static Fraction operator -(Fraction a, Fraction b) =>
        new(a.Numerator * b.Denominator - a.Denominator * b.Numerator, a.Denominator * b.Denominator);

    public static Fraction operator *(Fraction a, Fraction b) =>
        new(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

    public static Fraction operator /(Fraction a, Fraction b) =>
        b.Numerator == 0
            ? throw new DivideByZeroException("Denominator cannot be 0")
            : new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator);

    public static bool operator ==(Fraction a, Fraction b) =>
        a.Numerator == b.Numerator && a.Denominator == b.Denominator;

    public static bool operator !=(Fraction a, Fraction b) => !(a == b);

    public static bool operator <(Fraction a, Fraction b) =>
        (double)a.Numerator / a.Denominator < (double)b.Numerator / b.Denominator;

    public static bool operator >(Fraction a, Fraction b) =>
        (double)a.Numerator / a.Denominator > (double)b.Numerator / b.Denominator;

    public override string ToString() => 
        Denominator == 1 
            ? Numerator.ToString() 
            : $"{Numerator}/{Denominator}";
}
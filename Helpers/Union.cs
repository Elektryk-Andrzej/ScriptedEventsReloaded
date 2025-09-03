namespace SER.Helpers;

public record struct Union<T1, T2>
{
    public T1? Item1 { get; init; }
    public T2? Item2 { get; init; }
    
    public static implicit operator Union<T1, T2>(T1 value)
    {
        return new()
        {
            Item1 = value,
            Item2 = default
        };
    }
    
    public static implicit operator Union<T1, T2>(T2 value)
    {
        return new()
        {
            Item1 = default,
            Item2 = value
        };
    }
}
namespace SER.Helpers;

public class Union<T1, T2>
{
    public T1? Item1 { get; init; }
    public T2? Item2 { get; init; }

    public static implicit operator Union<T1, T2>(T1 value) => new() { Item1 = value };
    public static implicit operator Union<T1, T2>(T2 value) => new() { Item2 = value };
}

public class Union<T1, T2, T3>
{
    public T1? Item1 { get; init; }
    public T2? Item2 { get; init; }
    public T3? Item3 { get; init; }

    public static implicit operator Union<T1, T2, T3>(T1 value) => new() { Item1 = value };
    public static implicit operator Union<T1, T2, T3>(T2 value) => new() { Item2 = value };
    public static implicit operator Union<T1, T2, T3>(T3 value) => new() { Item3 = value };
}

public class Union<T1, T2, T3, T4>
{
    public T1? Item1 { get; init; }
    public T2? Item2 { get; init; }
    public T3? Item3 { get; init; }
    public T4? Item4 { get; init; }

    public static implicit operator Union<T1, T2, T3, T4>(T1 value) => new() { Item1 = value };
    public static implicit operator Union<T1, T2, T3, T4>(T2 value) => new() { Item2 = value };
    public static implicit operator Union<T1, T2, T3, T4>(T3 value) => new() { Item3 = value };
    public static implicit operator Union<T1, T2, T3, T4>(T4 value) => new() { Item4 = value };
}

public class Union<T1, T2, T3, T4, T5>
{
    public T1? Item1 { get; init; }
    public T2? Item2 { get; init; }
    public T3? Item3 { get; init; }
    public T4? Item4 { get; init; }
    public T5? Item5 { get; init; }

    public static implicit operator Union<T1, T2, T3, T4, T5>(T1 value) => new() { Item1 = value };
    public static implicit operator Union<T1, T2, T3, T4, T5>(T2 value) => new() { Item2 = value };
    public static implicit operator Union<T1, T2, T3, T4, T5>(T3 value) => new() { Item3 = value };
    public static implicit operator Union<T1, T2, T3, T4, T5>(T4 value) => new() { Item4 = value };
    public static implicit operator Union<T1, T2, T3, T4, T5>(T5 value) => new() { Item5 = value };
}

namespace SER.Helpers.ResultSystem;

public interface ITryGet<out T>
{
    public T? Value { get; }
    public string? ErrorMsg { get; }
    public bool WasSuccess { get; }
}
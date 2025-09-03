namespace SER.VariableSystem.Structures;

public class ValueCoordinate
{
    public required int StartIndex { get; init; }
    public required int EndIndex { get; init; }
    public required string OriginalValue { get; init; }
    public required string ResolvedValue { get; init; }
        
    public int Length => EndIndex - StartIndex + 1;
}

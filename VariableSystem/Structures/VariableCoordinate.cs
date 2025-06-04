namespace SER.VariableSystem.Structures;

public class VariableCoordinate
{
    public int StartIndex { get; init; }
    public int EndIndex { get; init; }
    public required string PlaceholderText { get; set; }
    public required string VariableName { get; init; }
    public required string ResolvedValue { get; init; }
    public VariableCoordinateType Type { get; init; }
        
    public int Length => EndIndex - StartIndex + 1;
    public bool IsValid => Type != VariableCoordinateType.Invalid;
}

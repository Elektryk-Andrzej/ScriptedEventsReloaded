using SER.TokenSystem.Slices;
using SER.TokenSystem.Tokens;

namespace SER.TokenSystem.Structures;

public class Line
{
    public required uint LineNumber { get; init; }
    public required string RawRepresentation { get; init; }
    public Slice[] Slices = [];
    public BaseToken[] Tokens = [];
}
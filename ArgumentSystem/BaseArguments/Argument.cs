using SER.ScriptSystem;

namespace SER.ArgumentSystem.BaseArguments;

public abstract class Argument(string name)
{
    public string Name { get; } = name;
    
    public bool ConsumesRemainingValues { get; init; } = false;
    
    public string? Description { get; init; } = null;
    
    public bool IsOptional { get; private set; } = false;

    public object? DefaultValue
    {
        get;
        init
        {
            IsOptional = true;
            field = value;
        }
    }
    
    public abstract string InputDescription { get; }

    public Script Script { get; set; } = null!;
}
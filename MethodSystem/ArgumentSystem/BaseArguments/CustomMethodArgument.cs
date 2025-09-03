namespace SER.MethodSystem.ArgumentSystem.BaseArguments;

public abstract class CustomMethodArgument(string name) : GenericMethodArgument(name)
{
    public override string? AdditionalDescription => null;
    public abstract string InputDescription { get; }
}
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.WarheadMethods;

public class DetonateWarheadMethod : Method
{
    public override string Description => "Detonates the alpha warhead.";
    
    public override BaseMethodArgument[] ExpectedArguments { get; } = [];
    
    public override void Execute()
    {
        Warhead.Detonate();
    }
}
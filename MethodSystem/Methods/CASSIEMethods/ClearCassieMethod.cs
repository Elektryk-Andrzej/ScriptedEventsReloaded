using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.CASSIEMethods;

public class ClearCassieMethod : SynchronousMethod
{
    public override string Description => "Clears all CASSIE announcements, active or queued.";
    
    public override GenericMethodArgument[] ExpectedArguments { get; } = [];
    
    public override void Execute()
    {
        Cassie.Clear();
    }
}
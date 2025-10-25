using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.CASSIEMethods;

public class IsCassieSpeakingMethod : ReturningMethod<BoolValue>
{
    public override string Description => "Returns boolean value indicating if CASSIE is speaking.";
    
    public override Argument[] ExpectedArguments { get; } = [];
    
    public override void Execute()
    {
        ReturnValue = Cassie.IsSpeaking;
    }
}
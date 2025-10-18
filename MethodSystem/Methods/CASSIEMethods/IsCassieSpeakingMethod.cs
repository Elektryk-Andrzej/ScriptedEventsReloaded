using System;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.CASSIEMethods;

public class IsCassieSpeakingMethod : ReturningMethod
{
    public override string Description => "Returns boolean value indicating if CASSIE is speaking.";
    public override Type[]? ReturnTypes => [typeof(TextValue)];
    
    public override Argument[] ExpectedArguments { get; } = [];
    
    public override void Execute()
    {
        Value = new TextValue(Cassie.IsSpeaking.ToString());
    }
}
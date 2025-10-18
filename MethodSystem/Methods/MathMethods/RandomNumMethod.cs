using System;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.ValueSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SER.MethodSystem.Methods.MathMethods;

public class RandomNumMethod : ReturningMethod, IAdditionalDescription
{
    public override string Description =>
        "Returns a randomly generated number.";

    public string AdditionalDescription =>
        "'startingNum' argument MUST be smaller than 'endingNum' argument.";
    
    public override Type[] ReturnTypes => [typeof(NumberValue)];

    public override Argument[] ExpectedArguments { get; } =
    [
        new FloatArgument("startingNum"),
        new FloatArgument("endingNum"),
        new OptionsArgument("numberType", "int", "real")
        {
            DefaultValue = "real"
        }
    ];

    public override void Execute()
    {
        var startingNum = Args.GetFloat("startingNum");
        var endingNum = Args.GetFloat("endingNum");
        var type = Args.GetOption("numberType");
        
        var val = Random.Range(startingNum, endingNum);
        if (type == "int")
        {
            val = Mathf.RoundToInt(val);
        }
        
        Value = new NumberValue((decimal)val);
    }
}
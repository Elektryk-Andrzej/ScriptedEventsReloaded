using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using UnityEngine;

namespace SER.MethodSystem.Methods.MathMethods;

public class RandomNumMethod : TextReturningMethod, IAdditionalDescription, IPureMethod
{
    public override string Description =>
        "Returns a randomly generated number.";

    public string AdditionalDescription =>
        "'startingNum' argument MUST be smaller than 'endingNum' argument.";

    public override GenericMethodArgument[] ExpectedArguments { get; } =
    [
        new FloatArgument("startingNum"),
        new FloatArgument("endingNum"),
        new OptionsArgument("numberType", "integer", "real")
        {
            Description = 
                "'integer' -> numbers like -2, 7, 21 | 'real' -> numbers like -0.5, 420.69, 3.14",
            DefaultValue = "real"
        }
    ];

    public override void Execute()
    {
        var startingNum = Args.GetFloat("startingNum");
        var endingNum = Args.GetFloat("endingNum");
        var type = Args.GetOption("numberType");
        
        var val = Random.Range(startingNum, endingNum);
        if (type == "integer")
        {
            val = Mathf.RoundToInt(val);
        }
        
        TextReturn = val.ToString();
    }
}
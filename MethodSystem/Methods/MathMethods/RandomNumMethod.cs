﻿using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.ValueSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SER.MethodSystem.Methods.MathMethods;

public class RandomNumMethod : ReturningMethod<NumberValue>, IAdditionalDescription
{
    public override string Description =>
        "Returns a randomly generated number.";

    public string AdditionalDescription =>
        "'startingNum' argument MUST be smaller than 'endingNum' argument.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new FloatArgument("startingNum"),
        new FloatArgument("endingNum"),
        new OptionsArgument(
            "numberType", 
            new("int", "Returns an integer number"), 
            new("real", "Returns a real number")
        )
        {
            DefaultValue = "real"
        }
    ];

    public override void Execute()
    {
        Log.D("starting random num is running");
        var startingNum = Args.GetFloat("startingNum");
        var endingNum = Args.GetFloat("endingNum");
        var type = Args.GetOption("numberType");
        
        var val = Random.Range(startingNum, endingNum);
        if (type == "int")
        {
            val = Mathf.RoundToInt(val);
        }
        
        Log.D("random number returns " + val);
        ReturnValue = new NumberValue((decimal)val);
    }
}
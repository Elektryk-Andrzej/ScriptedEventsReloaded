using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.TextMethods;

public class JoinTextMethod : ReturningMethod<TextValue>
{
    public override string Description => "Joins provided text values.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new TextArgument("text to join")
        {
            ConsumesRemainingValues = true
        }
    ];
    
    public override void Execute()
    {
        var texts = Args.GetRemainingArguments<string, TextArgument>("text to join");
        ReturnValue = new TextValue(string.Join("", texts));
    }
}
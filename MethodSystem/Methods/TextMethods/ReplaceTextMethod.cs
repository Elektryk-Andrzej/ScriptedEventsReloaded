using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.MethodSystem.Methods.TextMethods;

public class ReplaceTextMethod : ReturningMethod<TextValue>
{
    public override string Description => "Replaces given values in a given text.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new TextArgument("value to perform the replacement on"),
        new TextArgument("text to replace"),
        new TextArgument("replacement text")
    ];
    
    public override void Execute()
    {
        var value = Args.GetText("value to perform the replacement on");
        var text = Args.GetText("text to replace");
        var replacement = Args.GetText("replacement text");
        
        ReturnValue = new TextValue(value.Replace(text, replacement));
    }
}
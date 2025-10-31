using System;
using System.Linq;
using SER.ContextSystem;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.MethodSystem.BaseMethods;
using SER.ValueSystem;

namespace SER.TokenSystem.Tokens.ExpressionTokens;

public class MethodExpressionToken : ExpressionToken
{
    private ReturningMethod? _method = null!;

    protected override IParseResult InternalParse(BaseToken[] tokens)
    {
        if (tokens.FirstOrDefault() is not MethodToken methodToken)
        {
            return new Ignore();
        }

        if (methodToken.Method is not ReturningMethod method)
        {
            return new Error($"Method '{methodToken.Method.Name}' does not return a value.");
        }
        
        if (Contexter.ContextLine(tokens, null, Script).HasErrored(out var contextError))
        {
            return new Error(contextError);
        }
        
        _method = method;
        return new Success();
    }

    public override TryGet<Value> Value()
    {
        if (_method is null) throw new AndrzejFuckedUpException();
        
        _method.Execute();
        return _method.ReturnValue ?? throw new AndrzejFuckedUpException();
    }

    public override Type[]? PossibleValueTypes => null;
}
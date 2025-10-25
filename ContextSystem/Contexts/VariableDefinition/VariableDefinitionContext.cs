using System;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Structures;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.MethodSystem.BaseMethods;
using SER.TokenSystem.Tokens;
using SER.TokenSystem.Tokens.Interfaces;
using SER.TokenSystem.Tokens.Variables;
using SER.ValueSystem;
using SER.VariableSystem.Bases;
using Log = SER.Helpers.Log;

namespace SER.ContextSystem.Contexts.VariableDefinition;

public abstract class VariableDefinitionContext<TVarToken, TValue, TVariable>(TVarToken varToken) : StandardContext 
    where TVarToken : VariableToken<TVariable, TValue>
    where TValue    : Value
    where TVariable : Variable<TValue>
{
    protected virtual (TryAddTokenRes result, Func<TValue> parser) AdditionalParsing(BaseToken token)
    {
        return (TryAddTokenRes.Error($"Value '{token.RawRep}' ({token.GetType().GetAccurateName()}) cannot be assigned to {typeof(TVarToken).GetAccurateName()} variable"), null!);
    }
    
    protected Func<BaseToken, Func<TValue>?>? AdditionalTokenParser = null;
    
    private bool _equalSignSet = false;
    private MethodContext? _methodContext = null; 
    private Func<TValue>? _parser = null;
    
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (!_equalSignSet)
        {
            if (token is not SymbolToken { RawRep: "=" })
            {
                return TryAddTokenRes.Error(
                    "After a variable, an equals sign is expected to set a value to said variable."
                );
            }
            
            _equalSignSet = true;
            return TryAddTokenRes.Continue();
        }

        if (_methodContext != null)
        {
            return _methodContext.TryAddToken(token);
        }

        var parser = AdditionalTokenParser?.Invoke(token);
        if (parser != null)
        {
            _parser = parser;
            return TryAddTokenRes.End();
        }

        if (token is IValueCapableToken<TValue> valueCapable)
        {
            Log.D("set parser using value capable");
            _parser = () =>
            {
                if (valueCapable.ExactValue.HasErrored(out var error, out var value))
                {
                    throw new ScriptRuntimeError(error);
                }

                return value;
            };
            return TryAddTokenRes.End();
        }

        if (token is MethodToken methodToken)
        {
            if (methodToken.Method is not ReturningMethod)
            {
                return TryAddTokenRes.Error($"Method '{token.RawRep}' does not return a value");
            }
            
            _methodContext = new MethodContext(methodToken)
            {
                Script = Script,
                LineNum = LineNum,
            };
            return TryAddTokenRes.Continue();
        }

        Log.D("set parser using additional");
        var res = AdditionalParsing(token);
        _parser = res.parser;
        return res.result;
    }

    public override Result VerifyCurrentState()
    {
        return Result.Assert(
            _methodContext is not null ||
            _parser is not null,
            $"Value for variable '{varToken.RawRep}' was not provided."
        );
    }

    protected override void Execute()
    {
        if (_methodContext is { Method: ReturningMethod method })
        {
            method.Execute();
            Log.D("checking for returned value");
            if (method.ReturnValue is not { } value)
            {
                throw new AndrzejFuckedUpException($"Method '{method.Name}' did not return a value ({method.ReturnValue}).");
            }

            if (value is not TValue tValue)
            {
                throw new ScriptRuntimeError(
                    $"Value returned by the '{method.Name}' method cannot be assigned to the {varToken.RawRep} variable");
            }
        
            Script.AddVariable(Variable.CreateVariable(varToken.Name, Value.Parse(tValue)));
        }
        else if (_parser is not null)
        {
            Script.AddVariable(Variable.CreateVariable(varToken.Name, Value.Parse(_parser())));
        }
        else
        {
            throw new AndrzejFuckedUpException();
        }
    }
}
using System;
using System.Linq;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Structures;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.MethodSystem.BaseMethods;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens;
using SER.VariableSystem.Variables;
using MethodToken = SER.TokenSystem.Tokens.MethodToken;

namespace SER.ContextSystem.Contexts.VariableDefinition;

public class LiteralVariableDefinitionContext(LiteralVariableToken varToken) : StandardContext
{
    private bool _equalSignSet = false;
    private ReturningMethod? _returningMethod;
    private ReferenceReturningMethod? _referenceReturningMethod;
    private MethodContext? _methodContext;
    //private Func<TryGet<string>>? _contentFunc;
    
    private Func<TryGet<LiteralVariable>>? _variableFunc;

    private TryGet<LiteralVariable> MakeVariableFromVariableToken(LiteralVariableToken varTokenToCopy)
    {
        if (varTokenToCopy.TryGetVariable().HasErrored(out var error, out var variableToCopy))
        {
            return error;
        }

        LiteralVariable newVar;
        //Log.Debug($"the variable to copy is of type {variableToCopy.GetType().Name}");
        if (variableToCopy.GetType().BaseType!.GetGenericTypeDefinition() == typeof(TypeVariable<>))
        {
            //Log.Debug($"Variable {variableToCopy.Name} is a TypeVariable, copying it.");
            var newType = typeof(TypeVariable<>).MakeGenericType(variableToCopy.GetType().BaseType!.GenericTypeArguments.First());
            
            //Log.Debug($"Created new type for variable {variableToCopy.Name}: {newType.Name}<{newType.GenericTypeArguments.First().Name}>");
            var value = variableToCopy.GetType()
                .GetProperty(nameof(TypeVariable<>.ExactValue))!
                .GetValue(variableToCopy);
            
            //Log.Debug($"Variables have value {value} of type {value.GetType().Name}");
            newVar = (LiteralVariable)Activator.CreateInstance(
                newType, 
                varToken.Name, 
                value
            );
            
            //Log.Debug($"Created new variable {newVar.Name} of type {newVar.GetType().Name}");
        }
        else
        {
            //Log.Debug($"Variable {variableToCopy.Name} is not a {nameof(TypeVariable<>)}, copying it as a {nameof(TextVariable)}.");
            newVar = LiteralVariable.CopyVariable(variableToCopy);
        }

        return newVar;
    }

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (!_equalSignSet)
        {
            if (token is not SymbolToken { RawRepresentation: "=" })
                return TryAddTokenRes.Error(
                    "When a line starts with a variable, the only possibility is setting said variable to a value, " +
                    "which requires a '=' sign after said variable, which is missing/malformed. " +
                    "Example: $myVar = \"Hello, World!\"");

            _equalSignSet = true;
            return TryAddTokenRes.Continue();
        }
        
        // emulating method context if set
        if (_methodContext != null) 
            return _methodContext.TryAddToken(token);

        Log.Debug($"'{varToken.Name}' variable is now receiving token '{token.RawRepresentation}' ({token.GetType().Name})");
        switch (token)
        {
            case ILiteralValueToken literalValueToken:
            {
                _variableFunc = () =>
                {
                    if (literalValueToken.GetLiteralValue(Script).HasErrored(out var error, out var value))
                    {
                        return error;
                    }

                    return LiteralVariable.CreateVariable(varToken.Name, value);
                };
                return TryAddTokenRes.End();
            }
            case MethodToken and IContextableToken methodToken:
            {
                if (methodToken.TryGetContext(Script).HasErrored(out var error, out var context))
                {
                    return TryAddTokenRes.Error(error);
                }
            
                _methodContext = (MethodContext)context;

                switch (_methodContext.Method)
                {
                    case ReturningMethod textMethod:
                        _returningMethod = textMethod;
                        break;
                    case ReferenceReturningMethod referenceMethod:
                        _referenceReturningMethod = referenceMethod;
                        break;
                    // case ListReturningMethod listMethod:
                    //    _listReturningMethod = listMethod;
                    //     break;
                    default:
                        return TryAddTokenRes.Error(
                            $"Method {_methodContext.Method.Name} does not return a value, " +
                            "so you cannot use it to define a value of a variable.");
                }
            
                return TryAddTokenRes.Continue();
            }
            default:
                return TryAddTokenRes.Error($"Value '{token.RawRepresentation}' was not expected in this context.");
        }
    }

    public override Result VerifyCurrentState()
    {
        Result rs = $"Variable '{varToken.RawRepresentation}' cannot be created.";
        
        return _variableFunc is not null
               || _returningMethod is not null 
               || _referenceReturningMethod is not null
               //|| _listReturningMethod is not null
            ? true
            : rs + "There is no value to be assigned.";
    }

    protected override void Execute()
    {
        LiteralVariable variable = null!;

        if (_variableFunc != null)
        {
            if (_variableFunc().HasErrored(out var error, out var variableToCopy))
            {
                throw new ScriptErrorException(error);
                return;
            }

            variable = variableToCopy;
        }
        else if (_returningMethod != null)
        {
            _returningMethod.Execute();

            if (_returningMethod.Value == null)
            {
                throw new ScriptErrorException($"Method {_returningMethod.Name} hasn't returned a value, variable" +
                             $" {varToken.RawRepresentation} can't be created.");
                return;
            }

            variable = LiteralVariable.CreateVariable(varToken.Name, _returningMethod.Value);
        }
        else if (_referenceReturningMethod != null)
        {
            _referenceReturningMethod.Execute();

            if (_referenceReturningMethod.Reference == null)
            {
                throw new ScriptErrorException($"Method {_referenceReturningMethod.Name} hasn't returned a value, variable " +
                             $"{varToken.RawRepresentation} can't be created.");
                return;
            }

            variable = new ReferenceVariable(
                varToken.Name,
                _referenceReturningMethod.Reference
            );
        }
        /*else if (_contentFunc != null)
        {
            if (_contentFunc().HasErrored(out var error, out var result))
            {
                Log.Error(Script, error);
                return;
            }

            variable = new TextVariable(varToken.Name, () => result);
        }
        else if (_listReturningMethod != null)
        {
            _listReturningMethod.Execute();
            
            if (_listReturningMethod.ListReturn == null)
            {
                Log.Error(Script, $"Method {_listReturningMethod.Name} hasn't returned a value, variable {varToken.RawRepresentation} can't be created.");
                return;
            }
            
            _variable = new LiteralListVariable
            {
                Name = varToken.ValueWithoutBrackets,
                Value = () => null!,
                List = _listReturningMethod.ListReturn
            };
        }*/
        
        if (variable is null)
        {
            throw new AndrzejFuckedUpException($"Tried to execute {GetType().Name} without a variable set.");
        }
        
        Script.AddLocalLiteralVariable(variable);
    }
}
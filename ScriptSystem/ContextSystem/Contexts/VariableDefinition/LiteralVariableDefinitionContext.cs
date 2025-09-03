﻿using System.Linq;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;
using SER.VariableSystem;
using SER.VariableSystem.Structures;

namespace SER.ScriptSystem.ContextSystem.Contexts.VariableDefinition;

public class LiteralVariableDefinitionContext(LiteralVariableToken varToken) : StandardContext
{
    private bool _hasEqualsSignBeenVerified = false;
    //private ListReturningMethod? _listReturningMethod;
    private TextReturningMethod? _textReturningMethod;
    private ReferenceReturningMethod? _referenceReturningMethod;
    private MethodContext? _methodContext;
    private LiteralVariable? _variable;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        // emulating method context
        if (_methodContext != null) return _methodContext.TryAddToken(token);

        if (!_hasEqualsSignBeenVerified)
        {
            if (token.RawRepresentation != "=")
                return TryAddTokenRes.Error(
                    "When a line starts with a variable, the only possibility is setting said variable to a value, " +
                    "which requires a `=` sign after said variable, which is missing/malformed. " +
                    "Example: {test} = (Hello, World!)");

            _hasEqualsSignBeenVerified = true;
            return TryAddTokenRes.Continue();
        }

        if (token is not MethodToken methodToken)
        {
            var value = VariableParser.ReplaceVariablesInContaminatedString(token.GetValue(), Script);
            _variable = new()
            {
                Name = varToken.ValueWithoutBrackets,
                Value = () => value
            };

            return TryAddTokenRes.End();
        }
        
        if (methodToken.TryGetResultingContext().HasErrored(out var err, out var context))
            return TryAddTokenRes.Error(err);

        _methodContext = (MethodContext)context;

        switch (_methodContext.Method)
        {
            case TextReturningMethod textMethod:
                _textReturningMethod = textMethod;
                break;
            case ReferenceReturningMethod referenceMethod:
                _referenceReturningMethod = referenceMethod;
                break;
            // case ListReturningMethod listMethod:
            //    _listReturningMethod = listMethod;
            //     break;
            default:
                return TryAddTokenRes.Error(
                    $"Method {methodToken.Method.Name} does not return a value, " +
                    "so you cannot use it to define a value of a variable.");
        }
            
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        var rs = new ResultStacker($"Variable '{varToken.RawRepresentation}' cannot be created.");

        if (varToken.ValueWithoutBrackets.FirstOrDefault() is not '*' and not '&')
        {
            if (_referenceReturningMethod is not null)
            {
                return rs.Add(
                    "When an action returns an object reference, " +
                    "'*' must be used before the name e.g. {*myReference}");
            }

            // if (_listReturningMethod is not null)
            // {
            //     return rs.Add(
            //         "When an action returns a list, " +
            //         "'*' must be used before the name e.g. {&myList}");
            // }
            
            if (varToken.ValueWithoutBrackets.Length == 0)
            {
                return rs.Add("Variable must have a name.");
            }
            
            if (varToken.ValueWithoutBrackets.Any(c => !char.IsLetter(c)))
                return rs.Add("Variable name can only contain letters.");
        }
        else
        {
            if (varToken.ValueWithoutBrackets.Length < 2)
            {
                return rs.Add("Variable must have a name, the prefix doesn't count as name.");
            }
            
            if (varToken.ValueWithoutBrackets.Skip(1).Any(c => !char.IsLetter(c)))
                return rs.Add("Variable name can only contain letters.");
        }

        if (char.IsUpper(varToken.ValueWithoutBrackets.First()))
            return rs.Add("The first character in the name must be lowercase.");
        
        return _variable is not null 
               || _textReturningMethod is not null 
               || _referenceReturningMethod is not null
               //|| _listReturningMethod is not null
            ? true
            : rs.Add("There is no value to be assigned.");
    }

    protected override void Execute()
    {
        if (_textReturningMethod != null)
        {
            _textReturningMethod.Execute();

            if (_textReturningMethod.TextReturn == null)
            {
                Log.Error(Script, $"Method {_textReturningMethod.Name} hasn't returned a value, variable {varToken.RawRepresentation} can't be created.");
                return;
            }
            
            _variable = new()
            {
                Name = varToken.ValueWithoutBrackets,
                Value = () => _textReturningMethod.TextReturn
            };
        }
        else if (_referenceReturningMethod != null)
        {
            _referenceReturningMethod.Execute();

            if (_referenceReturningMethod.ValueReturn == null)
            {
                Log.Error(Script, $"Method {_referenceReturningMethod.Name} hasn't returned a value, variable {varToken.RawRepresentation} can't be created.");
                return;
            }

            _variable = new ReferenceVariable
            {
                Name = varToken.ValueWithoutBrackets,
                Value = () => ObjectReferenceSystem.RegisterObject(_referenceReturningMethod.ValueReturn),
                Type = _referenceReturningMethod.ValueReturn.GetType()
            };
        }
        /*else if (_listReturningMethod != null)
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
        
        if (_variable is null)
        {
            throw new AndrzejFuckedUpException($"Tried to execute {GetType().Name} without a variable set.");
        }
        
        Script.AddLocalLiteralVariable(_variable);
    }
}
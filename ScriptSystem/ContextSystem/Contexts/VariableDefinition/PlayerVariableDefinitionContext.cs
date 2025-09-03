using System.Linq;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.VariableDefinition;

public class PlayerVariableDefinitionContext(PlayerVariableToken varToken) : StandardContext
{
    private bool _hasEqualsSignBeenVerified = false;
    private PlayerReturningMethod? _method;
    private MethodContext? _methodContext;
    private PlayerVariableToken? _playerVariableToken;
    private bool _shouldBeEmpty;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        // emulating method context
        if (_methodContext != null) return _methodContext.TryAddToken(token);

        if (!_hasEqualsSignBeenVerified)
        {
            if (token.RawRepresentation != "=")
            {
                return TryAddTokenRes.Error(
                    "When a line starts with a variable, the only possibility is setting said variable to a value, " +
                    "which requires a `=` sign after said variable, which is missing/malformed.");
            }

            _hasEqualsSignBeenVerified = true;
            return TryAddTokenRes.Continue();
        }

        switch (token)
        {
            case MethodToken methodToken:
            {
                if (methodToken.TryGetResultingContext().HasErrored(out var err, out var context))
                    return TryAddTokenRes.Error(err);

                _methodContext = (MethodContext)context;

                if (_methodContext.Method is not PlayerReturningMethod playerMethod)
                    return TryAddTokenRes.Error(
                        $"Method {methodToken.Method.Name} does not return a value, " +
                        "so you cannot use it to define a value of a variable.");

                _method = playerMethod;
                return TryAddTokenRes.Continue();
            }
            case ParenthesesToken parenthesesToken:
            {
                if (parenthesesToken.GetValue().Length > 0)
                {
                    return TryAddTokenRes.Error("Parentheses must be empty in order to make an empty player var.");
                }
                _shouldBeEmpty = true;
                return TryAddTokenRes.End();
            }
            case PlayerVariableToken playerVariableToken:
            {
                _playerVariableToken = playerVariableToken;
                return TryAddTokenRes.End();
            }
            default:
            {
                return TryAddTokenRes.Error(
                    $"Value {token.RawRepresentation} is not a method or a player variable, so it can't represent a " +
                    "valid player value.");
            }
                
        }
    }

    public override Result VerifyCurrentState()
    {
        var rs = new ResultStacker($"Variable '{varToken.RawRepresentation}' cannot be created.");

        if (varToken.NameWithoutPrefix.Any(c => !char.IsLetterOrDigit(c)))
            return rs.Add("Variable name can only contain letters and numbers.");

        if (char.IsUpper(varToken.NameWithoutPrefix.First()))
            return rs.Add("The first character in the name must be lowercase.");

        return Result.Assert(_playerVariableToken is not null || _method is not null || _shouldBeEmpty,
            rs.Add("There is no value to be assigned."));
    }

    protected override void Execute()
    {
        if (_shouldBeEmpty)
        {
            Script.AddLocalPlayerVariable(new(varToken.NameWithoutPrefix, []));
            return;
        }
        
        if (_method != null)
        {
            _method.Execute();

            if (_method.PlayerReturn == null)
            {
                throw new AndrzejFuckedUpException($"Method {_method.Name} hasnt returned a value.");
            }

            Script.AddLocalPlayerVariable(
                new(varToken.NameWithoutPrefix, _method.PlayerReturn.ToList()));
            return;
        }

        if (_playerVariableToken == null)
            throw new AndrzejFuckedUpException("Method and var token are both null, this should never happen.");
        
        if (!Script.TryGetPlayerVariable(_playerVariableToken.NameWithoutPrefix).WasSuccessful(out var variable))
        {
            throw new InvalidVariableException(_playerVariableToken);
        }
            
        Script.AddLocalPlayerVariable(new(varToken.NameWithoutPrefix, variable.Players.ToList()));
    }
}
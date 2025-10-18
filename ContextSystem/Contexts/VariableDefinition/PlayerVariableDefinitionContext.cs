using System.Linq;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Structures;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.MethodSystem.BaseMethods;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens;
using MethodToken = SER.TokenSystem.Tokens.MethodToken;

namespace SER.ContextSystem.Contexts.VariableDefinition;

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
            case MethodToken methodToken and IContextableToken contextable:
            {
                if (contextable.TryGetContext(Script).HasErrored(out var err, out var context))
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
                if (parenthesesToken.TryGetTokens().HasErrored(out var err, out var tokens))
                {
                    return TryAddTokenRes.Error(err);
                }
                
                if (tokens.Length > 0)
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
        Result rs = $"Variable '{varToken.RawRepresentation}' cannot be created.";

        if (varToken.Name.Any(c => !char.IsLetterOrDigit(c)))
            return rs + "Variable name can only contain letters and numbers.";

        if (char.IsUpper(varToken.Name.First()))
            return rs + "The first character in the name must be lowercase.";

        return Result.Assert(_playerVariableToken is not null || _method is not null || _shouldBeEmpty,
            rs + "There is no value to be assigned.");
    }

    protected override void Execute()
    {
        if (_shouldBeEmpty)
        {
            Script.AddLocalPlayerVariable(new(varToken.Name, []));
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
                new(varToken.Name, _method.PlayerReturn.ToList()));
            return;
        }

        if (_playerVariableToken == null)
            throw new AndrzejFuckedUpException("Method and var token are both null, this should never happen.");
        
        if (!Script.TryGetPlayerVariable(_playerVariableToken.Name).HasErrored(out var error, out var variable))
        {
            throw new ScriptErrorException(error);
        }
            
        Script.AddLocalPlayerVariable(new(varToken.Name, variable.Players.ToList()));
    }
}
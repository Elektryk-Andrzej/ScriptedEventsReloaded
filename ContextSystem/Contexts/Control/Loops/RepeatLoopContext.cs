using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SER.ContextSystem.BaseContexts;
using SER.ContextSystem.Extensions;
using SER.ContextSystem.Structures;
using SER.Helpers.Exceptions;
using SER.Helpers.ResultSystem;
using SER.TokenSystem.Structures;
using SER.TokenSystem.Tokens;
using SER.ValueSystem;

namespace SER.ContextSystem.Contexts.Control.Loops;

[UsedImplicitly]
public class RepeatLoopContext : StatementContext, IKeywordContext
{
    private readonly Result _rs = "Cannot create 'repeat' loop.";
    private Func<TryGet<uint>>? _repeatCountExpression = null;
    private uint? _repeatCount = null;
    private bool _breakChild = false;
    
    public string KeywordName => "repeat";
    public string Description => "Repeats everything inside its body a given amount of times.";
    public string[] Arguments => ["[number]"];

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        switch (token)
        {
            case NumberToken numberToken:
                if (numberToken.Value < 0)
                {
                    return TryAddTokenRes.Error(
                        $"Value '{numberToken.Value}' cannot be negative.");
                }
                
                _repeatCount = (uint)numberToken.Value;
                return TryAddTokenRes.End();
            case ILiteralValueToken literalValueToken:
                _repeatCountExpression = () =>
                {
                    if (literalValueToken.GetLiteralValue(Script).HasErrored(out var error, out var value))
                    {
                        return error;
                    }

                    if (value is not NumberValue numberValue)
                    {
                        return $"Value '{value}' retreived from {token.RawRepresentation} is not a number.";
                    }

                    if (numberValue.Value < 0)
                    {
                        return $"Value '{value}' cannot be negative.";
                    }

                    return (uint)numberValue.Value;
                };
                return TryAddTokenRes.End();
        }

        return TryAddTokenRes.Error($"Value '{token.RawRepresentation}' cannot be interpreted as a number.");
    }

    public override Result VerifyCurrentState()
    {
        return Result.Assert(
            _repeatCountExpression != null || _repeatCount.HasValue,
            _rs + "The amount of times to repeat was not provided.");
    }

    protected override IEnumerator<float> Execute()
    {
        if (!_repeatCount.HasValue)
        {
            if (_repeatCountExpression == null) 
                throw new AndrzejFuckedUpException("Repeat context has no amount specified");

            if (_repeatCountExpression().HasErrored(out var error, out var val))
            {
                throw new ScriptErrorException(error);
            }
            
            _repeatCount = val;
        }

        for (var i = 0; i < _repeatCount.Value; i++)
        {
            foreach (var child in Children)
            {
                var coro = child.ExecuteBaseContext();
                while (coro.MoveNext())
                {
                    yield return coro.Current;
                }

                if (!_breakChild) continue;

                _breakChild = false;
                break;
            }
        }
    }

    protected override void OnReceivedControlMessageFromChild(ParentContextControlMessage msg)
    {
        if (msg == ParentContextControlMessage.LoopContinue)
        {
            _breakChild = true;
            return;
        }

        ParentContext?.SendControlMessage(msg);
    }
}
﻿using System;
using System.Collections.Generic;
using SER.Helpers.Exceptions;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.ScriptSystem.TokenSystem;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.VariableSystem;

namespace SER.ScriptSystem.ContextSystem.Contexts.Loops;

public class RepeatLoopContext : TreeContext
{
    private readonly ResultStacker _rs = new("Cannot create `repeat` loop.");
    private Func<string>? _getStringVal = null;
    private int? _repeatCount = null;
    private bool _skipChild = false;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (VariableParser.IsValueSyntaxUsedInString(token.GetValue(), Script, out var resultFunc))
        {
            _getStringVal = resultFunc;
            return TryAddTokenRes.End();
        }

        if (!int.TryParse(token.GetValue(), out var resultInt))
            return TryAddTokenRes.Error(_rs.Add($"Value '{token.GetValue()}' is not a valid integer."));

        _repeatCount = resultInt;
        return TryAddTokenRes.End();
    }

    public override Result VerifyCurrentState()
    {
        return Result.Assert(
            _getStringVal != null || _repeatCount.HasValue,
            _rs.Add("The amount of times to repeat was not provided."));
    }

    public override IEnumerator<float> Execute()
    {
        if (!_repeatCount.HasValue)
        {
            if (_getStringVal == null) throw new AndrzejFuckedUpException("Repeat context has no amount specified");

            var val = _getStringVal();
            if (!int.TryParse(val, out var resultInt)) throw new InvalidValueException("integer number", val);

            _repeatCount = resultInt;
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

                if (!_skipChild) continue;

                _skipChild = false;
                break;
            }
        }
    }

    protected override void OnReceivedControlMessageFromChild(ParentContextControlMessage msg)
    {
        if (msg == ParentContextControlMessage.LoopContinue)
        {
            _skipChild = true;
            return;
        }

        ParentContext?.SendControlMessage(msg);
    }
}
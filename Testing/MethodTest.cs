using System;
using JetBrains.Annotations;
using SER.TokenSystem.Tokens;

namespace SER.Testing;

[UsedImplicitly]
public class MethodTest : Test
{
    protected override string Content =>
        """
        Eval (1 + {var} > 5)
        Wait 5s
        Broadcast * 1m "Sup everyone"
        GlobalPlayerVariable @myVar @players
        """;

    protected override Type[][] TargetTokens => [
        [typeof(MethodToken), typeof(ParenthesesToken)],
        [typeof(MethodToken), typeof(DurationToken)],
        [typeof(MethodToken), typeof(SymbolToken), typeof(DurationToken), typeof(TextToken)],
        [typeof(MethodToken), typeof(PlayerVariableToken), typeof(PlayerVariableToken)]
    ];
}
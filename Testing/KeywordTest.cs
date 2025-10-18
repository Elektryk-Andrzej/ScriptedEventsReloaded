using System;
using JetBrains.Annotations;
using SER.TokenSystem.Tokens;

namespace SER.Testing;

[UsedImplicitly]
public class KeywordTest : Test
{
    protected override string Content =>
        """
        if 5 > 3
            Print "Awesome"
        elif
        else
        end
        repeat 10
        stop
        while true
        
        forever
            # nothing
        end
        """;

    protected override Type[][] TargetTokens =>
    [
        [typeof(KeywordToken), typeof(NumberToken), typeof(SymbolToken), typeof(NumberToken)],
        [typeof(MethodToken), typeof(TextToken)],
        [typeof(KeywordToken)],
        [typeof(KeywordToken)],
        [typeof(KeywordToken)],
        [typeof(KeywordToken), typeof(NumberToken)],
        [typeof(KeywordToken)],
        [typeof(KeywordToken), typeof(BaseToken)],
        [],
        [typeof(KeywordToken)],
        [typeof(CommentToken), typeof(BaseToken)],
        [typeof(KeywordToken)]
    ];
}
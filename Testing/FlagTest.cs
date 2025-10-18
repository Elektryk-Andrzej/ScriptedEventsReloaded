using System;
using JetBrains.Annotations;
using SER.TokenSystem.Tokens;

namespace SER.Testing;

[UsedImplicitly]
public class FlagTest : Test
{
    protected override string Content =>
        """
        !-- CustomCommand testCommand
        -- availableFor Player Server RemoteAdmin
        -- description "This is a test command"
        """;

    protected override Type[][] TargetTokens => [
        [typeof(FlagToken), typeof(BaseToken), typeof(BaseToken)],
        [typeof(FlagArgumentToken), typeof(BaseToken), typeof(BaseToken), typeof(BaseToken), typeof(BaseToken)],
        [typeof(FlagArgumentToken), typeof(BaseToken), typeof(TextToken)]
    ];
}
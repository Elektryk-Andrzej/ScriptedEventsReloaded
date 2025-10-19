using JetBrains.Annotations;

namespace SER.ExampleScripts.Scripts;

[UsedImplicitly]
public class InviteInfoScript : ExampleScript
{
    public override string Name => "InviteBroadcast";
    public override string Content =>
        """
        # this script is connected to the 'Joined' event, which means that this script will run when a player joins
        # this event provides us with the @evPlayer variable with the player who just joined
        !-- OnEvent Joined
        
        # this 'Broadcast' method sends a formatted message to the player who just joined
        Broadcast @evPlayer 10s "<b><color=#8dfcef><size=35>Welcome {@evPlayer nickname}!</size></color>\n<color=#ebebb9><size=20>This server is using <u>Scripted Events Reloaded</u>, enjoy your stay!</size></color></b>"
        """;
}
﻿namespace SER.Examples;

public class WelcomeScript : IExample
{
    public string Name => "welcome";

    public string Content =>
        """
        # this script is connected to the 'Joined' event, which means that this script will run when a player joins
        # this event provides us with the @evPlayer variable with the player who just joined
        !-- OnEvent Joined
        
        # this 'Broadcast' method sends a formatted message to the player who just joined
        Broadcast @evPlayer 10s "<b><color=#8dfcef><size=35>Welcome {@evPlayer name}!</size></color>\n<color=#ebebb9><size=20>This server is using <u>Scripted Events Reloaded</u>, enjoy your stay!</size></color></b>"
        
        # information for the server owner to visit the scripts folder
        Hint @evPlayer 10s "<line-height=1000%><br></line-height><b><size=30>This is the default message.<br>You can modify it - as well as any other example script - by going to the SER folder in plugin configs."
        """;
}
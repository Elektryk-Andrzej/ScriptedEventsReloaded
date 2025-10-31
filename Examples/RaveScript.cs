namespace SER.Examples;

public class RaveScript : IExample
{
    public string Name => "rave";

    public string Content =>
        """
        # confirm that @sender variable was created
        if {VarExists @sender} == false
            Reply "you need to run this script through remote admin!"
            stop
        end
        
        # the values for the rave
        $duration = .5s
        *room = {@sender roomRef}
        
        # changing colors for the room
        repeat 20
            TransitionLightColor *room #ff0000ff $duration
            Wait $duration
            
            TransitionLightColor *room #00ff00ff $duration
            Wait $duration
            
            TransitionLightColor *room #0000ffff $duration
            Wait $duration
        end
        
        # reset color to default
        Wait 1ms
        ResetLightColor *room
        """;
}
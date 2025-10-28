using LabApi.Features.Wrappers;
using LightContainmentZoneDecontamination;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Exceptions;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.MapMethods;

public class DecontaminationMethod : SynchronousMethod
{
    public override string Description => "Controls the LCZ decontamination.";

    public override Argument[] ExpectedArguments { get; } =
    [
        new OptionsArgument("mode",
            "enable",
            "disable",
            "force"
        )
    ];
    
    public override void Execute()
    {
        Decontamination.Status = Args.GetOption("mode") switch
        {
            "enable" => DecontaminationController.DecontaminationStatus.None,
            "disable" => DecontaminationController.DecontaminationStatus.Disabled,
            "force" => DecontaminationController.DecontaminationStatus.Forced,
            _ => throw new AndrzejFuckedUpException()
        };
    }
}
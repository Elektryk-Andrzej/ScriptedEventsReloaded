using LabApi.Features.Wrappers;
using SER.ArgumentSystem.BaseArguments;
using SER.Helpers.Extensions;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.MapMethods;

public class CleanupPickupsMethod : SynchronousMethod
{
    public override string? Description => "Cleans pickups (items) from the map.";

    public override Argument[] ExpectedArguments { get; } = [];
    
    public override void Execute()
    {
        Map.Pickups.ForEachItem(p => p.Destroy());
    }
}
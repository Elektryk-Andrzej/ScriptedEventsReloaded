using System.Linq;
using Interactables.Interobjects;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class ElevatorsArgument(string name) : GenericMethodArgument(name)
{
    public override string? AdditionalDescription => null;
    
    [UsedImplicitly]
    public ArgumentEvaluation<Elevator[]> GetConvertSolution(BaseToken token)
    {
        return MultipleSolutionConvert<Elevator[]>(token, new()
        {
            [OperatingValue.ElevatorGroup] = group => Elevator.GetByGroup((ElevatorGroup)group).ToArray(),
            [OperatingValue.AllOfType] = _ => Elevator.List.ToArray(),
        });
    }
}
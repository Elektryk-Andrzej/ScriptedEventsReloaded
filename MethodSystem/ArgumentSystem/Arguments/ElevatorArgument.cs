using System.Linq;
using Interactables.Interobjects;
using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class ElevatorsArgument(string name) : BaseMethodArgument(name)
{
    public override OperatingValue Input => OperatingValue.ElevatorGroup | OperatingValue.AllOfType;
    public override string? AdditionalDescription => null;
    
    public ArgumentEvaluation<Elevator[]> GetConvertSolution(BaseToken token)
    {
        return DefaultConvertSolution<Elevator[]>(token, new()
        {
            [OperatingValue.ElevatorGroup] = group => Elevator.GetByGroup((ElevatorGroup)group).ToArray(),
            [OperatingValue.AllOfType] = _ => Elevator.List.ToArray(),
        });
    }
}
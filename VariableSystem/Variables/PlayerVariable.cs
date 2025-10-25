using LabApi.Features.Wrappers;
using SER.ValueSystem;
using SER.VariableSystem.Bases;

namespace SER.VariableSystem.Variables;

public class PlayerVariable(string name, PlayerValue value) : Variable<PlayerValue>
{
    public override string Name => name;
    public override PlayerValue Value => value;
    public Player[] Players => Value.Players;
}
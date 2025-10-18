using System.Collections.Generic;
using LabApi.Features.Wrappers;

namespace SER.VariableSystem.Variables;

public class PlayerVariable(string name, List<Player> players) : IVariable
{
    public virtual string Name => name;
    public virtual List<Player> Players => players;
}
using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;

namespace SER.VariableSystem.Variables;

public class PredefinedPlayerVariable(string name, Func<List<Player>> value, string category) 
    : PlayerVariable(name, null!)
{
    public override List<Player> Players => value();
    public string Category => category;
}
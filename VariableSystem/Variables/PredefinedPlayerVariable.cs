using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using SER.ValueSystem;

namespace SER.VariableSystem.Variables;

public class PredefinedPlayerVariable(string name, Func<List<Player>> value, string category) 
    : PlayerVariable(name, null!)
{
    public override PlayerValue Value => new(value());
    public string Category => category;
}
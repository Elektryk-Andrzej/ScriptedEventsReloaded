using System;
using SER.Helpers.ResultSystem;

namespace SER.ArgumentSystem.Structures;

public struct ArgumentValueInfo()
{
    public required string Name { get; init; }
    public required Type ArgumentType { get; init; }
    public required DynamicTryGet Evaluator { get; init; }
    public bool IsPartOfCollection { get; init; } = false;
}
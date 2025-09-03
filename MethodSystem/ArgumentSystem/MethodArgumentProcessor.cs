﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.ArgumentSystem.Interfaces;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem;

public class MethodArgumentProcessor(Method method)
{
    private class ConverterInfo(MethodInfo method)
    {
        private MethodInfo Method { get; } = method;
        
        public IArgEvalRes Invoke(BaseToken token, GenericMethodArgument arg)
        {
            object[] parameters = [token];
            
            return (IArgEvalRes)Method.Invoke(arg, parameters);
        }
    }
    
    private static readonly Dictionary<Type, ConverterInfo> ConverterCache = new();
    
    private static ConverterInfo GetConverterInfo(Type argType)
    {
        if (ConverterCache.TryGetValue(argType, out var cachedInfo))
        {
            return cachedInfo;
        }
        
        var instanceMethod = argType.GetMethod("GetConvertSolution", 
            BindingFlags.Public | BindingFlags.Instance,
            null, [typeof(BaseToken)], null);
            
        if (instanceMethod != null)
        {
            return ConverterCache[argType] = new ConverterInfo(instanceMethod);
        }
        
        throw new AndrzejFuckedUpException($"No suitable GetConvertSolution method found for {argType.GetAccurateName()}.");
    }

    public TryGet<ArgumentSkeleton> TryGetSkeleton(BaseToken token, int index)
    {
        var rs = new ResultStacker(
            $"Argument {index + 1} '{token.RawRepresentation}' for method {method.Name} is invalid.");

        GenericMethodArgument arg;
        if (index >= method.ExpectedArguments.Length)
        {
            if (method.ExpectedArguments.LastOrDefault()?.ConsumesRemainingValues != true)
            {
                return rs.Add(
                    $"Method does not expect more than {method.ExpectedArguments.Length} arguments.");
            }
            
            arg = method.ExpectedArguments.Last();
        }
        else
        {
            arg = method.ExpectedArguments[index];
        }
        
        arg.Script = method.Script;
        var argType = arg.GetType();
        
        var evaluator = GetConverterInfo(argType).Invoke(token, arg);
        if (!evaluator.IsStatic)
        {
            return new ArgumentSkeleton
            {
                Evaluator = evaluator,
                ArgumentType = argType,
                Name = arg.Name,
                IsPartOfCollection = arg.ConsumesRemainingValues
            };
        }
        
        if (evaluator.GetResult().HasErrored(out var error))
        {
            return rs.Add(error);
        }

        return new ArgumentSkeleton
        {
            Evaluator = evaluator,
            ArgumentType = argType,
            Name = arg.Name,
            IsPartOfCollection = arg.ConsumesRemainingValues
        };
    }
}
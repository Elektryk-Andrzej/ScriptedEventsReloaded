using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SER.ArgumentSystem.BaseArguments;
using SER.ArgumentSystem.Structures;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.MethodSystem.BaseMethods;
using SER.TokenSystem.Tokens;

namespace SER.ArgumentSystem;

public class MethodArgumentDispatcher(Method method)
{
    private class ConverterInfo(MethodInfo method)
    {
        private MethodInfo Method { get; } = method;
        
        public DynamicTryGet Invoke(BaseToken token, Argument arg)
        {
            try
            {
                return (DynamicTryGet)Method.Invoke(arg, [token]);
            }
            catch (TargetInvocationException ex)
            {
                return DynamicTryGet.Error($"This error is not an expected one, report it to the developers. {ex.InnerException?.Message} {ex.InnerException?.StackTrace}");
            }
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

    public TryGet<ArgumentValueInfo> TryGetValueInfo(BaseToken token, int index)
    {
        Result rs = $"Argument {index + 1} '{token.RawRepresentation}' for method {method.Name} is invalid.";

        Argument arg;
        if (index >= method.ExpectedArguments.Length)
        {
            if (method.ExpectedArguments.LastOrDefault()?.ConsumesRemainingValues != true)
            {
                return rs + $"Method does not expect more than {method.ExpectedArguments.Length} arguments.";
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
            return new ArgumentValueInfo
            {
                Evaluator = evaluator,
                ArgumentType = argType,
                Name = arg.Name,
                IsPartOfCollection = arg.ConsumesRemainingValues
            };
        }
        
        if (evaluator.Result.HasErrored(out var error))
        {
            Log.D($"error from evaluator: {error}");
            Log.D($"default error: {rs}");
            return rs + error;
        }

        return new ArgumentValueInfo
        {
            Evaluator = evaluator,
            ArgumentType = argType,
            Name = arg.Name,
            IsPartOfCollection = arg.ConsumesRemainingValues
        };
    }
}
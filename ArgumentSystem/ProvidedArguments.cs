using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using SER.ArgumentSystem.Arguments;
using SER.ArgumentSystem.Structures;
using SER.Helpers;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.Helpers.ResultSystem;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem;
using SER.TokenSystem.Tokens;
using SER.VariableSystem.Variables;
using UnityEngine;

namespace SER.ArgumentSystem;

public class ProvidedArguments(Method method)
{
    private Dictionary<(string name, Type type), List<DynamicTryGet>> Arguments { get; } = [];
    
    public Room GetRoom(string argName)
    {
        return GetValue<Room, RoomArgument>(argName);
    }
    
    public Elevator[] GetElevators(string argName)
    {
        return GetValue<Elevator[], ElevatorsArgument>(argName);
    }
    
    public LiteralVariableToken GetLiteralVariable(string argName)
    {
        return GetValue<LiteralVariableToken, LiteralVariableArgument>(argName);
    }
    
    /// <summary>
    /// Retrieves an array of <see cref="Item"/> instances associated with the specified argument name.
    /// </summary>
    /// <param name="argName">The name of the argument containing the item value.</param>
    /// <returns>An array of <see cref="Item"/> instances that represents the retrieved items.</returns>
    public Item[] GetItems(string argName)
    {
        return GetValue<Item[], ItemsArgument>(argName);
    }
    
    /// <summary>
    /// Retrieves a <see cref="PlayerVariableToken"/> instance associated with the specified argument name.
    /// </summary>
    /// <param name="argName">The name of the argument containing the player variable value.</param>
    /// <returns>An instance of <see cref="PlayerVariableToken"/> that represents the retrieved player variable.</returns>
    public PlayerVariableToken GetPlayerVariableName(string argName)
    {
        return GetValue<PlayerVariableToken, PlayerVariableNameArgument>(argName);
    }
    
    /// <summary>
    /// Retrieves the <see cref="IVariable"/> instance associated with the specified argument name.
    /// </summary>
    /// <param name="argName">The name of the argument containing the variable value.</param>
    /// <returns>An instance of <see cref="IVariable"/> representing the retrieved variable.</returns>
    public IVariable GetVariable(string argName)
    {
        return GetValue<IVariable, VariableArgument>(argName);
    }

    /// <summary>
    /// Retrieves the <see cref="Script"/> instance associated with the specified argument name.
    /// </summary>
    /// <param name="argName">The name of the argument containing the script value.</param>
    /// <returns>An instance of <see cref="Script"/> representing the retrieved script.</returns>
    public Script GetScript(string argName)
    {
        return GetValue<Script, ScriptArgument>(argName);
    }

    /// <summary>
    /// Retrieves the <see cref="Color"/> value associated with the specified argument name.
    /// </summary>
    /// <param name="argName">The name of the argument containing the color value.</param>
    /// <returns>A <see cref="Color"/> object representing the retrieved color value.</returns>
    public Color GetColor(string argName)
    {
        return GetValue<Color, ColorArgument>(argName);
    }

    /// <summary>
    /// Retrieves an array of rooms associated with the given argument name.
    /// </summary>
    /// <param name="argName">The name of the argument containing the room array.</param>
    /// <returns>An array of <see cref="Room"/> objects representing the retrieved rooms.</returns>
    public Room[] GetRooms(string argName)
    {
        return GetValue<Room[], RoomsArgument>(argName);
    }

    /// <summary>
    /// Retrieves a boolean value associated with the given argument name.
    /// </summary>
    /// <param name="argName">The name of the argument containing the boolean value.</param>
    /// <returns>A <see cref="bool"/> representing the retrieved boolean value.</returns>
    public bool GetBool(string argName)
    {
        return GetValue<bool, BoolArgument>(argName);
    }

    /// <summary>
    /// Retrieves a function delegate that evaluates the boolean value associated with the specified argument name.
    /// </summary>
    /// <param name="argName">The name of the argument containing the boolean value to evaluate.</param>
    /// <returns>A <see cref="Func{T}"/> delegate that evaluates and returns the boolean value of the argument.</returns>
    public Func<bool> GetBoolFunc(string argName)
    {
        var evaluator = GetEvaluators<bool, BoolArgument>(argName).First();
        return () => evaluator.Invoke().Value;
    }

    /// <summary>
    /// Retrieves a reference to an object of the specified type associated with the given argument name.
    /// </summary>
    /// <typeparam name="T">The type of the reference to retrieve.</typeparam>
    /// <param name="argName">The name of the argument containing the reference.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the requested reference.</returns>
    public T GetReference<T>(string argName)
    {
        return GetValue<T, ReferenceArgument<T>>(argName);
    }

    /// <summary>
    /// Retrieves a collection of doors associated with the specified argument name.
    /// </summary>
    /// <param name="argName">The name of the argument from which to retrieve the door collection.</param>
    /// <returns>An array of <see cref="Door"/> objects matching the specified argument.</returns>
    public Door[] GetDoors(string argName)
    {
        return GetValue<Door[], DoorsArgument>(argName);
    }
    
    public Door GetDoor(string argName)
    {
        return GetValue<Door, DoorArgument>(argName);
    }

    /// <summary>
    /// Retrieves a duration value associated with the specified argument name.
    /// </summary>
    /// <param name="argName">The name of the argument from which to retrieve the duration value.</param>
    /// <returns>The duration value as a <see cref="TimeSpan"/> object.</returns>
    public TimeSpan GetDuration(string argName)
    {
        return GetValue<TimeSpan, DurationArgument>(argName);
    }

    /// <summary>
    /// Retrieves a processed text value associated with the specified argument name,
    /// with variables replaced based on the current script context.
    /// </summary>
    /// <param name="argName">The name of the argument to retrieve the processed text value from.</param>
    /// <returns>The processed text value with variables replaced.</returns>
    public string GetText(string argName)
    {
        return TextToken.ParseValue(GetValue<string, TextArgument>(argName), method.Script);
    }

    /// <summary>
    /// Retrieves the integer amount associated with the specified argument name.
    /// </summary>
    /// <param name="argName">The name of the argument to retrieve the integer value from.</param>
    /// <returns>The integer value associated with the specified argument.</returns>
    public int GetIntAmount(string argName)
    {
        return GetValue<int, IntArgument>(argName);
    }

    /// <summary>
    /// Retrieves the unprocessed raw text value associated with the specified argument name.
    /// </summary>
    /// <param name="argName">The name of the argument to retrieve the unprocessed text from.</param>
    /// <returns>The raw, unprocessed string value associated with the specified argument.</returns>
    public string GetUnprocessedText(string argName)
    {
        return GetValue<string, TextArgument>(argName);
    }

    /// <summary>
    /// Retrieves a list of players based on the specified argument.
    /// </summary>
    /// <param name="argName">The name of the argument that contains the list of players to retrieve.</param>
    /// <returns>A list of Player objects representing the players specified by the argument.</returns>
    public List<Player> GetPlayers(string argName)
    {
        return GetValue<List<Player>, PlayersArgument>(argName);
    }

    /// <summary>
    /// Retrieves a single player value from the specified argument.
    /// </summary>
    /// <param name="argName">The name of the argument that contains the single player to retrieve.</param>
    /// <returns>A Player object representing the single player value from the argument.</returns>
    public Player GetPlayer(string argName)
    {
        return GetValue<Player, PlayerArgument>(argName);
    }

    /// <summary>
    /// Retrieves a numeric value from the specified argument.
    /// </summary>
    /// <param name="argName">The name of the argument containing the numeric value to retrieve.</param>
    /// <returns>A floating-point number representing the value from the argument.</returns>
    public float GetFloat(string argName)
    {
        return GetValue<float, FloatArgument>(argName);
    }

    public int GetInt(string argName)
    {
        return GetValue<int, IntArgument>(argName);
    }

    /// <summary>
    /// Retrieves the value of an argument and converts it to the specified enumeration type.
    /// </summary>
    /// <param name="argName">The name of the argument to retrieve the enumeration value from.</param>
    /// <typeparam name="TEnum">The type of the enumeration to convert the argument value to.</typeparam>
    /// <returns>The value of the argument converted to the specified enumeration type.</returns>
    /// <exception cref="AndrzejFuckedUpException">Thrown if the argument cannot be converted to the specified enumeration type.</exception>
    public TEnum GetEnum<TEnum>(string argName) where TEnum : struct, Enum
    {
        var obj = GetValue<object, EnumArgument<TEnum>>(argName);
        if (obj is not TEnum value)
            throw new AndrzejFuckedUpException($"Cannot convert {obj.GetType().Name} to {typeof(TEnum).Name}");

        return value;
    }

    /// <summary>
    /// Retrieves the value of an option argument and converts it to lowercase.
    /// <remarks>
    /// >>>> Return value is always lowercase!
    /// </remarks>
    /// </summary>
    /// <param name="argName">The name of the argument to retrieve the option value from.</param>
    /// <returns>A string containing the lowercase representation of the option value.</returns>
    public string GetOption(string argName)
    {
        return GetValue<string, OptionsArgument>(argName).ToLower();
    }

    /// <summary>
    /// Retrieves a list of remaining arguments based on the specified argument name.
    /// The method resolves provided arguments into a typed list of values.
    /// </summary>
    /// <param name="argName">The name of the argument to retrieve and process remaining values.</param>
    /// <typeparam name="TValue">The type of values to be extracted from the arguments.</typeparam>
    /// <typeparam name="TArg">The argument type that corresponds to the specified argument name.</typeparam>
    /// <returns><typeparamref name="TValue"/> array objects extracted from the remaining arguments.</returns>
    public TValue[] GetRemainingArguments<TValue, TArg>(string argName)
    {
        return GetValues<TValue, TArg>(argName);
    }
    
    private TValue[] GetValues<TValue, TArg>(string argName)
    {
        return GetEvaluators<TValue, TArg>(argName).Select(dtg => dtg.Invoke().Value!).ToArray();
    }

    private TValue GetValue<TValue, TArg>(string argName)
    {
        return GetEvaluators<TValue, TArg>(argName).First().Invoke().Value!;
    }

    private List<DynamicTryGet<TValue>> GetEvaluators<TValue, TArg>(string argName)
    {
        Result mainErr = 
            $"Fetching argument '{argName}' (value {typeof(TValue).Name}) (argtype {typeof(TArg).Name}) " +
            $"for method '{method.Name}' failed.";

        var evaluators = GetValueInternal<TValue, TArg>(argName);

        List<DynamicTryGet<TValue>> resultList = [];
        foreach (var evaluator in evaluators)
        {
            if (evaluator.Result.HasErrored(out var error))
            {
                throw new ScriptErrorException(mainErr + error);
            }

            if (evaluator is not DynamicTryGet<TValue> argEvalRes)
                throw new AndrzejFuckedUpException(
                    mainErr + $"Argument value is not of type {typeof(TValue).Name}");
            
            resultList.Add(argEvalRes);
        }
        
        return resultList;
    }

    private List<DynamicTryGet> GetValueInternal<TValue, TArg>(string argName)
    {
        if (Arguments.TryGetValue((argName, typeof(TArg)), out var value))
        {
            return value;
        }

        var foundArg = method.ExpectedArguments.FirstOrDefault(arg => arg.Name == argName);
        if (foundArg is null)
        {
            throw new AndrzejFuckedUpException($"There is no argument registered of type '{nameof(TArg)}' and name '{argName}'.");
        }

        if (!foundArg.IsOptional)
        {
            throw new ScriptErrorException($"Method '{method.Name}' is missing required argument '{argName}'.");
        }

        return foundArg.DefaultValue switch
        {
            TValue argValue => [new DynamicTryGet<TValue>(argValue)],
            List<TValue> listValue => listValue.Select(DynamicTryGet (v) => new DynamicTryGet<TValue>(v)).ToList(),
            null when foundArg.IsOptional => [new DynamicTryGet<TValue>((TValue)(object)null!)], // magik
            _ => throw new AndrzejFuckedUpException(
                $"Argument {argName} for method {method.Name} has its default value set to type " +
                $"{foundArg.DefaultValue?.GetType().Name ?? "null"}, expected of type {typeof(TValue).Name} or a list of " +
                $"{typeof(TValue).Name}s.")
        };
    }

    public void Add(ArgumentValueInfo valueInfo)
    {
        Log.Debug($"adding {valueInfo.Name} for method {method.Name} ({method.GetHashCode()})");
        if (!valueInfo.IsPartOfCollection)
        {
            Arguments.Add((valueInfo.Name, valueInfo.ArgumentType), [valueInfo.Evaluator]);
            return;
        }
        
        Arguments.AddOrInitListWithKey((valueInfo.Name, valueInfo.ArgumentType), valueInfo.Evaluator);
    }
}
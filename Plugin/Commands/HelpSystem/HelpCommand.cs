using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandSystem;
using SER.Helpers.Exceptions;
using SER.Helpers.Extensions;
using SER.MethodSystem;
using SER.MethodSystem.ArgumentSystem.BaseArguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.Plugin.Commands.Interfaces;
using SER.ScriptSystem.TokenSystem.Tokens.LiteralVariables;
using SER.VariableSystem;
using SER.VariableSystem.Structures;
using EventHandler = SER.ScriptSystem.EventSystem.EventHandler;

namespace SER.Plugin.Commands.HelpSystem;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class HelpCommand : ICommand
{
    public string Command => "serhelp";
    public string[] Aliases => [];
    public string Description => string.Empty;
    
    private static List<Method> AllMethods => MethodIndex.NameToMethodIndex.Values.ToList();

    public static readonly Dictionary<HelpOption, Func<string>> GeneralOptions = new()
    {
        [HelpOption.Methods] = GetMethodList,
        [HelpOption.Variables] = GetVariableList,
        [HelpOption.Enums] = GetEnumHelpPage,
        [HelpOption.Events] = GetEventsHelpPage,
        [HelpOption.RefResMethods] = GetReferenceResolvingMethodsHelpPage,
        [HelpOption.PlayerProperty] = GetPlayerInfoAccessorsHelpPage
    };
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender _, out string response)
    {
        if (arguments.Count > 0)
        {
            return GetGeneralOutput(arguments.First().ToLower(), out response);
        }

        response = GetOptionsList();
        return true;
    }

    public static bool GetGeneralOutput(string arg, out string response)
    {
        if (Enum.TryParse(arg, true, out HelpOption option))
        {
            if (!GeneralOptions.TryGetValue(option, out var func))
            {
                throw new AndrzejFuckedUpException($"Option {option} was not added to the help system.");
            }
            
            response = func();
            return true;
        }
        
        var enumType = HelpInfoStorage.UsedEnums.FirstOrDefault(e => e.Name.ToLower() == arg);
        if (enumType is not null)
        {
            response = GetEnum(enumType);
            return true;
        }
        
        var ev = EventHandler.AvailableEvents.FirstOrDefault(e => e.Name.ToLower() == arg);
        if (ev is not null)
        {
            response = GetEventInfo(ev);
            return true;
        }
        
        var method = MethodIndex.NameToMethodIndex.Values
            .FirstOrDefault(met => met.Name.ToLower() == arg);
        if (method is not null)
        {
            response = GetMethodHelp(method);
            return true;
        }

        response = $"There is no '{arg}' option!";
        return false;
    }

    private static string GetOptionsList()
    {
        return $"""
                === Welcome to the help command of SER! ===

                To get specific information for your script creation adventure:
                (1) find the desired option (like 'methods')
                (2) use this command, attaching the option after it (like 'serhelp methods')
                (3) enjoy!

                Here are all the available options:
                > {"\n> ".Join(Enum.GetValues(typeof(HelpOption)).Cast<HelpOption>()
                    .Select(o => o.ToString().LowerFirst()))}
                    
                    
                === Other commands! ===
                > {"\n> ".Join(Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => typeof(ICommand).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && t != typeof(HelpCommand))
                    .Select(Activator.CreateInstance)
                    .Cast<ICommand>()
                    .Where(c => !string.IsNullOrEmpty(c.Command))
                    .Select(c 
                        => $"{c.Command} (permission: {(c as IUsePermissions)?.Permission ?? "not required"})" + 
                           $"\n{(string.IsNullOrEmpty(c.Description) ? string.Empty : c.Description + "\n")}"))}
                """;
    }

    private static string GetEventInfo(EventInfo ev)
    {
        var variables = EventHandler.GetMimicVariables(ev);
        string msg;
        if (variables.Count > 0)
        {
            msg = "This event has the following variables attached to it:\n";
            foreach (var variable in variables)
            {
                switch (variable)
                {
                    case PlayerVariable:
                        msg += $"> @{variable.Name} (player variable)\n";
                        continue;
                    case ReferenceVariable refVar:
                        msg += $"> {{{refVar.Name}}} (reference variable to {refVar.Type.GetAccurateName()})\n";
                        continue;
                    case LiteralVariable:
                        msg += $"> {{{variable.Name}}} (literal variable)\n";
                        continue;
                    default:
                        throw new AndrzejFuckedUpException();
                }
            }
        }
        else
        {
            msg = "This event does not have any variables attached to it.";
        }
        
        return 
             $"""
              Event {ev.Name} is a part of {ev.DeclaringType?.Name ?? "unknown event group"}.
              
              {msg}
              """;
    }
    
    private static string GetReferenceResolvingMethodsHelpPage()
    {
        var referenceResolvingMethods = AllMethods
            .Where(m => m is IReferenceResolvingMethod)
            .Select(m => (m.Name, ((IReferenceResolvingMethod)m).ReferenceType));
        
        var sb = new StringBuilder();
        foreach (var method in referenceResolvingMethods)
        {
            sb.AppendLine($"{method.ReferenceType.GetAccurateName()} ref -> {method.Name} method");
        }
        
        return
            $"""
             Reference resolving methods are methods that help you extract information from a given reference.
             This help option is just here to make it easier to find said methods.
             
             Here are all reference resolving methods:
             {sb}
             """;
    }

    private static string GetEventsHelpPage()
    {
        var sb = new StringBuilder();
        
        foreach (var category in EventHandler.AvailableEvents.Select(ev => ev.DeclaringType).ToHashSet().OfType<Type>())
        {
            sb.AppendLine($"--- {category.Name} ---");
            sb.AppendLine(string.Join(", ",  EventHandler.AvailableEvents
                .Where(ev => ev.DeclaringType == category)
                .Select(ev => ev.Name)));
        }
        
        return
            $"""
            Event is a signal that something happened on the server. 
            If the round has started, server will invoke an event (signal) called RoundStarted.
            You can use this functionality to run your scripts when a certain event happens.
            
            By putting `!-- Event RoundStarted` at the top of your script, you will run your script when the round starts.
            You can put something different there, e.g. `!-- Event Death`, which will run when someone has died.
            
            Some events have additional information attached to them in a form of variables.
            If you wish to know what variables are available for a given event, just use 'serhelp <eventName>'!
            
            Here are all events that SER can use:
            {sb}
            """;
    }
    
    private static string GetEnum(Type enumType)
    {
        return
            $"""
            Enum {enumType.Name} has the following values:
            {string.Join("\n", Enum.GetValues(enumType).Cast<Enum>().Select(e => $"> {e}"))}
            """;
    }

    private static string GetEnumHelpPage()
    {
        return 
            $"""
            Enums are basically options, where an enum has set of all valid values, so a valid option is an enum value.
            These enums are usually used to specify a room, door, zone etc.
            
            To get the list of all available values that an enum has, just use 'serhelp <enumName>'.
            For example: 'serhelp RoomName' will get you a list of all available room names to use in methods.
            
            Here are all enums used in SER:
            {string.Join("\n", HelpInfoStorage.UsedEnums.Select(e => $"> {e.Name}"))}
            """;
    }

    private static string GetMethodList()
    {
        var maxMethodLen = AllMethods.Max(m => m.Name.Length) + 7;
        
        Dictionary<string, List<Method>> methodsByCategory = new();
        foreach (var method in AllMethods)
        {
            if (methodsByCategory.ContainsKey(method.Subgroup))
            {
                methodsByCategory[method.Subgroup].Add(method);
            }
            else
            {
                methodsByCategory.Add(method.Subgroup, [method]);
            }
        }
        
        var sb = new StringBuilder($"Hi! There are {AllMethods.Count} methods available for your use!\n");
        sb.AppendLine("If a method has [txt], [plr] or [... ref], it represents a method returning text, players or an object reference accordingly.");
        sb.AppendLine("If a method has 'pure' (e.g. [pure text]) it represents a method having no side effects on the server, players, map etc. Used mostly for getting/manipulating variables.");
        
        foreach (var kvp in methodsByCategory.Reverse())
        {
            sb.AppendLine();
            sb.AppendLine($"--- {kvp.Key} methods ---");
            foreach (var method in kvp.Value)
            {
                var purePrefix = "";
                if (method is IPureMethod)
                {
                    purePrefix = "pure ";
                }
                
                var name = method.Name;
                switch (method)
                {
                    case TextReturningMethod:
                        name += $" [{purePrefix}txt]";
                        break;
                    case PlayerReturningMethod:
                        name += $" [{purePrefix}plr]";
                        break;
                    case ReferenceReturningMethod refMethod:
                        name += $" [{purePrefix}{refMethod.ReturnType.Name.ToLower()} ref]";
                        break;
                }

                if (maxMethodLen - name.Length > 0)
                {
                    var descFormatted = method.Description.Insert(0, new string(' ', maxMethodLen - name.Length));
                    sb.AppendLine($"> {name} {descFormatted}");
                }
                else
                {
                    sb.AppendLine($"> {name} {method.Description}");
                }
            }
        }

        sb.AppendLine();
        sb.AppendLine("If you want to get specific information about a given method, just do 'serhelp MethodName'!");
        
        return sb.ToString();
    }
    
    private static string GetVariableList()
    {
        var allVars = PlayerVariableIndex.GlobalPlayerVariables
            .Where(var => var is PredefinedPlayerVariable)
            .Cast<PredefinedPlayerVariable>()
            .ToList();
        
        var sb = new StringBuilder($"Hi! There are {allVars.Count} variables available for your use!\n");
        
        var categories = allVars.Select(var => var.Category).Distinct().ToList();
        foreach (var category in categories)
        {
            sb.AppendLine();
            sb.AppendLine($"--- {category ?? "Other"} variables ---");
            foreach (var var in allVars.Where(var => var.Category == category))
            {
                sb.AppendLine($"> @{var.Name}");
            }
        }
        
        return sb.ToString();
    }

    private static string GetMethodHelp(Method method)
    {
        var sb = new StringBuilder($"=== {method.Name} ===\n");
        sb.AppendLine($"> {method.Description}");
        if (method is IAdditionalDescription addDesc)
        {
            sb.AppendLine($"> {addDesc.AdditionalDescription}");
        }

        sb.AppendLine();
        
        switch (method)
        {
            case TextReturningMethod:
                sb.AppendLine("This method returns a text value, which can be saved or used directly.");
                break;
            case PlayerReturningMethod:
                sb.AppendLine("This method returns a player value, which can be saved or used directly.");
                break;
            case ReferenceReturningMethod refMethod:
                sb.AppendLine($"This method returns a reference to {refMethod.ReturnType.Name} object, which can be saved or used directly.\n" +
                              $"References represent an object which cannot be fully represented in text.\n" +
                              $"If you wish to use that reference further, find methods supporting references of this type.");
                break;
        } 
        
        sb.AppendLine();

        if (method.ExpectedArguments.Length == 0)
        {
            sb.AppendLine("This method does not expect any arguments.");
            return sb.ToString();
        }
        
        sb.AppendLine("This method expects the following arguments:");
        for (var index = 0; index < method.ExpectedArguments.Length; index++)
        {
            var argument = method.ExpectedArguments[index];
            var optionalArgPrefix = argument.IsOptional ? " optional" : "";
            sb.AppendLine($"({index + 1}){optionalArgPrefix} '{argument.Name}' argument");

            if (argument.Description is not null)
            {
                sb.AppendLine($" - Description: {argument.Description}");
            }

            if (argument is CustomMethodArgument customArgument)
            {
                sb.AppendLine($" - Expected value: {customArgument.InputDescription}");
            }
            else
            {
                sb.AppendLine($" - Expected value: {argument.GetExpectedValues()}");
            }

            if (argument.IsOptional)
            {
                sb.AppendLine($" - Default value: {argument.DefaultValue}");
            }

            if (argument.ConsumesRemainingValues)
            {
                sb.AppendLine(
                    " - This argument consumes all remaining values; this means that every value provided AFTER " +
                    "this one will ALSO count towards this argument's values.");
            }

            sb.AppendLine();
        }

        if (method.ExpectedArguments.All(arg => arg.AdditionalDescription is null))
        {
            return sb.ToString();
        }

        sb.AppendLine("\nAdditional information about arguments:");
        for (var index = 0; index < method.ExpectedArguments.Length; index++)
        {
            var argument = method.ExpectedArguments[index];
            if (argument.AdditionalDescription is null) continue;
            
            sb.AppendLine($"({index + 1}) '{argument.Name}' argument");
            sb.AppendLine($" - {argument.AdditionalDescription}");
            sb.AppendLine();
        }
        
        return sb.ToString();
    }

    public static string GetPlayerInfoAccessorsHelpPage()
    {
        var accessors = PlayerPropertyAccessToken.AccessiblePlayerProperties.Select(kvp =>
        {
            if (kvp.Key.Item1 is { } name)
            {
                return $".{name}\nReturns: {kvp.Value.description}";
            }

            if (kvp.Key.Item2 is { } names)
            {
                return $"{names.Select(n => $".{n}").JoinStrings(" or ")}\nReturns: {kvp.Value.description}";
            }

            throw new AndrzejFuckedUpException();
        }).JoinStrings("\n\n");
        
        return 
            """
            Player property accessors are suffixes added to a player variable with a single player to extract information about said player.

            Assuming you have a variable called '@myPlayer' that has 1 player, you can access properties like:
            name:   @myPlayer.name
            role:   @myPlayer.role
            health: @myPlayer.health
            etc.
            
            This works like any other literal variable, so you can save it to a variable, use it in a method, etc.

            Here is a list of all player property accessors and their definitions:
            
            """ + accessors;
    }
}












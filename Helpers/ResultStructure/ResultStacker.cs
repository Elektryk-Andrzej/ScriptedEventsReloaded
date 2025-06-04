using System.Diagnostics.Contracts;
using System.Linq;

namespace SER.Helpers.ResultStructure;

[Pure]
public class ResultStacker(string initMsg)
{
    public readonly string InitMsg = initMsg;
    
    private static string Process(string value)
    {
        if (value.Length < 2) return value;
        
        if (char.IsLower(value.First()))
        {
            value = value.First().ToString().ToUpper() + value.Substring(1);
        }

        if (!char.IsPunctuation(value.Last()))
        {
            value += ".";
        }
        
        return value;
    }
    
    [Pure]
    public Result Add(string other)
    {
        return $"{other}\n-> {Process(InitMsg)}";
    }
}
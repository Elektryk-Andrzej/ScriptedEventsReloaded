namespace SER.ScriptSystem.ContextSystem.Structures;

public interface IKeywordContext
{
    public string Keyword { get; }
    public string Description { get; }
    public string? Arguments { get; }
}
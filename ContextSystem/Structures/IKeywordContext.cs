namespace SER.ContextSystem.Structures;

public interface IKeywordContext
{
    public string KeywordName { get; }
    public string Description { get; }
    public string[] Arguments { get; }
}
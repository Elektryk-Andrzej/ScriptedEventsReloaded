namespace SER.ScriptSystem.ContextSystem.Structures;

public interface IStatementExtender
{
    public abstract IExtendableStatement.Signal Extends { get; }
}
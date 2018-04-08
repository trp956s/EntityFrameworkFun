namespace ExecutionStrategyCore
{
    public interface ITaskRunner
    {
        T Run<T>(IRunner<T> wrapper);
    }
}
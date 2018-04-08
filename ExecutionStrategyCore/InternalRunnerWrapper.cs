namespace ExecutionStrategyCore
{
    public class InternalRunnerWrapper<T>
    {
        public InternalRunnerWrapper(IRunner<T> runner)
        {
            Runner = runner;
        }

        internal IRunner<T> Runner { get; set; }
    }
}

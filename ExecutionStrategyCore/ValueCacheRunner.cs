namespace ExecutionStrategyCore
{
    public struct ValueCacheRunner<T> : IRunner<T>
    {
        private readonly T value;

        public ValueCacheRunner(T value)
        {
            this.value = value;
        }

        public T Run()
        {
            return value;
        }
    }
}

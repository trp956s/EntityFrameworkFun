namespace ExecutionStrategyCore
{
    public interface IMapper<T1, T2>
    {
        T2 Run(T1 arg);
    }
}

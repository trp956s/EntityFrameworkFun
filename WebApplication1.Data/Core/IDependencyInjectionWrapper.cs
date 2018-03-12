namespace WebApplication1.Data.Core
{
    public interface IDependencyInjectionWrapper<T>
    {
        T Dependency { get; }
    }
}

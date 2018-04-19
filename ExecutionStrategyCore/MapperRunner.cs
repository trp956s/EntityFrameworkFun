using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public struct MapperRunner<MapParameters, MapResultType> : IRunner<Task<MapResultType>>
    {
        private readonly IMapper<MapParameters, Task<InternalValueCache<MapResultType>>> ga;
        private readonly IRunner<MapParameters> parameters;

        public MapperRunner(IMapper<MapParameters, Task<InternalValueCache<MapResultType>>> ga, IRunner<MapParameters> parameters)
        {
            this.ga = ga;
            this.parameters = parameters;
        }

        public async Task<MapResultType> Run()
        {
            var valueWrapper = await ga.Run(parameters.Run());
            return valueWrapper.Value;
        }
    }
}

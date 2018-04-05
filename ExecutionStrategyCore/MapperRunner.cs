using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public struct MapperRunner<MapParameters, MapResultType> : IRunner<Task<MapResultType>>
    {
        private readonly IMapper<MapParameters, Task<MapResultType>> ga;
        private readonly IRunner<MapParameters> parameters;

        public MapperRunner(IMapper<MapParameters, Task<MapResultType>> ga, IRunner<MapParameters> parameters)
        {
            this.ga = ga;
            this.parameters = parameters;
        }

        public async Task<MapResultType> Run()
        {
            return await ga.Run(parameters.Run());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public class MapperRunner<MapParameters, MapResultType>: IRunner<Task<InternalRunnerWrapper<MapResultType>>>
    {
        private readonly IMapper<MapParameters, Task<InternalRunnerWrapper<MapResultType>>> ga;
        private readonly IRunner<MapParameters> parameters;

        public MapperRunner(IMapper<MapParameters, Task<InternalRunnerWrapper<MapResultType>>> ga, IRunner<MapParameters> parameters)
        {
            this.ga = ga;
            this.parameters = parameters;
        }

        public async Task<InternalRunnerWrapper<MapResultType>> Run()
        {
            return await ga.Run(parameters.Run());
        }
    }
}

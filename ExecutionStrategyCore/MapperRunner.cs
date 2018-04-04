using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public class MapperRunner<MapParameters, MapResultType>: IRunner<InternalRunnerWrapper<Task<MapResultType>>>
    {
        private readonly IMapper<MapParameters, InternalRunnerWrapper<Task<MapResultType>>> ga;
        private readonly MapParameters parameters;

        public MapperRunner(IMapper<MapParameters, InternalRunnerWrapper<Task<MapResultType>>> ga, MapParameters parameters)
        {
            this.ga = ga;
            this.parameters = parameters;
        }

        public InternalRunnerWrapper<Task<MapResultType>> Run()
        {
            return ga.Run(parameters);
        }
    }
}

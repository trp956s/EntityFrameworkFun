using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionStrategyCore
{
    public static class MapperExtensions
    {
        public static async Task<MapResultType> Run<MapParameters, MapResultType>(this IMapper<MapParameters, InternalRunnerWrapper<Task<MapResultType>>> ga, ITaskRunner runner, MapParameters parameters)
        {
            return await runner.Run(ga.Run(parameters));
        }
    }
}

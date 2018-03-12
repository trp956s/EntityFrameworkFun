using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.Helpers;

namespace WebApplication1.Data.Core
{
    /// <summary>
    /// Runs an executable with a given dependency.
    /// </summary>
    public interface IAsyncExecutableRunner
    {
        /// <summary>
        /// Runs an executable with a given dependency.
        /// </summary>
        /// <typeparam name="ArgumentsTypeT"></typeparam>
        /// <typeparam name="ReturnTypeT"></typeparam>
        /// <param name="executable">

        /// The executable could be, for example, a service or a query.  The executable should have all context specific 
        /// arguments it would need to run, like the id of the record being looked up.  What it should not have is anything 
        /// configuration based or dependency injected, such as the database connection to run the query against.

        /// </param>
        /// <param name="di">

        /// The single instance containing all configuration or dependency injection needed to supply the executable.  For
        /// instance, an instance of a DbSet<ArgumentsTypeT> presented as an IAsyncEnumerable

        /// </param>
        /// <returns></returns>
        Task<ReturnTypeT> Run<ArgumentsTypeT, ReturnTypeT>(IExecutable<ArgumentsTypeT, ReturnTypeT> executable, IDependencyInjectionWrapper<ArgumentsTypeT> di)
        where ArgumentsTypeT : class;
    }
}

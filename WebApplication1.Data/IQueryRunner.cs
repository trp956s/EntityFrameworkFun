using System.Threading.Tasks;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace WebApplication1.Data
{
    public interface IQueryRunner
    {
        Task<ReturnT> Run<DataSetT, ReturnT>(IQuery<DataSetT, BloggingContext, ReturnT> query) where DataSetT : class;
    }

    public interface IDbSetWrapper<T> where T: class
    {
        IEnumerable<T> DbSet { get; }
    }

    public class AsyncExecutableRunner
    {
        public virtual Task<ReturnTypeT> Run<ArgumentsTypeT, ReturnTypeT>(IExecutable<ArgumentsTypeT, ReturnTypeT> executable, IDependencyInjectionWrapper<ArgumentsTypeT> di) 
        where ArgumentsTypeT : class 
        {
            return executable.Execute(di.Dependency);
        }
    }

    public interface IDependencyInjectionWrapper<T>
    {
        T Dependency {get;}
    }

    public class DependencyInjectionWrapper<T> : IDependencyInjectionWrapper<IAsyncEnumerable<T>>
    where T : class
    {
        private readonly IDbSetWrapper<T> _dbSetWrapper;

        public DependencyInjectionWrapper(IDbSetWrapper<T> dbSetWrapper)
        {
            this._dbSetWrapper = dbSetWrapper;
        }
        public virtual IAsyncEnumerable<T> Dependency 
            {
                get
                {
                return _dbSetWrapper.DbSet.ToAsyncEnumerable<T>();
                }
            }
    }
}
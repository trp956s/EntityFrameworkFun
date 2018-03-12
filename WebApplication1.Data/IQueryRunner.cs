using System.Threading.Tasks;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace WebApplication1.Data
{
    public interface IDbSetWrapper<T> where T: class
    {
        IEnumerable<T> DbSet { get; }
    }

    public interface IAsyncExecutableRunner
    {
        Task<ReturnTypeT> Run<ArgumentsTypeT, ReturnTypeT>(IExecutable<ArgumentsTypeT, ReturnTypeT> executable, IDependencyInjectionWrapper<ArgumentsTypeT> di)
        where ArgumentsTypeT : class;
    }

    public class AsyncExecutableRunner : IAsyncExecutableRunner
    {
        public Task<ReturnTypeT> Run<ArgumentsTypeT, ReturnTypeT>(IExecutable<ArgumentsTypeT, ReturnTypeT> executable, IDependencyInjectionWrapper<ArgumentsTypeT> di) 
        where ArgumentsTypeT : class 
        {
            return executable.Execute(di.Dependency);
        }
    }

    public interface IDependencyInjectionWrapper<T>
    {
        T Dependency {get;}
    }

    public class DbSetInjection<T> : IDependencyInjectionWrapper<IAsyncEnumerable<T>>
    where T : class
    {
        public DbSetInjection(IDbSetWrapper<T> dbSetWrapper)
        {
            DbSetWrapper = dbSetWrapper;
        }
        public IAsyncEnumerable<T> Dependency => DbSetWrapper.DbSet.ToAsyncEnumerable<T>();

        public IDbSetWrapper<T> DbSetWrapper {get;}
    }
}
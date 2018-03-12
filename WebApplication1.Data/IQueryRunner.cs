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

    public class AsyncExecutableRunner<ArgumentsTypeT, ReturnTypeT> where ArgumentsTypeT : class 
    {
        public virtual Task<ReturnTypeT> Return(IExecutable<ArgumentsTypeT, ReturnTypeT> executable, Func<ArgumentsTypeT> argumentsFunction)
        {
            return executable.Execute(argumentsFunction());
        }
    }

    public class AsyncEnumerableWrapper<T> where T : class
    {
        private readonly IDbSetWrapper<T> _dbSetWrapper;

        public AsyncEnumerableWrapper(IDbSetWrapper<T> dbSetWrapper)
        {
            this._dbSetWrapper = dbSetWrapper;
        }
        public virtual IAsyncEnumerable<T> ToAsyncEnumerable()
        {
            return _dbSetWrapper.DbSet.ToAsyncEnumerable<T>();
        }
    }

    public class DbSetContextWrapper<T> where T : class
    {
        private IEnumerable<T> _context;

        public DbSetContextWrapper(IEnumerable<T> context)
        {
            this._context = context;
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Data.Models;

namespace WebApplication1.Data
{
    public static class TFilter
    {
        public static TFilter<T> CreateNew<T>(Expression<Func<T, bool>> expression, Func<IQueryable<T>, Expression<Func<T, bool>>, Task<T>> expressionTarget)
        {
            return new TFilter<T>(expression, expressionTarget);
        }
    }


    public class TFilter<T>
    {
        //types of filters: 1. Expression + Target, 2. Target only
        public TFilter(Expression<Func<T, bool>> expression, Func<IQueryable<T>, Expression<Func<T, bool>>, Task<T>> expressionTarget)
        {
            Expression = expression;
            ExpressionTarget = expressionTarget;
        }
        public Expression<Func<T, bool>> Expression { get; }
        public Func<IQueryable<T>, Expression<Func<T, bool>>, Task<T>> ExpressionTarget { get; }
    }

    public interface IFilterFactory<T>
    {
        TFilter<T> ExpressionFilter { get; }
    }

    //this is what you make/query in the service
    public class BlogsWhereSomething : IFilterFactory<Blog>
    {
        //CONTEXT FOR THE QUERY WOULD BE INCLUDED HERE

        public CancellationToken CancellationToken { get; internal set; }
        public TFilter<Blog> ExpressionFilter
        {
            get
            {
                return TFilter.CreateNew<Blog>(c => true, (ds, predicate) => ds.FirstAsync<Blog>(predicate, this.CancellationToken));
            }
        }
    }

    //TODO: make an interface for Run so that refrences to DbSetFilterExecutor<T> can be replaced with interface
    public class DbSetFilterExecutor<T>
    where T : class
    {
        public DbSetFilterExecutor(DbSet<T> dbSet)
        {
            DbSet = dbSet;
        }

        public DbSet<T> DbSet { get; }

        public async Task<T> Run(IFilterFactory<T> filterFactory)
        {
            var filter = filterFactory.ExpressionFilter;
            return await filter.ExpressionTarget(DbSet, filter.Expression);
        }
    }

    public class DbContextSaveWrapper
    {
        public DbContextSaveWrapper(DbContext context)
        {
            Context = context;
        }

        public DbContext Context { get; }

        public async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
        {
            return await Context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }

    public class DbSetUpsertDeleteExecutor<T>
    where T : class
    {
        private Func<T, CancellationToken, Task<EntityEntry<Blog>>> addAsync;

        public DbContextSaveWrapper DbContextSaveWrapper { get; }

        public DbSetUpsertDeleteExecutor(Func<T, CancellationToken, Task<EntityEntry<Blog>>> addAsync, DbContextSaveWrapper dbContextSaveWrapper)
        {
            this.addAsync = addAsync;
            DbContextSaveWrapper = dbContextSaveWrapper;
        }

        internal async Task Insert(T entity)
        {
            //TODO: make class
            var insertWrapper = new {
                Entity = entity,
                CancellationToken = CancellationToken.None
            };
            await addAsync(insertWrapper.Entity, insertWrapper.CancellationToken);
        }

        internal async Task<int> Update(T entity)
        {
            //TODO: make class
            var updateWrapper = new {
                NewValues = entity,
                PopulateFoundEntity = new Action<T>(t=>{ /* update t properties to match NewValues */ }),
                AcceptAllChangesOnSuccess = false,
                CancellationToken = CancellationToken.None,
                FindEntityToUpdateQuery = (IFilterFactory<T>) null,
                DbSetFilterExecutor = (DbSetFilterExecutor<T>)null
            };

            var entityToUpdate = await updateWrapper.DbSetFilterExecutor.Run(updateWrapper.FindEntityToUpdateQuery);
            return await DbContextSaveWrapper.SaveChangesAsync(updateWrapper.AcceptAllChangesOnSuccess, updateWrapper.CancellationToken);
        }

        //TODO: combine ABOVE to make upsert

        internal async Task<int> Delete(IFilterFactory<T> filter)
        {
            //TODO: make class
            var deleteWrapper = new
            {
                AcceptAllChangesOnSuccess = false,
                CancellationToken = CancellationToken.None,
                FindEntityToDeleteQuery = filter,
                DbSetFilterExecutor = (DbSetFilterExecutor<T>)null
            };

            var entityToDelete = await deleteWrapper.DbSetFilterExecutor.Run(deleteWrapper.FindEntityToDeleteQuery);
            return await DbContextSaveWrapper.SaveChangesAsync(deleteWrapper.AcceptAllChangesOnSuccess, deleteWrapper.CancellationToken);
        }
    }

    public class ServiceRunner<T>
    where T : class
    {
        public ServiceRunner(DbSetFilterExecutor<T> filterExecutor, IFilterFactory<T> filterFactory)
        {
            FilterExecutor = filterExecutor;
            FilterFactory = filterFactory;
        }

        public DbSetFilterExecutor<T> FilterExecutor { get; }
        public IFilterFactory<T> FilterFactory { get; }

        public Task<T> Run()
        {
            return FilterExecutor.Run(FilterFactory);
        }
    }

    //this is what you make/service call
    public class SomeBlogServiceContext
    {

    }

    //this is what you make/table, inject and mock for queries
    public interface IBlogQueryExecutionWrapper
    {
        DbSetFilterExecutor<Blog> BlogQueryExecutor { get; }
    }

    //this is what you make/table, inject and mock for updates
    public interface IBlogUpsertDeleteExecutionWrapper
    {
        DbSetUpsertDeleteExecutor<Blog> BlogUpsertDeleteExecutor { get; }
    }

    //this is what you make/table, inject and mock
    public class BlogExecutorWrapper : IBlogQueryExecutionWrapper, IBlogUpsertDeleteExecutionWrapper
    {
        public BlogExecutorWrapper(BloggingContext context)
        {
            BlogQueryExecutor = new DbSetFilterExecutor<Blog>(context.Blogs);
            BlogUpsertDeleteExecutor = new DbSetUpsertDeleteExecutor<Blog>(context.Blogs.AddAsync, new DbContextSaveWrapper(context));
        }

        public DbSetFilterExecutor<Blog> BlogQueryExecutor { get; }
        public DbSetUpsertDeleteExecutor<Blog> BlogUpsertDeleteExecutor { get; }
    }

    //this is what you make/service, inject and mock
    public class SomeBlogServiceWrapper
    {
        public SomeBlogServiceWrapper(IBlogQueryExecutionWrapper blogQueryExecutionWrapper, IBlogUpsertDeleteExecutionWrapper blogUpsertDeleteExecutionWrapper)
        {
            BlogQueryExecutionWrapper = blogQueryExecutionWrapper;
            BlogUpsertDeleteExecutionWrapper = blogUpsertDeleteExecutionWrapper;
        }

        public IBlogQueryExecutionWrapper BlogQueryExecutionWrapper { get; }
        public IBlogUpsertDeleteExecutionWrapper BlogUpsertDeleteExecutionWrapper { get; }

        public ServiceRunner<Blog> GetFirstBlogWhereSomething(SomeBlogServiceContext context)
        {
            return new ServiceRunner<Blog>(
                BlogQueryExecutionWrapper.BlogQueryExecutor, 
                (IFilterFactory<Blog>)new BlogsWhereSomething() //copy values from SomeBlogServiceContext
            );
        }
    }

    public class StrategyExecutor
    {
        public async Task Run()
        {
            //TODO: make a query that returns multiple results
            var blogExecutionWrapper = new BlogExecutorWrapper((BloggingContext)null);
            IBlogQueryExecutionWrapper injectedAndMocked = blogExecutionWrapper;
            await injectedAndMocked.BlogQueryExecutor.Run(new BlogsWhereSomething());

            IBlogUpsertDeleteExecutionWrapper injectedAndMocked2 = blogExecutionWrapper;
            await injectedAndMocked2.BlogUpsertDeleteExecutor.Insert(new Blog());
            await injectedAndMocked2.BlogUpsertDeleteExecutor.Update(new Blog());
            await injectedAndMocked2.BlogUpsertDeleteExecutor.Delete(new BlogsWhereSomething());


            var someServiceWhichIsInjectedAndMocked = new SomeBlogServiceWrapper(blogExecutionWrapper, blogExecutionWrapper);
            var service = someServiceWhichIsInjectedAndMocked.GetFirstBlogWhereSomething(new SomeBlogServiceContext());
            await service.Run();
        }
    }

    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> context) : base(context) { }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}

﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.Queries.BlogPersistanceLayer;
using WebApplication1.Data.Models;
using WebApplication1.Data.Injectors;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Core;
using WebApplication1.Data.Upserts;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    //todo: 9,  10, 11: test drive the rest of this class keeping blogs simple
    //todo: 12, 13, 14, 15, 16: test drive post using a service to encapsulate relationships and business rules and a PostApi to pass to/from the controller
    //todo: 17 - use custom logger to show full path names
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        public readonly IAsyncExecutableRunner _runner;
        private readonly BlogDbSetInjector _blogContext;

        public BlogController(IAsyncExecutableRunner runner, BlogDbSetInjector blogContext)
        {
            _runner = runner;
            _blogContext = blogContext;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var results = await _runner.Run(
                new QueryAllBlogs(), 
                new DbSetInjection<Blog>(_blogContext)
            );

            if (results.Any())
            {
                return Ok(results);
            }

            return NotFound();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var query = new QueryBlogsById(id);
            var results = await _runner.Run(
                query,
                new DbSetInjection<Blog>(_blogContext)
            );

            if(results == null)
            {
                return NotFound();
            }

            return Ok(results);
        }

        // POST api/values
        [HttpPost]
        public async Task<StatusCodeResult> Post(Blog blog)
        {
            if(blog == null)
            {
                return NotFound();
            }

            var query = new QueryBlogsById(blog.Id);
            var results = await _runner.Run(
                query,
                new DbSetInjection<Blog>(_blogContext)
            );

            if (results == null)
            {
                return NotFound();
            }

            await _runner.Run(
                new InsertBlog(),
                new DbSetInjection<Blog>(_blogContext)
            );

            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]string value)
        {
            // pretend these were injected
            BlogDbSetInjector blogInjector1 = null;
            BlogDbSetInjector blogInjector2 = null;

            await _runner.Run(
                new ExampleExecutableOfMultipleDbSets(),
                new ExampleInjectionOfMultipleDbSets(blogInjector1, blogInjector2)
            );
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class ExampleExecutableOfMultipleDbSets : IExecutable<ExampleMultipleDbSets, object>//IExecutable<IAsyncEnumerable<Blog>, Blog>
    {
        public Task<object> Execute(ExampleMultipleDbSets queryable)
        {
            var v = queryable.BlogInjector1;
            var v2 = queryable.BlogInjector1;

            return Task.FromResult((object)null);
        }
    }

    public class ExampleInjectionOfMultipleDbSets : IDependencyInjectionWrapper<ExampleMultipleDbSets>
    {
        public ExampleInjectionOfMultipleDbSets(BlogDbSetInjector blogInjector1, BlogDbSetInjector blogInjector2)
        {
            Dependency = new ExampleMultipleDbSets(blogInjector1, blogInjector2);
        }

        public ExampleMultipleDbSets Dependency { get; }
    }

    public class ExampleMultipleDbSets
    {
        public ExampleMultipleDbSets(IDbSetWrapper<Blog> blogWrapper1, IDbSetWrapper<Blog> blogWrapper2)
        {
            BlogWrapper1 = blogWrapper1;
            BlogWrapper2 = blogWrapper2;
        }

        public IAsyncEnumerable<Blog> BlogInjector1 => BlogWrapper1.DbSet.ToAsyncEnumerable();
        public IAsyncEnumerable<Blog> BlogInjector2 => BlogWrapper2.DbSet.ToAsyncEnumerable();
        public IDbSetWrapper<Blog> BlogWrapper1 { get; }
        public IDbSetWrapper<Blog> BlogWrapper2 { get; }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ExecutionStrategyCore;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.Models;
using WebApplication1.Data.Queries;

namespace WebApplication1.Controllers
{
    //todo: clean up code, consolidate stories 1-3, reduce test code
    //todo: get by id.. 9-11 are post, put, and delete
    //todo: 9,  10, 11: test drive the rest of this class keeping blogs simple
    //todo: 12: refactor injection to use IRunner
    //todo: 13: refactor DbSetRunner
    //todo: 14, 15, 16, 17, 18: test drive post using a service to encapsulate relationships and business rules and a PostApi to pass to/from the controller
    //todo: 19 - use custom logger to show full path names
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly ITaskRunner runner;
        private readonly BlogDbSetRunner blogData;

        public BlogController(ITaskRunner runner, BlogDbSetRunner blogData) {
            this.runner = runner;
            this.blogData = blogData;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            Func<Task<ActionResult>> notFoundFunc = () => Task.FromResult((ActionResult)NotFound());
            var strategyToggle = new StoryOverrideFunctionRunner<Task<ActionResult>>(
                notFoundFunc,
                GetAllBlogs,
                GetAllBlogs2,
                GetAllBlogs3
            );
            return await runner.Run(strategyToggle);
        }

        [Story("1")]
        private async Task<ActionResult> GetAllBlogs()
        {
            return await Task.FromResult(Ok(new Blog[] { }));
        }

        [Story("2")]
        private async Task<ActionResult> GetAllBlogs2()
        {
            return await Task.FromResult(Ok(new Blog[] {
                new Blog()
            }));
        }

        [Story("3")]
        private async Task<ActionResult> GetAllBlogs3()
        {
            var queryResult = await runner.Run(new GetAll<Blog>(), (IRunner<DbSet<Blog>>)blogData);

            return Ok(queryResult);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            return await runner.Run(ExecutionStrategy.Create<ActionResult>(() => NotFound()));
        }

        // POST api/values
        [HttpPost]
        public async Task<StatusCodeResult> Post([FromBody] Blog blog)
        {
            return await runner.Run(ExecutionStrategy.Create<StatusCodeResult>(() => BadRequest()));
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

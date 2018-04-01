using System;
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
    //todo: 12, 13, 14, 15, 16: test drive post using a service to encapsulate relationships and business rules and a PostApi to pass to/from the controller
    //todo: 17 - use custom logger to show full path names
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly IExecutionStrategyRunner runner;
        private readonly DbSetWrapper<Blog> blogData;

        public BlogController(IExecutionStrategyRunner runner, DbSetWrapper<Blog> blogData) {
            this.runner = runner;
            this.blogData = blogData;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var notFoundStrategy = ExecutionStrategy.Create<ActionResult>(()=>NotFound());
            var strategyToggle = StoryOverrideExecutionStrategy.Create<ActionResult>(
                notFoundStrategy,
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
            var runStrategy = new GetAll<Blog>(blogData).CreateExecutionStrategy();
            var queryResult = await runner.Run(runStrategy);

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

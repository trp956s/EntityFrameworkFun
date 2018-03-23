using System.Threading.Tasks;
using ExecutionStrategyCore;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.Models;

namespace WebApplication1.Controllers
{
    //todo: 9,  10, 11: test drive the rest of this class keeping blogs simple
    //todo: 12, 13, 14, 15, 16: test drive post using a service to encapsulate relationships and business rules and a PostApi to pass to/from the controller
    //todo: 17 - use custom logger to show full path names
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly IExecutionStrategyRunner runner;

        public BlogController(IExecutionStrategyRunner runner) {
            this.runner = runner;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var notFoundStrategy = ExecutionStrategy.Create<ActionResult>(()=>NotFound());
            var strategyToggle = new StoryOverrideExecutionStrategy<ActionResult>(
                notFoundStrategy, 
                () => new ExecutionStrategy<ActionResult>(GetAllBlogs)
            );
            return await runner.Run(strategyToggle);
        }

        [Story("1")]
        private async Task<ActionResult> GetAllBlogs()
        {
            return await Task.FromResult(Ok(new Blog[] { }));
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

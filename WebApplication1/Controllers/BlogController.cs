using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Data.Queries;
using WebApplication1.Data.Queries.BlogPersistanceLayer;
using WebApplication1.Data.Models;
using WebApplication1.Data.Injectors;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Core;

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
        public void Post([FromBody]string value)
        {
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

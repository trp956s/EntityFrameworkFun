using System.Linq;
using System.Threading.Tasks;
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
        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return NotFound();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            return NotFound();
        }

        // POST api/values
        [HttpPost]
        public async Task<StatusCodeResult> Post([FromBody] Blog blog)
        {
            return BadRequest();
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

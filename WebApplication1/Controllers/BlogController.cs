using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Data.Queries;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly IQueryRunner _queryRunner;

        public BlogController(IQueryRunner queryRunner)
        {
            _queryRunner = queryRunner;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ObjectResult Get(int id)
        {
            var query = new BlogPersistanceLayer().GetById(id); //no need to mock because this line can be copied to the test but you can if you want
            return Ok(_queryRunner.Run(query));
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

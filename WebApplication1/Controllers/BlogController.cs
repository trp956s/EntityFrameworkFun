using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ExecutionStrategyCore;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.Models;
using WebApplication1.Data.Queries;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

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
                GetAllBlogs3,
                GetAllBlogs4
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
            return await GetAllBlogsFromDb("3");
        }

        [Story("4")]
        private async Task<ActionResult> GetAllBlogs4()
        {
            return await GetAllBlogsFromDb("4");
        }

        private async Task<ActionResult> GetAllBlogsFromDb(string story)
        {
            var queryResult = await GetAllBlogsFromDb();

            if(story == "4" && !queryResult.Any())
            {
                return NotFound();
            }

            return Ok(queryResult);
        }

        private async Task<IEnumerable<Blog>> GetAllBlogsFromDb()
        {
            return await runner.Run(new GetAll<Blog>(), blogData);
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var story5 = new StoryFunctionRunner<Task<ActionResult>>(() => GetBlog(id), "5");
            var story6 = new StoryFunctionRunner<Task<ActionResult>>(() => GetBlog(id), "6");
            return await runner.Run(new StoryOverrideFunctionRunner<Task<ActionResult>>(
                () => runner.Run(ExecutionStrategy.Create<ActionResult>(() => NotFound())),                
                story5.Run,
                story6.Run
            ));
        }

        private async Task<ActionResult> GetBlog(int id)
        {
            var runResult = await runner.Run(new GetAllById<Blog>(id), blogData);
            return WrapGetSingleBlogIntoAResult(runResult);
        }

        private ActionResult WrapGetSingleBlogIntoAResult(Blog b)
        {
            if (b == null)
            {
                return NotFound();
            }

            return Ok(b);
        }

        // POST api/values
        [HttpPost]
        public async Task<StatusCodeResult> Post([FromBody] Blog blog)
        {
            var post = new StoryFunctionRunner<Task<StatusCodeResult>>(()=>PostBlog(blog), "7", "8", "9");

            return await runner.Run(new StoryOverrideFunctionRunner<Task<StatusCodeResult>>(
                () => Task.FromResult((StatusCodeResult)BadRequest()),
                post.Run
            ));
        }

        private async Task<StatusCodeResult> PostBlog(Blog postBlog)
        {
            if (postBlog == null)
                return await Task.FromResult(BadRequest());
            var result = await runner.Run(new GetAllById<Blog>(postBlog.Id), blogData);

            var story8 = new StoryFunctionRunner<Task<StatusCodeResult>>(() => CheckForConflict(result, postBlog), "8", "9");

            return await runner.Run(new StoryOverrideFunctionRunner<Task<StatusCodeResult>>(
                () => Task.FromResult(StatusCode(StatusCodes.Status409Conflict)),
                story8.Run
            ));
        }

        private async Task<StatusCodeResult> CheckForConflict(Blog matchingPostId, Blog postBlog)
        {
            if(matchingPostId != null)
                return StatusCode(StatusCodes.Status409Conflict);
            var story9 = new StoryFunctionRunner<Task<StatusCodeResult>>(() => CreateNew(postBlog), "9");
            return await runner.Run(new StoryOverrideFunctionRunner<Task<StatusCodeResult>>(
                () => Task.FromResult(StatusCode(StatusCodes.Status201Created)),
                story9.Run
            ));
        }

        private async Task<StatusCodeResult> CreateNew(Blog postBlog)
        {
            await runner.Run(new CreateBlog(postBlog), blogData);
            return StatusCode(StatusCodes.Status201Created);
        }



        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]Blog blog)
        {
            var find = new StoryFunctionRunner<Task<ActionResult>>(() => Find(id), "10", "11");

            var response = await runner.Run(new StoryOverrideFunctionRunner<Task<ActionResult>>(
                () => Task.FromResult<ActionResult>(NotFound()),
                find.Run
            ));

            if(response is OkResult)
            {
                var update = new StoryFunctionRunner<Task<ActionResult>>(() => Update(id, blog), "11");
                return await runner.Run(new StoryOverrideFunctionRunner<Task<ActionResult>>(
                    () => Task.FromResult(response),
                    update.Run
                ));
            }

            return response;
        }

        private async Task<ActionResult> Update(int id, Blog blog)
        {
            blog.Id = id;
            var updatedBlog = await runner.Run(new UpdateBlog(blog), blogData);

            return Ok(updatedBlog);
        }

        private async Task<ActionResult> Find(int id)
        {
            if(null == await runner.Run(new GetAllById<Blog>(id), blogData))
            {
                return NotFound();
            }

            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

﻿using System;
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
using WebApplication1.Data;
using WebApplication1.Data.GeneralInterfaces;

namespace WebApplication1.Controllers
{
    //todo: test the GetAllById2 and DeleteBlog2 classes
    //todo: rewrite Mapper (and runner) to follow command pattern DataInteraction : IInternalDataInteraction { internal T Act(){...}; public InternalExecutionStrategyRunner AsRunner(){return new InternalExecutionStrategyRunner(new InternalDataInteraction(this))**;} };
    //todo: rename Execution Strategy to follow command pattern names
    //todo: **internal helper this.ToInternalRunner()
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
            if (runner.IsStoryOverrideActive(out var story, "1"))
            {
                return await Task.FromResult(Ok(new Blog[] { }));
            }
            else if (runner.IsStoryOverrideActive(out story, "2"))
            {
                return await Task.FromResult(Ok(new Blog[] {
                    new Blog()
                }));
            }
            else if (runner.IsStoryOverrideActive(out story, "3", "4"))
            {
                var queryResult = await GetAllBlogsFromDb();
                if (runner.IsStoryOverrideActive(out story, "4") && queryResult.Count() == 0)
                {
                    return NotFound();
                }
                return Ok(queryResult);
            }

            return NotFound();
        }

        private async Task<IEnumerable<Blog>> GetAllBlogsFromDb()
        {
            return await runner.Run(new GetAll<Blog>(), blogData);
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {

            if(!runner.IsStoryOverrideActive(out var story5Or6, "5", "6"))
            {
                return NotFound();
            }

            return await runner.Run(story5Or6, () => GetBlog(id));
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
            if (!runner.IsStoryOverrideActive(out var story7Or8Or9, "7", "8", "9"))
            {
                return BadRequest();
            }
            return await runner.Run(story7Or8Or9, () => PostBlog(blog));
        }

        private async Task<StatusCodeResult> PostBlog(Blog postBlog)
        {
            if (postBlog == null)
            {
                return await Task.FromResult(BadRequest());
            }

            var result = await runner.Run(new GetAllById<Blog>(postBlog.Id), blogData);

            if(!runner.IsStoryOverrideActive(out var story8Or9, "8", "9"))
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            return await runner.Run(story8Or9, () => CheckForConflict(result, postBlog));
        }

        private async Task<StatusCodeResult> CheckForConflict(Blog matchingPostId, Blog postBlog)
        {
            if (matchingPostId != null)
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            if(!runner.IsStoryOverrideActive(out var story9, "9"))
            {
                return StatusCode(StatusCodes.Status201Created);
            }

            return await runner.Run(story9, () => CreateNew(postBlog));
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
            if (!runner.IsStoryOverrideActive(out var story10Or11, "10", "11"))
            {
                return NotFound();
            }

            var blogFound = await runner.Run(story10Or11, () => Find(id));
            if (blogFound == null)
            {
                return NotFound();
            }

            if (!runner.IsStoryOverrideActive(out var story11, "11"))
            {
                return Ok();
            }

            return await runner.Run(story11, () => Update(blogFound, blog));
        }

        private async Task<ActionResult> Update(Blog existingBlog, Blog newBlogValues)
        {
            var updatedBlog = await runner.Run(new UpdateBlog(existingBlog, newBlogValues), blogData);

            return Ok(updatedBlog);
        }

        private async Task<Blog> Find(int id)
        {
            return await runner.Run(new GetAllById<Blog>(id), blogData);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var y = runner.CreateAsyncMapRuner2();

            var searchForBlogById = runner.For9<Blog>().
                CreateRunner(blogData as IRunner<IQueryable<Blog>>
            );

            var blogFoundById = await y.Run6(
                new GetAllById4<Blog>(id),
                searchForBlogById
            );

            if (blogFoundById == null)
            {
                return NotFound();
            }

            var delete = runner.For9<int>().
                CreateRunner(blogData as IRunner<BloggingContext>
            );

            await y.Run6(new DeleteBlog4(blogFoundById), delete);

            return Ok(null);

            //OH MY GOD IT WORKS!!! 

            //TODO: something about the blogData 'as'
            //var a = await x.For8<int>().Run2(
            //    new DeleteBlog3(blogFoundById), 
            //    blogData as IRunner<BloggingContext>
            //);

            //var b = await x.For8<Blog>().Run2(
            //    new GetAllById3<Blog>(id),
            //    blogData as IRunner<IQueryable<Blog>>
            //);
        }
    }
}

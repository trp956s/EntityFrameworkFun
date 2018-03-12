using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Controllers;
using FakeItEasy;
using FakeItEasy.Configuration;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Data.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Data.Queries.BlogPersistanceLayer;

namespace WebApplication1.Test.Controllers
{
    [TestClass]
    public class BlogControllerTest
    {
        private BlogController _controller;
        private BlogContext _blogContext;
        private IAsyncExecutableRunner _runner;
        [TestInitialize]
        public void Initialize()
        {
            _runner = A.Fake<IAsyncExecutableRunner>();
            _blogContext = A.Fake<BlogContext>(options => options.WithArgumentsForConstructor(new object[] { null }));
            _controller = new BlogController(_runner, _blogContext);
        }

        [TestClass]
        public class Get : BlogControllerTest
        {
            [TestClass]
            public class Empty : BlogControllerTest.Get
            {
                [TestMethod]
                public async Task ReturnsOKWithValue()
                {
                    var blogs = new List<Blog> {
                                new Blog()
                            };

                    A.CallTo(_runner)
                        .Method("Run")
                        .Returns(blogs.ToEnumTask());

                    var result = (OkObjectResult) await _controller.Get();

                    Assert.IsInstanceOfType(result, typeof(OkObjectResult));
                    Assert.AreEqual(blogs, result.Value);
                }

                [TestMethod]
                public async Task ReturnsNotFoundWhenNoBlogs()
                {
                    var result = await _controller.Get();

                    A.CallTo(_runner)
                        .Method("Run")
                        .Returns(
                            new List<Blog>().ToEnumTask()
                        );

                    Assert.IsInstanceOfType(result, typeof(NotFoundResult));
                }

                [TestMethod]
                public async Task SetsUpTheQueryCorrectly()
                {
                    A.CallTo(_runner)
                        .Method("Run")
                        .Returns(
                            new List<Blog>().ToEnumTask()
                        );

                    await _controller.Get();

                    A.CallTo(() => _runner.Run(
                        A<QueryAllBlogs>.That.IsNotNull(), 
                        A<DbSetInjection<Blog>>.That
                            .Matches(arg => 
                                arg.DbSetWrapper == _blogContext
                            )
                        )
                    )
                    .MustHaveHappenedOnceExactly();
                }
            }

            [TestClass]
            public class WithId : Get
            {
                [TestMethod]
                public async Task ReturnsOkWithQueryResult()
                {
                    var id = 0;
                    var fakeBlog = A.Fake<Blog>();
                    A.CallTo(_runner)
                        .Method("Run")
                        .Returns(Task.FromResult(fakeBlog));
                    
                    var result = (ObjectResult) await _controller.Get(id);
                    
                    Assert.IsInstanceOfType(result, typeof(OkObjectResult));
                    Assert.AreEqual(fakeBlog, result.Value);
                }

                [TestMethod]
                public async Task SetsUpQueryResultsCorrectly()
                {
                    var id = 5;
                    var fakeBlog = A.Fake<Blog>();
                    A.CallTo(_runner)
                        .Method("Run")
                        .Returns(Task.FromResult(fakeBlog));

                    await _controller.Get(id);

                    A.CallTo(_runner)
                        .Where(call =>
                            call.Method.Name == "Run" &&
                            call.GetArgument<QueryBlogsById>(0).Id == id
                        )
                        .MustHaveHappened();                        
                }

                [TestMethod]
                public async Task Returns404WhenNothingFound()
                {
                    var id = 0;
                    var fakeBlog = (Blog)null;
                    A.CallTo(_runner)
                        .Method("Run")
                        .Returns(Task.FromResult(fakeBlog));

                    var result = await _controller.Get(id);

                    Assert.IsInstanceOfType(result, typeof(NotFoundResult));
                }
            }
        }
    }

    public static class FakeHelp
    {
        public static IAnyCallConfigurationWithReturnTypeSpecified<object> Method(this IAnyCallConfigurationWithNoReturnTypeSpecified fakeCall, string methodName)
        {
            return fakeCall
                .Where(call => call.Method.Name == methodName)
                .WithNonVoidReturnType();
        }

        public static Task<IEnumerable<T>> ToEnumTask<T>(this IEnumerable<T> enumerable)
        {
            return Task.FromResult(enumerable.AsEnumerable());
        }
    }
}

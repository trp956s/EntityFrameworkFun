using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Controllers;
using FakeItEasy;
using FakeItEasy.Configuration;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Data.Queries.BlogPersistanceLayer;
using WebApplication1.Data.Injectors;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Core;
using WebApplication1.Data.Upserts;
using System;

namespace WebApplication1.Test.Controllers
{
    [TestClass]
    public class BlogControllerTest
    {
        private BlogController _controller;
        private BlogDbSetInjector _blogContext;
        private IAsyncExecutableRunner _runner;

        [TestInitialize]
        public void Initialize()
        {
            _runner = A.Fake<IAsyncExecutableRunner>();
            _blogContext = A.Fake<BlogDbSetInjector>(options => options.WithArgumentsForConstructor(new object[] { null }));            
            _controller = new BlogController(_runner, _blogContext);

            //override default return of this method
            A.CallTo(() =>
                _runner.Run(
                    A<QueryBlogsById>.Ignored,
                    A<DbSetInjection<Blog>>.Ignored
                )
            ).Returns((Blog)null);
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

        [TestClass]
        public class Post : BlogControllerTest
        {
            [TestMethod]
            public async Task Returns404WhenNothingSent()
            {
                var result = await _controller.Post(null);

                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

            [TestMethod]
            public async Task ReturnsOKWhenUpdateRecordNotFound()
            {
                var blogToUpdate = new Blog();
                var blogInDatabase = new Blog();

                var result = await _controller.Post(blogToUpdate);

                Assert.IsInstanceOfType(result, typeof(OkResult));
            }

            [TestMethod]
            public async Task Returns400WhenSomethingFound()
            {
                var blogToUpdate = new Blog();
                var blogInDatabase = blogToUpdate;

                A.CallTo(() =>
                    _runner.Run(
                        A<QueryBlogsById>.Ignored,
                        A<DbSetInjection<Blog>>.Ignored
                    )
                )
                .Returns(blogInDatabase);

                var result = await _controller.Post(blogToUpdate);

                Assert.IsInstanceOfType(result, typeof(BadRequestResult));

            }

            [TestMethod]
            public async Task LooksForTheRightId()
            {
                var expectedId = 9;

                var result = await _controller.Post(new Blog() { Id = expectedId });

                A.CallTo(() =>
                    _runner.Run(
                        A<QueryBlogsById>.That.Matches(arg => arg.Id == expectedId),
                        A<DbSetInjection<Blog>>.Ignored
                    )
                )
                .MustHaveHappenedOnceExactly();
            }

            [TestMethod]
            public async Task ReturnsInsertFailure()
            {
                var expectedException = new Exception();

                A.CallTo(() =>
                    _runner.Run<IUpsertDbSet<Blog>, int>(
                        A<InsertBlog>.Ignored,
                        A<UpserterInjection<Blog>>.Ignored
                    )
                )
                .Throws(expectedException);
                ;

                var actualResult = await Assert.ThrowsExceptionAsync<Exception>(() =>
                    _controller.Post(new Blog())
                );


                Assert.AreEqual(actualResult, expectedException);
            }

            [TestMethod]
            public async Task UsesBlogAndBlogContext()
            {
                var expectedBlog = new Blog();
                var result = _controller.Post(expectedBlog);

                A.CallTo(() =>
                    _runner.Run(
                        A<QueryBlogsById>.Ignored,
                        A<DbSetInjection<Blog>>.That.Matches(arg =>
                            arg.DbSetWrapper == _blogContext
                        )
                    )
                ).MustHaveHappenedOnceExactly();

                A.CallTo(() =>
                    _runner.Run<IUpsertDbSet<Blog>, int>(
                        A<InsertBlog>.That.Matches(arg =>
                            arg.BlogToInsert == expectedBlog
                        ),
                        A<UpserterInjection<Blog>>.That.Matches(arg =>
                            arg.Dependency == _blogContext
                        )
                    )
                ).MustHaveHappenedOnceExactly();
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

using ExecutionStrategyCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Data.Models;
using System.Linq;
using FakeItEasy;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WebApplication1.Data.Queries;
using Microsoft.AspNetCore.Http;
using System;
using WebApplication1.Data;
using FakeItEasy.Configuration;
using WebApplication1.Data.GeneralInterfaces;
using System.Linq.Expressions;

namespace WebApplication1.Test.Controllers
{
    [TestClass]
    public class BlogControllerTest
    {
        BlogController blogController;
        ITaskRunner runner;

        [TestInitialize]
        public void TestInit()
        {
            runner = A.Fake<ITaskRunner>(options => options.Wrapping(new ExecutionStrategyRunner()));
            blogController = new BlogController(runner, null);
        }

        [TestClass]
        public class GetAll : BlogControllerTest
        {
            [TestMethod]
            public async Task ReturnsNotFound()
            {
                var getResult = await blogController.Get();

                Assert.IsInstanceOfType(getResult, typeof(NotFoundResult));
            }

            [TestClass]
            public class OverrideStory : GetAll
            {
                private FakeActiveStoryFactory activeStories;
                private StoryOverrideRunner runnerWrapper;
                private BlogDbSetRunner dbSet;

                [TestInitialize]
                public void TestInitialize()
                {
                    dbSet = A.Fake<BlogDbSetRunner>();
                    activeStories = new FakeActiveStoryFactory();
                    runnerWrapper = new StoryOverrideRunner(runner, activeStories);
                    blogController = new BlogController(runnerWrapper, dbSet);
                }

                [TestMethod]
                public async Task ReturnsAnEmptyArray()
                {
                    activeStories.ActiveStory = "1";

                    var getResult = await blogController.Get();

                    Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));

                    var resultValue = ((OkObjectResult)getResult).Value;
                    Assert.IsInstanceOfType(resultValue, typeof(Blog[]));
                }

                [TestMethod]
                public async Task ReturnsASingleItemArray()
                {
                    activeStories.ActiveStory = "2";

                    var getResult = await blogController.Get();

                    Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));

                    var resultValue = (Blog[])((OkObjectResult)getResult).Value;
                    Assert.AreEqual(resultValue.Count(), 1);
                    CollectionAssert.AllItemsAreNotNull(resultValue);
                }

                [TestMethod]
                public async Task ReturnsAnArrayFromQuery()
                {
                    activeStories.ActiveStory = "3";
                    var fakeBlogs = new Collection<Blog> { new Blog() };

                    A.CallTo(runner).Where(x => true).
                        WithReturnType<Task<IEnumerable<Blog>>>().
                        Returns(Task.FromResult(fakeBlogs.AsEnumerable()));

                    var getResult = await blogController.Get();

                    Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));

                    var resultValue = (IEnumerable<Blog>)((OkObjectResult)getResult).Value;
                    CollectionAssert.AreEquivalent(fakeBlogs, new Collection<Blog>(resultValue.ToList()));
                }

                [TestMethod]
                public async Task ReturnsAnArrayFromQuery2()
                {
                    activeStories.ActiveStory = "3";
                    var fakeBlogs = new Collection<Blog> { new Blog() };

                    var getAll = new GetAll<Blog>();
                    var getAllRunner = getAll.ToRunner(dbSet);
                    A.CallTo(() => runner.Run(getAllRunner)).
                        Returns(Task.FromResult(fakeBlogs.AsEnumerable()));

                    var getResult = await blogController.Get();

                    Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));

                    var resultValue = (IEnumerable<Blog>)((OkObjectResult)getResult).Value;
                    CollectionAssert.AreEquivalent(fakeBlogs, new Collection<Blog>(resultValue.ToList()));
                }

                [TestMethod]
                public async Task ReturnsNotFoundWhenNoBlogs()
                {
                    activeStories.ActiveStory = "4";

                    var getAll = new GetAll<Blog>();
                    var getAllRunner = getAll.ToRunner(dbSet);
                    A.CallTo(() => runner.Run(getAllRunner)).
                        Returns(Task.FromResult(Enumerable.Empty<Blog>()));

                    var getResult = await blogController.Get();

                    Assert.IsInstanceOfType(getResult, typeof(NotFoundResult));
                    A.CallTo(() => runner.Run(getAllRunner)).
                        MustHaveHappenedOnceExactly();
                }

            }
        }

        [TestClass]
        public class GetOne : BlogControllerTest
        {
            private FakeActiveStoryFactory activeStories;
            private StoryOverrideRunner runnerWrapper;
            private BlogDbSetRunner dbSet;

            [TestInitialize]
            public void TestInitialize()
            {
                dbSet = A.Fake<BlogDbSetRunner>();
                activeStories = new FakeActiveStoryFactory();
                runnerWrapper = new StoryOverrideRunner(runner, activeStories);
                blogController = new BlogController(runnerWrapper, dbSet);
            }

            [TestMethod]
            public async Task ReturnsNotFound()
            {
                var getResult = await blogController.Get(0);

                Assert.IsInstanceOfType(getResult, typeof(NotFoundResult));
            }

            [TestMethod]
            public async Task ReturnsNotFoundWhenDbEmpty()
            {
                var id = 99;
                var mockDbResponse = (Blog)null;
                activeStories.ActiveStory = "5";

                var getAllRunner = A.CallTo(() =>
                    runner.Run(A<IUnwrappedMapRunner<Blog>>.Ignored)
                ).CallsFake(fake => A.CallTo(()=>
                    fake.MapAsync(
                        A<GetAllById<Blog>>.Ignored,
                        A<IRunner<IQueryable<Blog>>>.Ignored
                    )
                )).AndReturns(
                    Task.FromResult(mockDbResponse)
                );

                var getResult = await blogController.Get(id);

                Assert.IsInstanceOfType(getResult, typeof(NotFoundResult));
                getAllRunner.MustHaveHappenedOnceExactly();
            }

            [TestMethod]
            public async Task ReturnsOkWithValue()
            {
                var id = 99;
                var expectedBlog = new Blog();
                activeStories.ActiveStory = "6";

                A.CallTo(() => runner.Run(A<IUnwrappedMapRunner<Blog>>.Ignored)).ReturnsNewFake(fake =>
                {
                    A.CallTo(() => fake.MapAsync(
                        A<GetAllById<Blog>>.Ignored,
                        A<IRunner<IQueryable<Blog>>>.Ignored
                    )).Returns(Task.FromResult(expectedBlog));
                });

                var getResult = await blogController.Get(id);

                Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));
                Assert.AreSame(((OkObjectResult)getResult).Value, expectedBlog);
            }
        }

        [TestClass]
        public class Post : BlogControllerTest
        {
            private FakeActiveStoryFactory activeStories;
            private StoryOverrideRunner runnerWrapper;
            private BlogDbSetRunner dbSet;

            [TestInitialize]
            public void TestInitialize()
            {
                dbSet = A.Fake<BlogDbSetRunner>();
                activeStories = new FakeActiveStoryFactory();
                runnerWrapper = new StoryOverrideRunner(runner, activeStories);
                blogController = new BlogController(runnerWrapper, dbSet);
            }

            [TestMethod]
            public async Task ReturnsBadRequestWhenNull()
            {
                var getResult = await blogController.Post(null);

                Assert.IsInstanceOfType(getResult, typeof(BadRequestResult));
            }

            [TestMethod]
            public async Task ReturnsBadRequestWhenNullInStories()
            {
                //this would be better with test cases
                var stories = new string[] { "7", "8", "9" };
                foreach (var story in stories)
                {
                    activeStories.ActiveStory = story;
                    var getResult = await blogController.Post(null);

                    Assert.IsInstanceOfType(getResult, typeof(BadRequestResult));
                }
            }

            [TestMethod]
            public async Task ReturnsConflictWhenIdIsFound()
            {
                //this would be better with test cases
                var stories = new string[] { "7", "8", "9" };
                foreach (var story in stories)
                {
                    activeStories.ActiveStory = story;
                    var blog = new Blog();

                    IReturnValueArgumentValidationConfiguration<Task<Blog>> getByIdCall = null;
                    A.CallTo(() => runner.Run(A<IUnwrappedMapRunner<Blog>>.Ignored)).ReturnsNewFake(fake =>
                    {
                        getByIdCall = A.CallTo(() => fake.MapAsync(
                            A<GetAllById<Blog>>.Ignored,
                            A<IRunner<IQueryable<Blog>>>.Ignored
                        ));

                        getByIdCall.Returns(Task.FromResult(blog));
                    });

                    var result = await blogController.Post(blog);

                    Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
                    Assert.AreEqual(result.StatusCode, StatusCodes.Status409Conflict);
                    getByIdCall.MustHaveHappenedOnceExactly();
                }
            }

            [TestMethod]
            public async Task ReturnsSuccessWhenUnique()
            {
                var stories = new string[] { "8", "9" };
                foreach (var story in stories)
                {
                    var blog = new Blog();
                    activeStories.ActiveStory = story;
                    IReturnValueArgumentValidationConfiguration<Task<Blog>> getByIdCall = null;
                    A.CallTo(() => runner.Run(A<IUnwrappedMapRunner<Blog>>.Ignored)).ReturnsNewFake(fake =>
                    {
                        getByIdCall = A.CallTo(() => fake.MapAsync(
                            A<GetAllById<Blog>>.Ignored,
                            A<IRunner<IQueryable<Blog>>>.Ignored
                        ));

                        getByIdCall.Returns(Task.FromResult(((Blog)null)));
                    });

                    if (story == "9")
                    {
                        var createBlog = new CreateBlog(blog).ToRunner(dbSet);
                        A.CallTo(() => runner.Run(createBlog)).Returns(0);
                    }

                    var result = await blogController.Post(blog);

                    Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
                    Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
                    getByIdCall.MustHaveHappenedOnceExactly();
                }
            }

            [TestMethod]
            public async Task ReturnsErrorOccuredDuringInsert()
            {
                var expected = new System.Exception();
                var blog = new Blog();
                activeStories.ActiveStory = "9";
                A.CallTo(() => runner.Run(A<IUnwrappedMapRunner<Blog>>.Ignored)).ReturnsNewFake(fake =>
                {
                    A.CallTo(() => fake.MapAsync(
                        A<GetAllById<Blog>>.Ignored,
                        A<IRunner<IQueryable<Blog>>>.Ignored
                    )).Returns(Task.FromResult(((Blog)null)));
                });

                var createBlog = new CreateBlog(blog).ToRunner(dbSet);
                A.CallTo(() => runner.Run(createBlog)).Throws(
                    expected
                );

                var actual = await Assert.ThrowsExceptionAsync<System.Exception>(() =>
                    blogController.Post(blog)
                );

                Assert.AreSame(expected, actual);
            }
        }

        [TestClass]
        public class Put : BlogControllerTest
        {
            private FakeActiveStoryFactory activeStories;
            private StoryOverrideRunner runnerWrapper;
            private BlogDbSetRunner dbSet;

            [TestInitialize]
            public void TestInitialize()
            {
                dbSet = A.Fake<BlogDbSetRunner>();
                activeStories = new FakeActiveStoryFactory();
                runnerWrapper = new StoryOverrideRunner(runner, activeStories);
                blogController = new BlogController(runnerWrapper, dbSet);
            }

            [TestMethod]
            public async Task ReturnsNotFound()
            {
                var response = await blogController.Put(0, null);

                Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            }

            [TestMethod]
            public async Task ReturnsNotFoundWhenIdIsNotFound()
            {
                foreach (var story in new string[] { "10", "11" })
                {
                    activeStories.ActiveStory = story;

                    var id = 999;

                    A.CallTo(() => runner.Run(A<IUnwrappedMapRunner<Blog>>.Ignored)).ReturnsNewFake(fake =>
                    {
                        A.CallTo(() => fake.MapAsync(
                            A<GetAllById<Blog>>.Ignored,
                            A<IRunner<IQueryable<Blog>>>.Ignored
                        )).
                        Returns(Task.FromResult(((Blog)null)));
                    });

                    var response = await blogController.Put(id, null);
                    Assert.IsInstanceOfType(response, typeof(NotFoundResult));
                }
            }

            [TestMethod]
            public async Task ReturnsOKWhenIdIsFound()
            {
                activeStories.ActiveStory = "10";

                var id = 321;

                A.CallTo(() => runner.Run(A<IUnwrappedMapRunner<Blog>>.Ignored)).ReturnsNewFake(fakeBlogMapper=> { 
                    A.CallTo(() => fakeBlogMapper.MapAsync(
                        A<GetAllById<Blog>>.Ignored,
                        A<IRunner<IQueryable<Blog>>>.Ignored
                    )).Returns(Task.FromResult((new Blog())));
                });

                var response = await blogController.Put(id, null);

                Assert.IsInstanceOfType(response, typeof(OkResult));
            }

            [TestMethod]
            public async Task UpdatesFoundId()
            {
                activeStories.ActiveStory = "11";

                var id = 4513242;
                var putArg = new Blog();
                var editBlog = new Blog() { Id = id };
                var expected = 876;

                A.CallTo(() => runner.Run(A<IUnwrappedMapRunner<Blog>>.Ignored)).ReturnsNewFake(fakeBlogMapper=>
                {
                    A.CallTo(() => fakeBlogMapper.MapAsync(
                        A<GetAllById<Blog>>.Ignored,
                        A<IRunner<IQueryable<Blog>>>.Ignored
                    )).
                    Returns(Task.FromResult(editBlog));
                });

                var updateBlog = new UpdateBlog(editBlog, putArg).ToRunner(dbSet);
                A.CallTo(() => runner.Run(updateBlog))
                    .Returns(Task.FromResult(expected));

                var response = await blogController.Put(id, putArg);

                Assert.IsInstanceOfType(response, typeof(OkObjectResult));
                Assert.AreEqual(expected, ((OkObjectResult)response).Value);
            }
        }

        [TestClass]
        public class Delete : BlogControllerTest
        {
            private IReturnValueArgumentValidationConfiguration<Task<Blog>> lookupBlogByIdMock;
            private IReturnValueArgumentValidationConfiguration<Task<int>> deleteBlogMock;

            //todo: test support for WrappedParameters and "naked" parameters
            [TestInitialize]
            public void TestInitialize()
            {
                var fakeMapFactory = A.Fake<IMapRunnerFactory>(o=>o.Wrapping(new MapRunnerFactory(runner)));
                var fakeBlogMapper = A.Fake<IUnwrappedMapRunner<Blog>>();
                var fakeIntMapper = A.Fake<IAsyncMapRunner<int>>();

                A.CallTo(() => runner.Run(A<IMapRunnerFactory>.Ignored)).Returns(fakeMapFactory);
                A.CallTo(() => runner.Run(A<IUnwrappedMapRunner<Blog>>.Ignored)).Returns(fakeBlogMapper);
                A.CallTo(() => fakeMapFactory.CreateMapRunnerAsync<int>()).Returns(fakeIntMapper);

                lookupBlogByIdMock = A.CallTo(() => fakeBlogMapper.MapAsync(
                    A<GetAllById<Blog>>.Ignored,
                    A<IRunner<IQueryable<Blog>>>.Ignored
                ));

                deleteBlogMock = A.CallTo(() => fakeIntMapper.Map(
                    A<DeleteBlog>.Ignored,

                    A<IRunner<BloggingContext>>.Ignored
                ));

                //TODO:VERIFY THAT THE CORRECT dbset is used
                A.CallTo(() => runner.Run(A<IRunner<IQueryable<Blog>>>.Ignored)).Returns(null);
                A.CallTo(() => runner.Run(A<IRunner<BloggingContext>>.Ignored)).Returns(null);

                var dbSet = A.Fake<BlogDbSetRunner>();
                blogController = new BlogController(runner, dbSet);
            }

            [TestMethod]
            public async Task NotFoundReturnedWhenGetByIdReturnsNull()
            {
                var deleteId = 99;
                Blog noBlog = null;
                var lookupBlogByIdMockInvocations = lookupBlogByIdMock.CollectInvocations();
                lookupBlogByIdMock.Returns(noBlog);

                var result = await blogController.Delete(deleteId);

                Assert.IsInstanceOfType(result, typeof(NotFoundResult));

                lookupBlogByIdMock.MustHaveHappenedOnceExactly();
                Assert.AreEqual(
                    new GetAllById<Blog>(deleteId),
                    lookupBlogByIdMockInvocations.Argument(0,0)
                );
            }

            [TestMethod]
            public async Task OKReturnedWhenGetByIdReturnsBlogAndDelteSuccessful()
            {
                var mockedBlogFoundById = new Blog();
                var deleteBlogMockInvocations = deleteBlogMock.CollectInvocations();
                lookupBlogByIdMock.Returns(mockedBlogFoundById);
                deleteBlogMock.Returns(0);

                var result = await blogController.Delete(0);

                Assert.IsInstanceOfType(result, typeof(OkObjectResult));

                Assert.AreEqual(
                    new DeleteBlog(mockedBlogFoundById),
                    deleteBlogMockInvocations[0].Arguments[0]
                );
                deleteBlogMock.MustHaveHappenedOnceExactly();
            }

            [TestMethod]
            public async Task ThrowsWhenDeleteBlogThrows()
            {
                var fakeException = new Exception();
                lookupBlogByIdMock.Returns(new Blog());
                deleteBlogMock.Throws(fakeException);

                var result = await Assert.ThrowsExceptionAsync<Exception>(() =>
                   blogController.Delete(0)
                );

                Assert.AreSame(result, fakeException);
            }
        }
    }

    public static class IReturnValueArgumentValidationConfigurationExtensions
    {
        public static List<FakeItEasy.Core.IFakeObjectCall> CollectInvocations<T>(this IReturnValueArgumentValidationConfiguration<T> callTarget)
        {
            var callsMade = new List<FakeItEasy.Core.IFakeObjectCall>();
            callTarget.Invokes(callsMade.Add);

            return callsMade;
        }

        public static object Argument(this List<FakeItEasy.Core.IFakeObjectCall> list, int invocationCardinality, int argumentCardinality)
        {
            var callInvocation = list[invocationCardinality];
            return callInvocation.Arguments[argumentCardinality];
        }

        //PROMOTE THIS EXTENSION THIS THING IS HANDY!
        public static T ReturnsNewFake<T>(this IReturnValueArgumentValidationConfiguration<T> callConfig, Action<T> fakeConfiguration = null)
        {
            var fake = (fakeConfiguration == null) ? A.Fake<T>() : A.Fake<T>(optionsBuilder=>optionsBuilder.ConfigureFake(fakeConfiguration));
            callConfig.Returns(fake);
            return fake;
        }

        public static IReturnValueArgumentValidationConfiguration<T2> CallsFake<T, T2>(this IReturnValueArgumentValidationConfiguration<T> callConfig, Func<T, IReturnValueArgumentValidationConfiguration<T2>> useFake)
        {
            var fake = A.Fake<T>();
            callConfig.Returns(fake);
            return useFake(fake);
        }

        public static IReturnValueArgumentValidationConfiguration<T> AndReturns<T>(this IReturnValueArgumentValidationConfiguration<T> config, T value)
        {
            config.Returns(value);
            return config;
        }


    }

    public class FakeActiveStoryFactory : IRunner<ActiveStories>
    {
        public string ActiveStory {get;set;}
        public ActiveStories Run()
        {
            if(ActiveStory == null)
            {
                return new ActiveStories(new string[] { });
            }

            return new ActiveStories(new string[] { ActiveStory });
        }
    }
}
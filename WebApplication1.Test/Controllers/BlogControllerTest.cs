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
                        WithReturnType<Task<InternalValueCache<IEnumerable<Blog>>>>().
                        Returns(Task.FromResult(fakeBlogs.AsEnumerable().ToWrapper()));

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
                        Returns(Task.FromResult(fakeBlogs.AsEnumerable().ToWrapper()));

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
                        Returns(Task.FromResult(Enumerable.Empty<Blog>().ToWrapper()));

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
                var mockDbResponse = Task.FromResult(((Blog)null).ToWrapper());
                activeStories.ActiveStory = "5";

                var getAllRunner = new GetAllById<Blog>(id).ToRunner(dbSet);
                A.CallTo(() => runner.Run(getAllRunner)).
                    Returns(mockDbResponse);

                var getResult = await blogController.Get(id);

                Assert.IsInstanceOfType(getResult, typeof(NotFoundResult));
                A.CallTo(() => runner.Run(getAllRunner)).
                    MustHaveHappenedOnceExactly();
            }

            [TestMethod]
            public async Task ReturnsOkWithValue()
            {
                var id = 99;
                var expectedBlog = new Blog();
                activeStories.ActiveStory = "6";

                var getById = new GetAllById<Blog>(id).ToRunner(dbSet);
                A.CallTo(() => runner.Run(getById)).
                    Returns(Task.FromResult(expectedBlog.ToWrapper()));

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
                var expectedCount = 1;
                foreach (var story in stories)
                {
                    activeStories.ActiveStory = story;
                    var blog = new Blog();

                    var getById = new GetAllById<Blog>(blog.Id).ToRunner(dbSet);
                    var getByIdCall = A.CallTo(() => runner.Run(getById));
                    getByIdCall.Returns(Task.FromResult(blog.ToWrapper()));

                    var result = await blogController.Post(blog);

                    Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
                    Assert.AreEqual(result.StatusCode, StatusCodes.Status409Conflict);
                    getByIdCall.MustHaveHappened(expectedCount++, Times.Exactly);
                }
            }

            [TestMethod]
            public async Task ReturnsSuccessWhenUnique()
            {
                var stories = new string[] { "8", "9" };
                var expectedCount = 1;
                foreach (var story in stories)
                {
                    var blog = new Blog();
                    activeStories.ActiveStory = story;
                    var getById = new GetAllById<Blog>(blog.Id).ToRunner(dbSet);
                    var getByIdCall = A.CallTo(() => runner.Run(getById));
                    getByIdCall.Returns(Task.FromResult(((Blog)null).ToWrapper()));

                    if (story == "9")
                    {
                        var createBlog = new CreateBlog(blog).ToRunner(dbSet);
                        A.CallTo(() => runner.Run(createBlog)).Returns(0.ToWrapper());
                    }

                    var result = await blogController.Post(blog);

                    Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
                    Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
                    getByIdCall.MustHaveHappened(expectedCount++, Times.Exactly);
                }
            }

            [TestMethod]
            public async Task ReturnsErrorOccuredDuringInsert()
            {
                var expected = new System.Exception();
                var blog = new Blog();
                activeStories.ActiveStory = "9";
                var getById = new GetAllById<Blog>(blog.Id).ToRunner(dbSet);
                var createBlog = new CreateBlog(blog).ToRunner(dbSet);
                A.CallTo(() => runner.Run(getById)).Returns(
                    Task.FromResult(((Blog)null).ToWrapper())
                );
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

                    var getById = new GetAllById<Blog>(id).ToRunner(dbSet);
                    A.CallTo(() => runner.Run(getById)).
                        Returns(Task.FromResult(((Blog)null).ToWrapper()));

                    var response = await blogController.Put(id, null);
                    Assert.IsInstanceOfType(response, typeof(NotFoundResult));
                }
            }

            [TestMethod]
            public async Task ReturnsOKWhenIdIsFound()
            {
                activeStories.ActiveStory = "10";

                var id = 321;

                var getById = new GetAllById<Blog>(id).ToRunner(dbSet);
                A.CallTo(() => runner.Run(getById)).
                    Returns(Task.FromResult((new Blog()).ToWrapper()));

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

                var getById = new GetAllById<Blog>(id).ToRunner(dbSet);
                A.CallTo(() => runner.Run(getById)).
                    Returns(Task.FromResult(editBlog.ToWrapper()));
                var updateBlog = new UpdateBlog(editBlog, putArg).ToRunner(dbSet);
                A.CallTo(() => runner.Run(updateBlog))
                    .Returns(Task.FromResult(expected.ToWrapper()));

                var response = await blogController.Put(id, putArg);

                Assert.IsInstanceOfType(response, typeof(OkObjectResult));
                Assert.AreEqual(expected, ((OkObjectResult)response).Value);
            }
        }

        [TestClass]
        public class Delete : BlogControllerTest
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
                var deleteId = 897;
                var result = await blogController.Delete(deleteId);

                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }

            public async Task ReturnsNotFoundAfterLookupFails()
            {
                var deleteId = 4325;
                foreach (var story in new string[] { null, "12", "13" })
                {
                    activeStories.ActiveStory = story;
                    A.CallTo(() => runner.Run(
                        new GetAllById<Blog>(deleteId).ToRunner(dbSet))
                    ).Returns(((Blog)null).ToWrapper());

                    var result = await blogController.Delete(deleteId);

                    Assert.IsInstanceOfType(result, typeof(NotFoundResult));
                }
            }

            [TestMethod]
            public async Task ReturnsOkWhenIdFound()
            {
                var deleteId = 3421;

                foreach (var story in new string[] { "12" })
                {
                    activeStories.ActiveStory = story;
                    A.CallTo(() => runner.Run(
                        new GetAllById<Blog>(deleteId).ToRunner(dbSet))
                    ).Returns(new Blog().ToWrapper());

                    var result = await blogController.Delete(deleteId);

                    Assert.IsInstanceOfType(result, typeof(OkResult));
                }
            }

            [TestMethod]
            public async Task ReturnsOkWhenIdFoundAndDeleteSuccess()
            {
                var deleteId = 3421;
                var deleteEntity = new Blog();
                foreach (var story in new string[] { "12" })
                {
                    activeStories.ActiveStory = story;
                    A.CallTo(() => runner.Run(
                        new GetAllById<Blog>(deleteId).ToRunner(dbSet))
                    ).Returns(deleteEntity.ToWrapper());
                    A.CallTo(() => runner.Run(
                        new DeleteBlog(deleteEntity).ToRunner(dbSet))
                    ).Returns(0.ToWrapper());

                    var result = await blogController.Delete(deleteId);

                    Assert.IsInstanceOfType(result, typeof(OkResult));
                }
            }


            [TestMethod]
            public async Task ReturnsErrorWhenDeleteBlowsUp()
            {
                var deleteId = 546;
                var deleteEntity = new Blog();
                var expectedException = new Exception("Test");
                activeStories.ActiveStory = "13";
                A.CallTo(() => runner.Run(
                    new GetAllById<Blog>(deleteId).ToRunner(dbSet))
                ).Returns(deleteEntity.ToWrapper());
                A.CallTo(() => runner.Run(
                    new DeleteBlog(deleteEntity).ToRunner(dbSet))
                ).Throws(expectedException);

                var excptionThrown = await Assert.ThrowsExceptionAsync<Exception>(() => blogController.Delete(deleteId));

                Assert.AreSame(excptionThrown, expectedException);
            }
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
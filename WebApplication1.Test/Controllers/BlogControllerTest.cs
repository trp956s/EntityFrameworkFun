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

namespace WebApplication1.Test.Controllers
{
    [TestClass]
    public class BlogControllerTest
    {
        BlogController blogController;
        ITaskRunner runner;

        [TestInitialize]
        public void TestInit() {
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

                    A.CallTo(runner).Where(x=>true).
                        WithReturnType<Task<InternalRunnerWrapper<IEnumerable<Blog>>>>().
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
        public class Get : BlogControllerTest
        {
            [TestMethod]
            public async Task ReturnsNotFound()
            {
                var getResult = await blogController.Get(0);

                Assert.IsInstanceOfType(getResult, typeof(NotFoundResult));
            }
        }

        [TestClass]
        public class Post : BlogControllerTest
        {
            [TestMethod]
            public async Task ReturnsNotFound()
            {
                var getResult = await blogController.Post(null);

                Assert.IsInstanceOfType(getResult, typeof(BadRequestResult));
            }
        }
    }

    public class FakeActiveStoryFactory : IRunner<ActiveStories>
    {
        public string ActiveStory {get;set;}
        public ActiveStories Run()
        {
            return new ActiveStories(new string[] { ActiveStory });
        }
    }
}
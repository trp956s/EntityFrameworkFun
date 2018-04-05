using ExecutionStrategyCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Data.Models;
using System.Linq;
using WebApplication1.Test.Helpers;
using FakeItEasy;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data.Queries;
using System;

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
                private ActiveStoryFactory activeStories;
                private StoryOverrideRunner runnerWrapper;

                [TestInitialize]
                public void TestInitialize()
                {
                    activeStories = new ActiveStoryFactory();
                    runnerWrapper = new StoryOverrideRunner(runner, activeStories);
                    blogController = new BlogController(runnerWrapper, null);
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

                    A.CallTo(() => runner.Run(A<IRunner<Task<InternalRunnerWrapper<IEnumerable<Blog>>>>>.Ignored)).
                        Returns(Task.FromResult(fakeBlogs.AsEnumerable().ToWrapper()));

                    var getResult = await blogController.Get();

                    Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));

                    var resultValue = (IEnumerable<Blog>)((OkObjectResult)getResult).Value;
                    CollectionAssert.AreEquivalent(fakeBlogs, new Collection<Blog>(resultValue.ToList()));
                }

                [TestMethod]
                public async Task SearchesAgainstBlogDbSet()
                {
                    //    var expectedDbSetWrapper = A.Fake<DbSetWrapper<Blog>>();
                    //    var expectedStrategySource = new GetAll<Blog>(expectedDbSetWrapper);
                    //    var runner = A.Fake<IExecutionStrategyRunner>(optionsBuilder => optionsBuilder.
                    //        Wrapping(runnerWrapper.CreateStoryRunner("3"))
                    //    );
                    //    blogController = new BlogController(runner, expectedDbSetWrapper);
                    //    A.CallTo(() => runner.Run(A<ExecutionStrategy<IEnumerable<Blog>>>.Ignored)).
                    //        Returns(Task.FromResult(Enumerable.Empty<Blog>()));

                    //    var getResult = await blogController.Get();

                    //    A.CallTo(() => runner.Run(A<ExecutionStrategy<IEnumerable<Blog>>>.That.
                    //        Matches(s=>
                    //            s.Source.Equals(expectedStrategySource)))
                    //        ).
                    //        MustHaveHappenedOnceExactly();
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

    public class ActiveStoryFactory : IRunner<ActiveStories>
    {
        public string ActiveStory {get;set;}
        public ActiveStories Run()
        {
            return new ActiveStories(new string[] { ActiveStory });
        }
    }

    public class StoryRunnerWrapper
    {
        public StoryRunnerWrapper()
        {
            Runner = new ExecutionStrategyRunner();
        }

        public ExecutionStrategyRunner Runner { get; }
        public IExecutionStrategyRunner CreateStoryRunner(params string[] stories)
        {
            var activeStories = new ActiveStories(stories);
            var storyExecutionStrategyRunner = new StoryExecutionStrategyRunner(activeStories, Runner);
            var fakeRunner = A.Fake<IExecutionStrategyRunner>(optionsBuilder => optionsBuilder.
                    Wrapping(storyExecutionStrategyRunner)
            );

            return fakeRunner;
        }
    }
}
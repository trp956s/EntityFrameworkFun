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

namespace WebApplication1.Test.Controllers
{
    [TestClass]
    public class BlogControllerTest
    {
        BlogController blogController;
        StoryRunnerWrapper runnerWrapper;

        [TestInitialize]
        public void Initialize() {
            runnerWrapper = new StoryRunnerWrapper();
            blogController = new BlogController(runnerWrapper.Runner, null);
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

            [TestMethod]
            public async Task ReturnsAnEmptyArray()
            {
                blogController = new BlogController(runnerWrapper.CreateStoryRunner("1"), null);

                var getResult = await blogController.Get();

                Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));

                var resultValue = ((OkObjectResult)getResult).Value;
                Assert.IsInstanceOfType(resultValue, typeof(Blog[]));
            }

            [TestMethod]
            public async Task ReturnsASingleItemArray()
            {
                blogController = new BlogController(runnerWrapper.CreateStoryRunner("2"), null);

                var getResult = await blogController.Get();

                Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));

                var resultValue = (Blog[])((OkObjectResult)getResult).Value;
                Assert.AreEqual(resultValue.Count(), 1);
                CollectionAssert.AllItemsAreNotNull(resultValue);
            }

            [TestMethod]
            public async Task ReturnsAnArrayFromQuery()
            {
                var runner = A.Fake<IExecutionStrategyRunner>(optionsBuilder => optionsBuilder.
                    Wrapping(runnerWrapper.CreateStoryRunner("3"))
                );
                var fakeBlogs = new Collection<Blog> { new Blog() };
                var fakeBlogsTask = Task.FromResult(fakeBlogs.AsEnumerable());
                A.CallTo(() => runner.Run(A<ExecutionStrategy<IEnumerable<Blog>>>.Ignored)).
                    Returns(fakeBlogsTask);
                blogController = new BlogController(runner, null);

                var getResult = await blogController.Get();

                Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));

                var resultValue = (IEnumerable<Blog>)((OkObjectResult)getResult).Value;
                CollectionAssert.AreEquivalent(fakeBlogs, new Collection<Blog>(resultValue.ToList()));
            }

            [TestMethod]
            public async Task SearchesAgainstBlogDbSet()
            {
                var expectedDbSetWrapper = A.Fake<DbSetWrapper<Blog>>();
                var expectedStrategySource = new GetAll<Blog>(expectedDbSetWrapper);
                var runner = A.Fake<IExecutionStrategyRunner>(optionsBuilder => optionsBuilder.
                    Wrapping(runnerWrapper.CreateStoryRunner("3"))
                );
                blogController = new BlogController(runner, expectedDbSetWrapper);
                A.CallTo(() => runner.Run(A<ExecutionStrategy<IEnumerable<Blog>>>.Ignored)).
                    Returns(Task.FromResult(Enumerable.Empty<Blog>()));

                var getResult = await blogController.Get();

                A.CallTo(() => runner.Run(A<ExecutionStrategy<IEnumerable<Blog>>>.That.
                    Matches(s=>
                        s.Source.Equals(expectedStrategySource)))
                    ).
                    MustHaveHappenedOnceExactly();
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

    public class StoryRunnerWrapper
    {
        public StoryRunnerWrapper()
        {
            Runner = new ExecutionStrategyRunner();
        }

        public ExecutionStrategyRunner Runner { get; }
        public StoryExecutionStrategyRunner CreateStoryRunner(params string[] stories)
        {
            var activeStories = new ActiveStories(stories);
            return new StoryExecutionStrategyRunner(activeStories, Runner);
        }
    }
}
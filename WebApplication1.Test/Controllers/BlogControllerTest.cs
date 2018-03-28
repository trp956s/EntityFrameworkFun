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
        ExecutionStrategyRunner runner;
        BlogController blogController;

        [TestInitialize]
        public void Initialize() {
            runner = new ExecutionStrategyRunner();
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

            [TestMethod]
            public async Task ReturnsAnEmptyArray()
            {
                var stories = new ActiveStories(new string[] { "1" });
                var storyRunner = new StoryExecutionStrategyRunner(stories, runner);
                blogController = new BlogController(storyRunner, null);

                var getResult = await blogController.Get();

                Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));

                var resultValue = ((OkObjectResult)getResult).Value;
                Assert.IsInstanceOfType(resultValue, typeof(Blog[]));
            }

            [TestMethod]
            public async Task ReturnsASingleItemArray()
            {
                var stories = new ActiveStories(new string[] { "2" });
                var storyRunner = new StoryExecutionStrategyRunner(stories, runner);
                blogController = new BlogController(storyRunner, null);

                var getResult = await blogController.Get();

                Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));

                var resultValue = (Blog[])((OkObjectResult)getResult).Value;
                Assert.AreEqual(resultValue.Count(), 1);
                CollectionAssert.AllItemsAreNotNull(resultValue);
            }

            [TestMethod]
            public async Task ReturnsAnArrayFromQuery()
            {
                var stories = new ActiveStories(new string[] { "3" });
                var storyRunner = A.Fake<IExecutionStrategyRunner>(optionsBuilder => optionsBuilder.
                    Wrapping(new StoryExecutionStrategyRunner(stories, runner))
                );
                var fakeBlogs = new Collection<Blog> { new Blog() };
                var fakeBlogsTask = Task.FromResult(fakeBlogs.AsEnumerable());
                A.CallTo(() => storyRunner.Run(A<ExecutionStrategy<IEnumerable<Blog>>>.Ignored)).
                    Returns(fakeBlogsTask);
                blogController = new BlogController(storyRunner, null);

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
                var stories = new ActiveStories(new string[] { "3" });
                var storyRunner = A.Fake<IExecutionStrategyRunner>(optionsBuilder => optionsBuilder.
                    Wrapping(new StoryExecutionStrategyRunner(stories, runner))
                );
                blogController = new BlogController(storyRunner, expectedDbSetWrapper);
                A.CallTo(() => storyRunner.Run(A<ExecutionStrategy<IEnumerable<Blog>>>.Ignored)).
                    Returns(Task.FromResult(Enumerable.Empty<Blog>()));

                var getResult = await blogController.Get();

                A.CallTo(() => storyRunner.Run(A<ExecutionStrategy<IEnumerable<Blog>>>.That.
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
}
using ExecutionStrategyCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Data.Models;

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
            blogController = new BlogController(runner);
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
                var stories = new ActiveStories(new string[]{"1"});
                var storyRunner = new StoryExecutionStrategyRunner(stories, runner);
                blogController = new BlogController(storyRunner);

                var getResult = await blogController.Get();

                Assert.IsInstanceOfType(getResult, typeof(OkObjectResult));

                var resultValue = ((OkObjectResult)getResult).Value;
                Assert.IsInstanceOfType(resultValue, typeof(Blog[]));
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
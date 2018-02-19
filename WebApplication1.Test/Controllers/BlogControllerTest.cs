using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Controllers;
using FakeItEasy;
using FakeItEasy.Configuration;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Data.Models;
using System.Threading.Tasks;

namespace WebApplication1.Test.Controllers
{
    [TestClass]
    public class BlogControllerTest
    {
        private BlogController _controller;
        private IQueryRunner _queryRunner;
        [TestInitialize]
        public void Initialize()
        {
            _queryRunner = A.Fake<IQueryRunner>();
            _controller = new BlogController(_queryRunner);
        }

        [TestClass]
        public class Get : BlogControllerTest
        {
            [TestClass]
            public class WithId : Get
            {
                [TestMethod]
                public async Task ReturnsOkWithQueryResult()
                {
                    var id = 0;
                    var fakeBlog = A.Fake<Blog>();
                    A.CallTo(_queryRunner)
                        .Method("Run")
                        .Returns(Task.FromResult(fakeBlog));
                    
                    var result = await _controller.Get(id);
                    
                    Assert.IsInstanceOfType(result, typeof(OkObjectResult));
                    Assert.AreEqual(fakeBlog, result.Value);
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
    }
}

using ExecutionStrategyCore;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.Queries;
using WebApplication1.Data.Test.Helpers;

namespace WebApplication1.Data.Test.Queries
{
    [TestClass]
    public class GetAllTest
    {
        GetAll<object> getAllOfAnything;
        ExecutionStrategyRunner runner;

        [TestInitialize]
        public void TestInitialize()
        {
            getAllOfAnything = new GetAll<object>();
            runner = new ExecutionStrategyRunner();
        }

        [TestClass]
        public class Run : GetAllTest
        {
            [TestMethod]
            public async Task ReturnsNothingAsynchronouslyWhenListEmpty()
            {
                var testList = new Collection<object>();
                var queryableEntity = FakeIAsyncEnumFactory.CreateFakeIAsyncEnum(testList, testList.AsQueryable());

                var result = runner.Run(await getAllOfAnything.Run(queryableEntity));

                Assert.AreEqual(0, result.Count());
            }

            [TestMethod]
            public async Task ReturnsAllOfQuerySourceInOrder()
            {
                var testList = new Collection<object> { new object(), new object(), new object() };
                var queryableEntity = FakeIAsyncEnumFactory.CreateFakeIAsyncEnum(testList, testList.AsQueryable());

                var result = runner.Run(await getAllOfAnything.Run(queryableEntity));

                CollectionAssert.AreEquivalent(testList, new Collection<object>(result.ToList()));
                for (var i = 0; i < testList.Count(); i++)
                {
                    Assert.AreEqual(testList.ElementAt(i), result.ElementAt(i));
                }
            }
        }
    }
}

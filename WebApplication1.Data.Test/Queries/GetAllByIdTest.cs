using ExecutionStrategyCore;
using FakeItEasy;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Data.ModelInterfaces;
using WebApplication1.Data.Queries;
using WebApplication1.Data.Test.Helpers;

namespace WebApplication1.Data.Test.Queries
{
    [TestClass]
    public class GetAllByIdTest
    {
        ExecutionStrategyRunner runner;

        [TestInitialize]
        public void TestInitialize()
        {
            runner = new ExecutionStrategyRunner();
        }

        [TestClass]
        public class Run : GetAllByIdTest
        {
            [TestMethod]
            public async Task ReturnsNullAsynchronouslyWhenListEmpty()
            {
                var getById = new GetAllById<IHasId>();
                var testList = new Collection<IHasId>();
                var queryableEntity = FakeIAsyncEnumFactory.CreateFakeIAsyncEnum(testList, testList.AsQueryable());

                var result = runner.Run(await getById.Run(queryableEntity));

                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ReturnsNullAsynchronouslyWhenNoIdsMatch()
            {
                var uniqueId = 1234;
                var getById = new GetAllById<IHasId>(uniqueId);
                var testList = new Collection<IHasId> { A.Fake<IHasId>() };
                var queryableEntity = FakeIAsyncEnumFactory.CreateFakeIAsyncEnum(testList, testList.AsQueryable());
                A.CallTo(() => testList[0].Id).Returns(uniqueId + 1);

                var result = runner.Run(await getById.Run(queryableEntity));

                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ReturnsFirstItemWIthMatchingId()
            {
                var matchingId = 1234;
                var getById = new GetAllById<IHasId>(matchingId);
                var expectedElement = A.Fake<IHasId>();
                var unexpectedElement = A.Fake<IHasId>();
                var testList = new Collection<IHasId> { A.Fake<IHasId>(), expectedElement, unexpectedElement, A.Fake<IHasId>() };
                var queryableEntity = FakeIAsyncEnumFactory.CreateFakeIAsyncEnum(testList, testList.AsQueryable());
                A.CallTo(() => expectedElement.Id).Returns(matchingId);
                A.CallTo(() => unexpectedElement.Id).Returns(matchingId);

                var result = runner.Run(await getById.Run(queryableEntity));

                Assert.AreSame(expectedElement, result);
                Assert.AreNotSame(unexpectedElement, result);
            }
        }
    }
}

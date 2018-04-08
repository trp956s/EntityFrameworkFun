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

namespace WebApplication1.Data.Test.Queries
{
    [TestClass]
    public class GetAllTest
    {
        GetAll<object> getAllOfAnything;

        [TestInitialize]
        public void TestInitialize()
        {
            getAllOfAnything = new GetAll<object>();
        }

        [TestClass]
        public class Run : GetAllTest
        {
            [TestMethod]
            public async Task ReturnsNothingAsynchronouslyWhenListEmpty()
            {
                var testList = new Collection<object>();

                //todo move the IasyncEnumerable stuff into a helper
                var dataEnum = testList.GetEnumerator();
                var runner = new ExecutionStrategyRunner();
                var queryableEntity = A.Fake<IQueryable<object>>(optionsBuilder =>
                {
                    optionsBuilder.Implements<IAsyncEnumerable<object>>();
                    optionsBuilder.Wrapping(testList.AsQueryable());
                }
                );
                var asyncEnum = A.Fake<IAsyncEnumerator<object>>();
                A.CallTo(queryableEntity).Where(call => call.Method.Name == "GetEnumerator").
                    WithReturnType<IAsyncEnumerator<object>>().
                    Returns(asyncEnum);
                A.CallTo(asyncEnum).Where(call => call.Method.Name == "MoveNext").
                    WithReturnType<Task<System.Boolean>>().
                    ReturnsLazily(c =>
                        Task.FromResult(dataEnum.MoveNext())
                    );
                A.CallTo(() => asyncEnum.Current).
                    ReturnsLazily(c =>
                        dataEnum.Current
                    );

                var wrappedQueryResult = await getAllOfAnything.Run(queryableEntity);

                var result = runner.Run(wrappedQueryResult);

                Assert.AreEqual(0, result.Count());
            }
        }
    }
}

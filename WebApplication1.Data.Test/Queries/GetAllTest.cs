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
                var queryableEntity = CreateFakeIAsyncEnum(testList, testList.AsQueryable());

                var result = runner.Run(await getAllOfAnything.Run(queryableEntity));

                Assert.AreEqual(0, result.Count());
            }

            [TestMethod]
            public async Task ReturnsAllOfQuerySource()
            {
                var testList = new Collection<object> { new object(), new object(), new object() };
                var queryableEntity = CreateFakeIAsyncEnum(testList, testList.AsQueryable());

                var result = runner.Run(await getAllOfAnything.Run(queryableEntity));

                CollectionAssert.AreEquivalent(testList, new Collection<object>(result.ToList()));
            }

            private FakeType CreateFakeIAsyncEnum<T, FakeType>(IEnumerable<T> dataToWrap, FakeType fakeToWrap)
            {
                var dataEnum = dataToWrap.GetEnumerator();
                var fake = A.Fake<FakeType>(optionsBuilder =>
                {
                    optionsBuilder.Implements<IAsyncEnumerable<object>>();
                    optionsBuilder.Wrapping(fakeToWrap);
                }
                );
                var asyncEnum = A.Fake<IAsyncEnumerator<object>>();
                A.CallTo(fake).Where(call => call.Method.Name == "GetEnumerator").
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

                return fake;
            }
        }
    }
}

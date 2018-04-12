using ExecutionStrategyCore;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.ModelInterfaces;
using WebApplication1.Data.Queries;

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
                var queryableEntity = CreateFakeIAsyncEnum(testList, testList.AsQueryable());

                var result = runner.Run(await getById.Run(queryableEntity));

                Assert.IsNull(result);
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

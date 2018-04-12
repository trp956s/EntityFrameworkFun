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

            [TestMethod]
            public async Task ReturnsNullAsynchronouslyWhenNoIdsMatch()
            {
                var uniqueId = 1234;
                var getById = new GetAllById<IHasId>(uniqueId);
                var testList = new Collection<IHasId> { A.Fake<IHasId>() };
                var queryableEntity = CreateFakeIAsyncEnum(testList, testList.AsQueryable());
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
                var queryableEntity = CreateFakeIAsyncEnum(testList, testList.AsQueryable());
                A.CallTo(() => expectedElement.Id).Returns(matchingId);
                A.CallTo(() => unexpectedElement.Id).Returns(matchingId);

                var result = runner.Run(await getById.Run(queryableEntity));

                Assert.AreSame(expectedElement, result);
                Assert.AreNotSame(unexpectedElement, result);
            }

            private FakeType CreateFakeIAsyncEnum<T, FakeType>(IEnumerable<T> dataToWrap, FakeType fakeToWrap)
            where FakeType : IQueryable<T>
            {
                var dataEnum = dataToWrap.GetEnumerator();
                var fake = A.Fake<FakeType>(optionsBuilder =>
                {
                    optionsBuilder.Implements<IAsyncEnumerable<T>>();
                    optionsBuilder.Wrapping(fakeToWrap);
                }
                );
                var asyncEnum = A.Fake<IAsyncEnumerator<T>>();
                var provider = A.Fake<IQueryProvider>(o=> 
                {
                    o.Implements<IAsyncQueryProvider>();
                    o.Wrapping(fakeToWrap.Provider);
                });
                A.CallTo(() => fake.Provider).Returns(provider);

                A.CallTo(fake).Where(call => 
                    call.Method.Name == "GetEnumerator"
                ).
                    WithReturnType<IAsyncEnumerator<T>>().
                    Returns(asyncEnum);
                A.CallTo(asyncEnum).Where(call => 
                    call.Method.Name == "MoveNext"
                ).
                    WithReturnType<Task<System.Boolean>>().
                    ReturnsLazily(c =>
                        Task.FromResult(dataEnum.MoveNext())
                    );
                A.CallTo(() => asyncEnum.Current).
                    ReturnsLazily(c =>
                        dataEnum.Current
                    );
                A.CallTo(()=>((IAsyncQueryProvider)provider).
                    ExecuteAsync<T>(A<Expression>.Ignored, A<CancellationToken>.Ignored)
                ).ReturnsLazily(call => {
                    return Task.FromResult(provider.Execute<T>((Expression)call.Arguments[0]));
                });

                return fake;
            }
        }
    }
}

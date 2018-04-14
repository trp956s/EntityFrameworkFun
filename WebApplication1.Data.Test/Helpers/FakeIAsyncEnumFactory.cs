using FakeItEasy;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.Data.Test.Helpers
{
    public static class FakeIAsyncEnumFactory
    {
        public static FakeType CreateFakeIAsyncEnum<T, FakeType>(IEnumerable<T> dataToWrap, FakeType fakeToWrap)
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
            var provider = A.Fake<IQueryProvider>(o =>
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
            A.CallTo(() => ((IAsyncQueryProvider)provider).
                ExecuteAsync<T>(A<Expression>.Ignored, A<CancellationToken>.Ignored)
            ).ReturnsLazily(call => {
                return Task.FromResult(provider.Execute<T>((Expression)call.Arguments[0]));
            });

            return fake;
        }
    }
}

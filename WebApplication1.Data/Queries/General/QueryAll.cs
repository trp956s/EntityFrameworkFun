﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data.Helpers;

namespace WebApplication1.Data.Queries.General
{
    public abstract class QueryAll<T, Context> : IQuery<T, Context, IEnumerable<T>>
    where T : class
    {
        public async Task<IEnumerable<T>> Execute(IAsyncEnumerable<T> queryable)
        {
            return await queryable.ToList();
        }

        public abstract IEnumerable<T> GetDataEnumerable(Context content);
    }
}
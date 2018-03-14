using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Data.Helpers
{
    public interface IUpsertDbSet<T>
    {
        Task AddAsync(T entityToTrack);
    }
}

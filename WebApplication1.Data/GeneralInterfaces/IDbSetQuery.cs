using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Data.GeneralInterfaces
{
    public interface IDbSetQuery<DbSetType, ReturnType> : IMapper<DbSet<DbSetType>, Task<InternalRunnerWrapper<ReturnType>>>
    where DbSetType : class
    { }
}

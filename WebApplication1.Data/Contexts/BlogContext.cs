using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;

public class BlogContext : IBlogContext
{
    public Task<Blog> Run(IExecutable<IAsyncEnumerable<Blog>, Blog> executable)
    {
        return executable.Execute(null);
    }
}
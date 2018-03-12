using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;

public class BlogContext : IDbSetWrapper<Blog>
{
    private readonly BloggingContext _context;

    public BlogContext(BloggingContext _context){
        this._context = _context;
    }
    public IEnumerable<Blog> DbSet => _context.Blogs;
}
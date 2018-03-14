using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Injectors
{
    public class BlogDbSetInjector : IDbSetWrapper<Blog>, IUpsertDbSet<Blog>
    {
        private readonly BloggingContext _context;

        public BlogDbSetInjector(BloggingContext _context)
        {
            this._context = _context;
        }

        public IEnumerable<Blog> DbSet => _context.Blogs;

        public async Task AddAsync(Blog entityToTrack)
        {
            await _context.Blogs.AddAsync(entityToTrack);
        }

        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
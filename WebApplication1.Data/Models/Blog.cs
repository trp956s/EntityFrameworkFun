using System.Collections.Generic;
using WebApplication1.Data.ModelInterfaces;

namespace WebApplication1.Data.Models
{
    public class Blog : IHasId
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Post> Posts { get; set; }

        public override string ToString()
        {
            return base.ToString() + " where id = " + Id.ToString() + " and name = " + (Name ?? "").ToString();
        }
    }
}

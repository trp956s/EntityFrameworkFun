using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplication1.Data.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Post> Posts { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewNewTry.Models;

namespace NewNewTry.Data
{
    public class NewNewTryNewContext : DbContext
    {
        public NewNewTryNewContext (DbContextOptions<NewNewTryNewContext> options)
            : base(options)
        {
        }

        public DbSet<NewNewTry.Models.Flower> Flower { get; set; }

        public DbSet<NewNewTry.Models.Payment> Payment { get; set; }

        public DbSet<NewNewTry.Models.Order> Order { get; set; }
    }
}

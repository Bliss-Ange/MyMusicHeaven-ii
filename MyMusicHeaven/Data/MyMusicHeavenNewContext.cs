using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyMusicHeaven.Models;

namespace MyMusicHeaven.Data
{
    public class MyMusicHeavenNewContext : DbContext
    {
        public MyMusicHeavenNewContext (DbContextOptions<MyMusicHeavenNewContext> options)
            : base(options)
        {
        }

        public DbSet<MyMusicHeaven.Models.Product> Product { get; set; }

        public DbSet<MyMusicHeaven.Models.Payment> Payment { get; set; }
    }
}

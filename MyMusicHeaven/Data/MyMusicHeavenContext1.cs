using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyMusicHeaven.Models;

namespace MyMusicHeaven.Data
{
    public class MyMusicHeavenContext1 : DbContext
    {
        public MyMusicHeavenContext1 (DbContextOptions<MyMusicHeavenContext1> options)
            : base(options)
        {
        }

        public DbSet<MyMusicHeaven.Models.Product> Product { get; set; }
    }
}

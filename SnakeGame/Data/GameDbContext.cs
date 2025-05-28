using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore; //стоит ли это использоавть?
using SnakeGame.Models;

namespace SnakeGame.Data
{
    public class GameDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=snakegame.db");
    }
}

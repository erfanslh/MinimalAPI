using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using MinimalAPIMoviez;
using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
          //******************************************
          //* Yeki dg az ravesh haye Data Annotation*
          //******************************************
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Actor>().Property(p => p.Name).HasMaxLength(150);
            modelBuilder.Entity<Actor>().Property(p => p.Imagename).IsUnicode();

        }

        public DbSet<Genre> genres { get; set; }
        public DbSet<Actor> actor { get; set; }
    }
}

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
        //  ******************************************
        //  * Yeki dg az ravesh haye Data Annotation *
        //  ******************************************
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Genre>().Property(p=> p.Name).HasMaxLength (50);
        //}

        public DbSet<Genre> genres { get; set; }
    }
}

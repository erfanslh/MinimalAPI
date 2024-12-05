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

            modelBuilder.Entity<Movie>().Property(p => p.Title).HasMaxLength(150);
            modelBuilder.Entity<Movie>().Property(p => p.CoverImage).IsUnicode();

            modelBuilder.Entity<GenreMovie>().HasKey(gm => new { gm.MovieId, gm.GenreId });
            modelBuilder.Entity<ActorMovie>().HasKey(am => new { am.MovieId, am.ActorId });
        }

        public DbSet<Genre> genres { get; set; }
        public DbSet<Actor> actors { get; set; }
        public DbSet<Movie> movies { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<GenreMovie> GenresMovies { get; set; }
        public DbSet<ActorMovie> ActorMovies { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using MinimalAPIMoviez;
using MinimalAPIMoviez.Entities;
using System.Security.Claims;

namespace MinimalAPIMoviez
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
          //******************************************
          //* Another way of Data Annotations ********
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
            
            #region Asp.net Core Identity Package

            // Table to store user accounts (like usernames, emails, passwords, etc.)
            modelBuilder.Entity<IdentityUser>().ToTable("Users");

            // Table to store roles like "Admin", "Moderator", or "User" that users can belong to.
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");

            // This table stores claims for roles.
            // Claims are custom attributes you assign to roles, like permissions("CanEdit" or "CanDelete").
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RolesClaims");

            // This Table stores additational information about a specific user.
            // For example, you can say "User X has the claim 'CanEditProfile'".
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsersClaims");

            // Table to store external login information
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UsersLogins");

            // This Table links users to roles
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UsersRoles");

            // Table for storing authentication tokens.
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsersTokens");

            #endregion
        }

        public DbSet<Genre> genres { get; set; }
        public DbSet<Actor> actors { get; set; }
        public DbSet<Movie> movies { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<GenreMovie> GenresMovies { get; set; }
        public DbSet<ActorMovie> ActorMovies { get; set; }
        public DbSet<Error> Errors { get; set; }
    }
}

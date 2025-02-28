using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace MinimalAPIsWithASPNetEF.Entities
{
    public class AppDbCtx: IdentityDbContext // originally using DbContext, and then changing to IdentityDbContext for authentication identities
    {
        public AppDbCtx() { }
        public AppDbCtx(DbContextOptions<AppDbCtx> options) : base(options) { 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning to protect potentially sentitive information in your connection string
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>().Property(p => p.Name).HasMaxLength(100);
            modelBuilder.Entity<Actor>().Property(p => p.Name).HasMaxLength(100);
            modelBuilder.Entity<Actor>().Property(p => p.Picture).IsUnicode();
            modelBuilder.Entity<Movie>().Property(p => p.Title).HasMaxLength(200);
            modelBuilder.Entity<Movie>().Property(p => p.Poster).IsUnicode();
            modelBuilder.Entity<Comment>().Property(p => p.Body).HasMaxLength(100);
            modelBuilder.Entity<ActorMovie>().HasKey(am => new { am.MovieId, am.ActorId });

            // Identity Stuff
            modelBuilder.Entity<IdentityUser>().ToTable("Users"); // table stores users
            modelBuilder.Entity<IdentityRole>().ToTable("Roles"); // table stores roles eg enduser, admin etc
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsersClaims"); // eg some user can do what 
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RolesClaims"); // eg some role can do what
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UsersLogins"); // when using 3rd provider for authentication
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UsersRoles"); // in what roles of our the users
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsersTokens"); // stores the tokens
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<ActorMovie> ActorsMovies { get; set; }

        public DbSet<Error> Errors { get; set; }

    }
}

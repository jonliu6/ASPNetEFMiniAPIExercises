using Microsoft.EntityFrameworkCore;

namespace MinimalAPIsWithASPNetEF.Entities
{
    public class AppDbCtx: DbContext
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
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Comment> Comments { get; set; }

    }
}

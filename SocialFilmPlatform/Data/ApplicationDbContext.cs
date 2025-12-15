using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Models;

namespace SocialFilmPlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Diary> Diaries { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<MovieDiary> MovieDiaries { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //m:m movie and diary
            modelBuilder.Entity<MovieDiary>()
                .HasKey(md => new { md.Id, md.MovieId, md.DiaryId });

            modelBuilder.Entity<MovieDiary>()
                .HasOne(md => md.Movie)
                .WithMany(m => m.MovieDiaries)
                .HasForeignKey(md => md.MovieId);

            modelBuilder.Entity<MovieDiary>()
                .HasOne(md => md.Diary)
                .WithMany(d => d.MovieDiaries)
                .HasForeignKey(md => md.DiaryId);
            
            //m:m movie and actor
            modelBuilder.Entity<ActorMovie>()
                .HasKey(md => new { md.Id, md.ActorId, md.MovieId });

            modelBuilder.Entity<ActorMovie>()
                .HasOne(md => md.Movie)
                .WithMany(m => m.ActorMovies)
                .HasForeignKey(md => md.MovieId);

            modelBuilder.Entity<ActorMovie>()
                .HasOne(md => md.Actor)
                .WithMany(d => d.ActorMovies)
                .HasForeignKey(md => md.ActorId);
        }
    }
}

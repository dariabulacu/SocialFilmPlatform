using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialFilmPlatform.Data;
using SocialFilmPlatform.Models;

namespace SocialFilmPlatform.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // 1. ROLES & USERS (Identity)
                // We use specific IDs to easily link them later without querying
                const string ADMIN_ID = "8e445865-a24d-4543-a6c6-9443d048cdb0";
                const string EDITOR_ID = "8e445865-a24d-4543-a6c6-9443d048cdb1";
                const string USER_ID = "8e445865-a24d-4543-a6c6-9443d048cdb2";

                const string ROLE_ADMIN_ID = "2c5e174e-3b0e-446f-86af-483d56fd7210";
                const string ROLE_EDITOR_ID = "2c5e174e-3b0e-446f-86af-483d56fd7211";
                const string ROLE_USER_ID = "2c5e174e-3b0e-446f-86af-483d56fd7212";

                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(
                        new IdentityRole { Id = ROLE_ADMIN_ID, Name = "Admin", NormalizedName = "ADMIN" },
                        new IdentityRole { Id = ROLE_EDITOR_ID, Name = "Editor", NormalizedName = "EDITOR" },
                        new IdentityRole { Id = ROLE_USER_ID, Name = "User", NormalizedName = "USER" }
                    );
                }

                if (!context.Users.Any())
                {
                    var hasher = new PasswordHasher<ApplicationUser>();

                    context.Users.AddRange(
                        new ApplicationUser
                        {
                            Id = ADMIN_ID,
                            UserName = "admin@test.com",
                            Email = "admin@test.com",
                            NormalizedEmail = "ADMIN@TEST.COM",
                            NormalizedUserName = "ADMIN@TEST.COM",
                            EmailConfirmed = true,
                            PasswordHash = hasher.HashPassword(null, "Admin1!"),
                            FirstName = "Admin",
                            LastName = "User",
                            Description = "Administrator account for the platform."
                        },
                        new ApplicationUser
                        {
                            Id = EDITOR_ID,
                            UserName = "editor@test.com",
                            Email = "editor@test.com",
                            NormalizedEmail = "EDITOR@TEST.COM",
                            NormalizedUserName = "EDITOR@TEST.COM",
                            EmailConfirmed = true,
                            PasswordHash = hasher.HashPassword(null, "Editor1!"),
                            FirstName = "Editor",
                            LastName = "User",
                            Description = "Editor account for value moderation."
                        },
                        new ApplicationUser
                        {
                            Id = USER_ID,
                            UserName = "user@test.com",
                            Email = "user@test.com",
                            NormalizedEmail = "USER@TEST.COM",
                            NormalizedUserName = "USER@TEST.COM",
                            EmailConfirmed = true,
                            PasswordHash = hasher.HashPassword(null, "User1!"),
                            FirstName = "Regular",
                            LastName = "User",
                            Description = "Standard user account."
                        }
                    );

                    context.UserRoles.AddRange(
                        new IdentityUserRole<string> { RoleId = ROLE_ADMIN_ID, UserId = ADMIN_ID },
                        new IdentityUserRole<string> { RoleId = ROLE_EDITOR_ID, UserId = EDITOR_ID },
                        new IdentityUserRole<string> { RoleId = ROLE_USER_ID, UserId = USER_ID }
                    );

                    context.SaveChanges();
                }

                // 2. GENRES
                if (!context.Genres.Any())
                {
                    context.Genres.AddRange(
                        new Genre { GenreName = "Action" },
                        new Genre { GenreName = "Sci-Fi" },
                        new Genre { GenreName = "Drama" },
                        new Genre { GenreName = "Comedy" },
                        new Genre { GenreName = "Thriller" },
                        new Genre { GenreName = "Romance" },
                        new Genre { GenreName = "Animation" },
                        new Genre { GenreName = "Horror" }
                    );
                    context.SaveChanges();
                }

                // 3. ACTORS
                if (!context.Actors.Any())
                {
                     context.Actors.AddRange(
                        new Actor { Name = "Leonardo DiCaprio", PhotoUrl = "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?auto=format&fit=crop&q=80&w=200&h=200", Description = "American actor and film producer. Known for pending implementation." },
                        new Actor { Name = "Christian Bale", PhotoUrl = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&q=80&w=200&h=200", Description = "English actor known for intense method acting roles." },
                        new Actor { Name = "Timothée Chalamet", PhotoUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&q=80&w=200&h=200", Description = "American and French actor with various accolades." },
                        new Actor { Name = "Scarlett Johansson", PhotoUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&q=80&w=200&h=200", Description = "American actress and world's highest paid actress 2018." },
                        new Actor { Name = "Robert Downey Jr.", PhotoUrl = "https://images.unsplash.com/photo-1599566150163-29194dcaad36?auto=format&fit=crop&q=80&w=200&h=200", Description = "American actor and producer, known for Iron Man role." }
                    );
                    context.SaveChanges();
                }

                // 4. MOVIES
                if (!context.Movies.Any())
                {
                    // Lookups for Genre IDs
                    int GetGenreId(string name) => context.Genres.FirstOrDefault(g => g.GenreName == name)?.Id ?? 0;

                    context.Movies.AddRange(
                        new Movie
                        {
                            Title = "Inception",
                            Director = "Christopher Nolan",
                            Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                            Score = 9.5f,
                            ReleaseDate = new DateTime(2010, 7, 16),
                            GenreId = GetGenreId("Sci-Fi"),
                            UserId = ADMIN_ID,
                            TrailerUrl = "https://www.youtube.com/watch?v=YoHD9XEInc0",
                            ImageUrl = "https://images.unsplash.com/photo-1626814026160-2237a95fc5a0?q=80&w=1000&auto=format&fit=crop"
                        },
                        new Movie
                        {
                            Title = "The Dark Knight",
                            Director = "Christopher Nolan",
                            Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                            Score = 9.8f,
                            ReleaseDate = new DateTime(2008, 7, 18),
                            GenreId = GetGenreId("Action"),
                            UserId = EDITOR_ID,
                            TrailerUrl = "https://www.youtube.com/watch?v=EXeTwQWrcwY",
                            ImageUrl = "https://images.unsplash.com/photo-1509347528160-9a9e33742cd4?q=80&w=1000&auto=format&fit=crop"
                        },
                        new Movie
                        {
                            Title = "Dune: Part Two",
                            Director = "Denis Villeneuve",
                            Description = "Paul Atreides unites with Chani and the Fremen while on a warpath of revenge against the conspirators who destroyed his family.",
                            Score = 9.2f,
                            ReleaseDate = new DateTime(2024, 3, 1),
                            GenreId = GetGenreId("Sci-Fi"),
                            UserId = ADMIN_ID,
                            TrailerUrl = "https://www.youtube.com/watch?v=Way9Dexny3w",
                            ImageUrl = "https://images.unsplash.com/photo-1541963463532-d68292c34b19?q=80&w=1000&auto=format&fit=crop"
                        },
                        new Movie
                        {
                            Title = "Pulp Fiction",
                            Director = "Quentin Tarantino",
                            Description = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                            Score = 8.9f,
                            ReleaseDate = new DateTime(1994, 10, 14),
                            GenreId = GetGenreId("Drama"),
                            UserId = EDITOR_ID,
                            TrailerUrl = "https://www.youtube.com/watch?v=s7EdQ4FqbhY",
                            ImageUrl = "https://images.unsplash.com/photo-1594909122845-11baa439b7bf?q=80&w=1000&auto=format&fit=crop"
                        },
                        new Movie
                        {
                            Title = "The Grand Budapest Hotel",
                            Director = "Wes Anderson",
                            Description = "A writer encounters the owner of an aging high-class hotel, who tells him of his early years serving as a lobby boy in the hotel's glorious years under an exceptional concierge.",
                            Score = 8.8f,
                            ReleaseDate = new DateTime(2014, 3, 28),
                            GenreId = GetGenreId("Comedy"),
                            UserId = ADMIN_ID,
                            TrailerUrl = "https://www.youtube.com/watch?v=1Fg5iHt5p60",
                            ImageUrl = "https://images.unsplash.com/photo-1551963831-b3b1ca40c98e?q=80&w=1000&auto=format&fit=crop"
                        }
                    );
                    context.SaveChanges();
                }

                // 5. DIARIES
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                        new Category { Name = "Favorites", IsPublic = true },
                        new Category { Name = "To Watch", IsPublic = false }
                    );
                    context.SaveChanges();
                }

                if (!context.Diaries.Any())
                {
                    // Need IDs
                    var catFavorites = context.Categories.First(c => c.Name == "Favorites");
                    var catToWatch = context.Categories.First(c => c.Name == "To Watch");

                    var mInception = context.Movies.FirstOrDefault(m => m.Title == "Inception");
                    var mDarkKnight = context.Movies.FirstOrDefault(m => m.Title == "The Dark Knight");

                    var tagClassic = new Tag { Name = "classic" };
                    var tagMustWatch = new Tag { Name = "must-watch" };
                    var tagScifi = new Tag { Name = "scifi" };

                    // Lists need to be instantiated carefully with collections
                    
                    var d1 = new Diary
                    {
                        Name = "Cinematic Masterpieces",
                        Description = "A collection of films essential for any cinephile.",
                        Content = "These movies define their genres.",
                        IsPublic = true,
                        CreatedAt = DateTime.Now.AddDays(-10),
                        UserId = ADMIN_ID,
                        Categories = new List<Category> { catFavorites },
                        Tags = new List<Tag> { tagClassic, tagMustWatch }
                    };
                    // Link movies via join table if using implicit, or explicit collection
                    // Model has ICollection<MovieDiary>
                    
                    if (mInception != null && mDarkKnight != null)
                    {
                        d1.MovieDiaries = new List<MovieDiary> 
                        { 
                            new MovieDiary { MovieId = mInception.Id, Name = "-", DiaryTime = DateTime.Now },
                            new MovieDiary { MovieId = mDarkKnight.Id, Name = "-", DiaryTime = DateTime.Now }
                        };
                    }
                    context.Diaries.Add(d1);

                    var d2 = new Diary
                    {
                        Name = "Mind Bending Sci-Fi",
                        Description = "Trippy movies that make you think.",
                        IsPublic = true,
                        CreatedAt = DateTime.Now.AddDays(-2),
                        UserId = USER_ID,
                        Categories = new List<Category> { catToWatch },
                        Tags = new List<Tag> { tagScifi }
                    };
                    if (mInception != null)
                    {
                        d2.MovieDiaries = new List<MovieDiary>
                        {
                            new MovieDiary { MovieId = mInception.Id, Name = "-", DiaryTime = DateTime.Now }
                        };
                    }
                    context.Diaries.Add(d2);

                    context.SaveChanges();
                }

                // 6. REVIEWS
                if (!context.Reviews.Any())
                {
                    var mInception = context.Movies.FirstOrDefault(m => m.Title == "Inception");
                    var mDarkKnight = context.Movies.FirstOrDefault(m => m.Title == "The Dark Knight");

                    if (mInception != null)
                    {
                        context.Reviews.Add(new Review
                        {
                            Content = "Absolutely mind-blowing visual effects and story.",
                            DatePosted = DateTime.Now.AddDays(-5),
                            UserId = USER_ID,
                            MovieId = mInception.Id
                        });
                    }

                    if (mDarkKnight != null)
                    {
                        context.Reviews.Add(new Review
                        {
                            Content = "Heath Ledger is the best Joker ever. Period.",
                            DatePosted = DateTime.Now.AddDays(-3),
                            UserId = EDITOR_ID,
                            MovieId = mDarkKnight.Id
                        });
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}

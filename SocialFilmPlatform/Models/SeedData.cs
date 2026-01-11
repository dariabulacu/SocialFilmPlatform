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


                if (!context.Actors.Any())
                {
                     context.Actors.AddRange(
                        new Actor { Name = "Leonardo DiCaprio", PhotoUrl = "/images/actors/4bbb9907-1617-451f-81c2-0036395fa8ce.webp", Description = "American actor and film producer. Known for his work in biographical and period films." },
                        new Actor { Name = "Christian Bale", PhotoUrl = "/images/actors/694f7e2e-e2a7-44a8-acbb-c7f0cd3384fd.png", Description = "English actor known for intense method acting roles." },
                        new Actor { Name = "Timothée Chalamet", PhotoUrl = "/images/actors/770ca68b-13d5-45bc-8e33-5bef7872b24e.jfif", Description = "American and French actor with various accolades." },
                        new Actor { Name = "Scarlett Johansson", PhotoUrl = "/images/actors/cca2c747-324e-4b81-8f3a-cfc76b155556.jpg", Description = "American actress and world's highest paid actress 2018." },
                        new Actor { Name = "Robert Downey Jr.", PhotoUrl = "/images/actors/f196ec54-7198-43bb-876e-416e8f65f1e6.jpg", Description = "American actor and producer, known for Iron Man role." },
                        new Actor { Name = "Keanu Reeves", PhotoUrl = "/images/actors/f9fda7db-f93e-4f42-97fe-11110565689b.jpg", Description = "Canadian actor known for his roles in The Matrix and John Wick series." }
                    );
                    context.SaveChanges();
                }


                if (!context.Movies.Any())
                {

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
                            ImageUrl = "/images/movies/0d5ccde1-1dcf-4a9a-bc7f-131ea9718ed4.jpg"
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
                            ImageUrl = "/images/movies/250197b5-e45d-4870-8ef6-243211e49a29.jpg"
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
                            ImageUrl = "/images/movies/2603c8ea-5aea-49e3-8172-af5207ddd6a1.jpg"
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
                            ImageUrl = "/images/movies/509e04bb-1afb-40b8-8ea7-6552916ed727.jpg"
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
                            ImageUrl = "/images/movies/8248f663-4cc1-4dac-a6b4-172d7d5349f0.jpg"
                        },
                        new Movie
                        {
                            Title = "Interstellar",
                            Director = "Christopher Nolan",
                            Description = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
                            Score = 9.3f,
                            ReleaseDate = new DateTime(2014, 11, 7),
                            GenreId = GetGenreId("Sci-Fi"),
                            UserId = ADMIN_ID,
                            TrailerUrl = "https://www.youtube.com/watch?v=zSWdZVtXT7E",
                            ImageUrl = "/images/movies/8b3e8e99-df2f-4a63-8978-9c415c77944c.webp"
                        },
                        new Movie
                        {
                            Title = "The Shawshank Redemption",
                            Director = "Frank Darabont",
                            Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                            Score = 9.9f,
                            ReleaseDate = new DateTime(1994, 9, 23),
                            GenreId = GetGenreId("Drama"),
                            UserId = EDITOR_ID,
                            TrailerUrl = "https://www.youtube.com/watch?v=6hB3S9bIaco",
                            ImageUrl = "/images/movies/d312d0bb-99d1-4963-9cd4-9e9c5d8f0761.webp"
                        },
                        new Movie
                        {
                            Title = "The Matrix",
                            Director = "The Wachowskis",
                            Description = "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
                            Score = 9.1f,
                            ReleaseDate = new DateTime(1999, 3, 31),
                            GenreId = GetGenreId("Sci-Fi"),
                            UserId = ADMIN_ID,
                            TrailerUrl = "https://www.youtube.com/watch?v=m8e-FF8MsqU",
                            ImageUrl = "/images/movies/d3b94526-ff3d-4558-8001-167b310d6558.jpg"
                        },
                        new Movie
                        {
                            Title = "Fight Club",
                            Director = "David Fincher",
                            Description = "An insomniac office worker and a devil-may-care soap maker form an underground fight club that evolves into much more.",
                            Score = 8.7f,
                            ReleaseDate = new DateTime(1999, 10, 15),
                            GenreId = GetGenreId("Thriller"),
                            UserId = EDITOR_ID,
                            TrailerUrl = "https://www.youtube.com/watch?v=SUXWAEX2jlg",
                            ImageUrl = "/images/movies/da533cb7-e6d9-42cd-b33e-ed7bb17258a5.jpg"
                        }
                    );
                    context.SaveChanges();
                }


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

                    var catFavorites = context.Categories.First(c => c.Name == "Favorites");
                    var catToWatch = context.Categories.First(c => c.Name == "To Watch");

                    var mInception = context.Movies.FirstOrDefault(m => m.Title == "Inception");
                    var mDarkKnight = context.Movies.FirstOrDefault(m => m.Title == "The Dark Knight");

                    var tagClassic = new Tag { Name = "classic" };
                    var tagMustWatch = new Tag { Name = "must-watch" };
                    var tagScifi = new Tag { Name = "scifi" };


                    
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

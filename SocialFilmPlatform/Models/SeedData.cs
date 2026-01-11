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
                        new Actor { Name = "Uma Thurman", PhotoUrl = "/images/actors/Uma_Thurman_2014_(cropped).jpg", Description = "Uma Karuna Thurman is an American actress, writer, producer and model. She has acted in a variety of films, from romantic comedies and dramas to science fiction and action films.", },
                        new Actor { Name = "Edward Norton", PhotoUrl = "/images/actors/Ed_Norton_Shankbone_Metropolitan_Opera_2009.jpg", Description = "Edward Harrison Norton (born August 18, 1969) is a European-American businessman, entrepreneur, actor, producer, director and screenwriter.", },
                        new Actor { Name = "Brad Pitt", PhotoUrl = "/images/actors/bradpitt.jpg", Description = "William Bradley Pitt (born December 18, 1963) is an American actor and film producer. He has received various accolades, including two Academy Awards, two British Academy Film Awards, two Golden Globe Awards, and a Primetime Emmy Award.", },
                        new Actor { Name = "Willem Dafoe", PhotoUrl = "/images/actors/Willem_Dafoe-63668_(cropped).jpg", Description = "William James \"Willem\" Dafoe (born July 22, 1955) is an American actor. Known for his prolific career portraying diverse roles in both mainstream and arthouse films, he is the recipient of various accolades, including the Volpi Cup for Best Actor as well as nominations for four Academy Awards.", },
                        new Actor { Name = "Naomi Watts", PhotoUrl = "/images/actors/cca2c747-324e-4b81-8f3a-cfc76b155556.jpg", Description = "Naomi Ellen Watts is a British actress. Known for her work predominantly in independent films with dark or tragic themes, she has received various accolades, including nominations for two Academy Awards, a Primetime Emmy Award, and two Golden Globe Awards." },
                        new Actor { Name = "Jack Nance", PhotoUrl= "/images/actors/f196ec54-7198-43bb-876e-416e8f65f1e6.jpg", Description= "Marvin John \"Jack\" Nance was an American actor. A frequent collaborator of filmmaker David Lynch, he starred in Lynch's directorial debut Eraserhead and continued to work with Lynch throughout his career, including a recurring role as Pete Martell on Twin Peaks." }


                    );
                    context.SaveChanges();
                }


                if (!context.Movies.Any())
                {

                    int GetGenreId(string name) => context.Genres.FirstOrDefault(g => g.GenreName == name)?.Id ?? 0;

                    context.Movies.AddRange(
                        new Movie
                        {
                            Title = "Mulholland Drive",
                            Director = "David Lynch",
                            Description = "A love story in the city of dreams.\r\nBlonde Betty Elms has only just arrived in Hollywood to become a movie star when she meets an enigmatic brunette with amnesia. Meanwhile, as the two set off to solve the second woman’s identity, filmmaker Adam Kesher runs into ominous trouble while casting his latest project.",
                            Score = 9.5f,
                            ReleaseDate = new DateTime(2001, 10, 19),
                            GenreId = GetGenreId("Thriller"),
                            UserId = ADMIN_ID,
                            TrailerUrl = "https://youtu.be/jbZJ487oJlY?si=U-P5E18fPDBPjgJ6",
                            ImageUrl = "/images/movies/Mulholland-Drive.jpg"
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
                            ImageUrl = "/images/movies/pulpfiction.jpg"
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
                            TrailerUrl = "https://youtu.be/1Fg5iWmQjwk?si=jKKlDzeJf3PVXvmA",
                            ImageUrl = "/images/movies/0d5ccde1-1dcf-4a9a-bc7f-131ea9718ed4.jpg"
                        },
                       
                       
                        new Movie
                        {
                            Title = "Fight Club",
                            Director = "David Fincher",
                            Description = "An insomniac office worker and a devil-may-care soap maker form an underground fight club that evolves into much more.",
                            Score = 8.7f,
                            ReleaseDate = new DateTime(1999, 10, 15),
                            GenreId = GetGenreId("Action"),
                            UserId = EDITOR_ID,
                            TrailerUrl = "https://www.youtube.com/watch?v=SUXWAEX2jlg",
                            ImageUrl = "/images/movies/fightclub_1280.jpg"
                        },
                         new Movie
                         {
                             Title = "Fantastic Mr. Fox",
                             Director = "Wes Anderson",
                             Description = "Mr Fox, a family man, goes back to his ways of stealing, unable to resist his animal instincts. However, he finds himself trapped when three farmers decide to kill him and his kind.",
                             Score = 8.7f,
                             ReleaseDate = new DateTime(2009, 9, 20),
                             GenreId = GetGenreId("Comedy"),
                             UserId = EDITOR_ID,
                             TrailerUrl = "https://youtu.be/n2igjYFojUo?si=WjuvAQHHPZWpvPBV",
                             ImageUrl = "/images/movies/fantasticmrfox.jpg"
                         },
                             new Movie
                             {
                                 Title = "Eraserhead",
                                 Director = "David Lynch",
                                 Description = "Henry, who's trying to survive the gloomy industrial landscape, discovers that he has become a father. His baby turns out to be a mutant lizard-like creature whose endless wailing drives him insane.",
                                 Score = 7.2f,
                                 ReleaseDate = new DateTime(1977, 3, 17),
                                 GenreId = GetGenreId("Thriller"),
                                 UserId = EDITOR_ID,
                                 TrailerUrl = "https://youtu.be/7WAzFWu2tVw?si=wijO8mwA96y0clCD",
                                 ImageUrl = "/images/movies/2603c8ea-5aea-49e3-8172-af5207ddd6a1.jpg"
                             },
                              new Movie
                              {
                                  Title = "Spellbound",
                                  Director = "Alfred Hitchcock",
                                  Description = "Dr Anthony Edwardes arrives at a mental hospital to join as the director. Later, Dr Constance discovers that Edwardes is an impostor who does not recall his past.",
                                  Score = 7.5f,
                                  ReleaseDate = new DateTime(1945, 10, 28),
                                  GenreId = GetGenreId("Drama"),
                                  UserId = ADMIN_ID,
                                  TrailerUrl = "https://youtu.be/HIBaGIUbXqI?si=JxJ-ZdxIiO8-aqoh",
                                  ImageUrl = "/images/movies/250197b5-e45d-4870-8ef6-243211e49a29.jpg"
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

                    var mMulhollandDrive = context.Movies.FirstOrDefault(m => m.Title == "Mulholland Drive");
                    var mEraserhead = context.Movies.FirstOrDefault(m => m.Title == "Eraserhead");
                    var mSpellbound = context.Movies.FirstOrDefault(m => m.Title == "Spellbound");
                    var mGrandBudapest = context.Movies.FirstOrDefault(m => m.Title == "The Grand Budapest Hotel");
                    var mFantasticMrFox = context.Movies.FirstOrDefault(m => m.Title == "Fantastic Mr. Fox");

                    var tagClassic = new Tag { Name = "classic" };
                    var tagMustWatch = new Tag { Name = "must-watch" };
                    var tagSurreal = new Tag { Name = "surreal" };
                    var tagWhimsical = new Tag { Name = "whimsical" };

                    
                    var d1 = new Diary
                    {
                        Name = "David Lynch's Mind-Bending Cinema",
                        Description = "A journey through the surreal and haunting worlds of David Lynch, plus Hitchcock's psychological masterpiece.",
                        Content = "These films explore the darkest corners of the human psyche through dreamlike narratives and unforgettable imagery.",
                        IsPublic = true,
                        CreatedAt = DateTime.Now.AddDays(-10),
                        UserId = ADMIN_ID,
                        Categories = new List<Category> { catFavorites },
                        Tags = new List<Tag> { tagClassic, tagSurreal }
                    };

                    var movieDiariesList1 = new List<MovieDiary>();
                    if (mMulhollandDrive != null)
                        movieDiariesList1.Add(new MovieDiary { MovieId = mMulhollandDrive.Id, Name = "-", DiaryTime = DateTime.Now });
                    if (mEraserhead != null)
                        movieDiariesList1.Add(new MovieDiary { MovieId = mEraserhead.Id, Name = "-", DiaryTime = DateTime.Now });
                    if (mSpellbound != null)
                        movieDiariesList1.Add(new MovieDiary { MovieId = mSpellbound.Id, Name = "-", DiaryTime = DateTime.Now });
                    
                    if (movieDiariesList1.Any())
                        d1.MovieDiaries = movieDiariesList1;
                    
                    context.Diaries.Add(d1);

                    var d2 = new Diary
                    {
                        Name = "Wes Anderson's Whimsical World",
                        Description = "The distinctive, meticulously crafted visual style of Wes Anderson.",
                        Content = "Symmetrical compositions, vibrant color palettes, and quirky characters define these cinematic gems.",
                        IsPublic = true,
                        CreatedAt = DateTime.Now.AddDays(-5),
                        UserId = EDITOR_ID,
                        Categories = new List<Category> { catToWatch },
                        Tags = new List<Tag> { tagWhimsical, tagMustWatch }
                    };

                    var movieDiariesList2 = new List<MovieDiary>();
                    if (mGrandBudapest != null)
                        movieDiariesList2.Add(new MovieDiary { MovieId = mGrandBudapest.Id, Name = "-", DiaryTime = DateTime.Now });
                    if (mFantasticMrFox != null)
                        movieDiariesList2.Add(new MovieDiary { MovieId = mFantasticMrFox.Id, Name = "-", DiaryTime = DateTime.Now });
                    
                    if (movieDiariesList2.Any())
                        d2.MovieDiaries = movieDiariesList2;
                    
                    context.Diaries.Add(d2);

                    context.SaveChanges();
                }


                if (!context.Reviews.Any())
                {
                    var mMulhollandDrive = context.Movies.FirstOrDefault(m => m.Title == "Mulholland Drive");
                    var mPulpFiction = context.Movies.FirstOrDefault(m => m.Title == "Pulp Fiction");
                    var mGrandBudapest = context.Movies.FirstOrDefault(m => m.Title == "The Grand Budapest Hotel");

                    if (mMulhollandDrive != null)
                    {
                        context.Reviews.Add(new Review
                        {
                            Content = "A mesmerizing dive into the subconscious. Lynch at his absolute best!",
                            DatePosted = DateTime.Now.AddDays(-8),
                            UserId = USER_ID,
                            MovieId = mMulhollandDrive.Id
                        });
                    }

                    if (mPulpFiction != null)
                    {
                        context.Reviews.Add(new Review
                        {
                            Content = "Tarantino's masterpiece. Non-linear storytelling at its finest.",
                            DatePosted = DateTime.Now.AddDays(-5),
                            UserId = EDITOR_ID,
                            MovieId = mPulpFiction.Id
                        });
                    }

                    if (mGrandBudapest != null)
                    {
                        context.Reviews.Add(new Review
                        {
                            Content = "Visually stunning with perfect symmetry. Wes Anderson's attention to detail is unmatched.",
                            DatePosted = DateTime.Now.AddDays(-3),
                            UserId = ADMIN_ID,
                            MovieId = mGrandBudapest.Id
                        });
                    }
                    context.SaveChanges();
                }

                if (!context.ActorMovies.Any())
                {
                    var actorUmaThurman = context.Actors.FirstOrDefault(a => a.Name == "Uma Thurman");
                    var actorEdwardNorton = context.Actors.FirstOrDefault(a => a.Name == "Edward Norton");
                    var actorBradPitt = context.Actors.FirstOrDefault(a => a.Name == "Brad Pitt");
                    var actorWillemDafoe = context.Actors.FirstOrDefault(a => a.Name == "Willem Dafoe");
                    var actorNaomiWatts = context.Actors.FirstOrDefault(a => a.Name == "Naomi Watts");
                    var actorJackNance = context.Actors.FirstOrDefault(a => a.Name == "Jack Nance");

                    var mMulhollandDrive = context.Movies.FirstOrDefault(m => m.Title == "Mulholland Drive");
                    var mPulpFiction = context.Movies.FirstOrDefault(m => m.Title == "Pulp Fiction");
                    var mGrandBudapest = context.Movies.FirstOrDefault(m => m.Title == "The Grand Budapest Hotel");
                    var mFightClub = context.Movies.FirstOrDefault(m => m.Title == "Fight Club");
                    var mEraserhead = context.Movies.FirstOrDefault(m => m.Title == "Eraserhead");

                    var actorMovies = new List<ActorMovie>();

                    // The Grand Budapest Hotel - Willem Dafoe, Edward Norton
                    if (mGrandBudapest != null && actorWillemDafoe != null)
                        actorMovies.Add(new ActorMovie { MovieId = mGrandBudapest.Id, ActorId = actorWillemDafoe.Id, Name = "J.G. Jopling" });
                    if (mGrandBudapest != null && actorEdwardNorton != null)
                        actorMovies.Add(new ActorMovie { MovieId = mGrandBudapest.Id, ActorId = actorEdwardNorton.Id, Name = "Henckels" });

                    // Eraserhead - Jack Nance
                    if (mEraserhead != null && actorJackNance != null)
                        actorMovies.Add(new ActorMovie { MovieId = mEraserhead.Id, ActorId = actorJackNance.Id, Name = "Henry Spencer" });

                    // Pulp Fiction - Uma Thurman
                    if (mPulpFiction != null && actorUmaThurman != null)
                        actorMovies.Add(new ActorMovie { MovieId = mPulpFiction.Id, ActorId = actorUmaThurman.Id, Name = "Mia Wallace" });

                    // Fight Club - Brad Pitt, Edward Norton
                    if (mFightClub != null && actorBradPitt != null)
                        actorMovies.Add(new ActorMovie { MovieId = mFightClub.Id, ActorId = actorBradPitt.Id, Name = "Tyler Durden" });
                    if (mFightClub != null && actorEdwardNorton != null)
                        actorMovies.Add(new ActorMovie { MovieId = mFightClub.Id, ActorId = actorEdwardNorton.Id, Name = "The Narrator" });

                    // Mulholland Drive - Naomi Watts
                    if (mMulhollandDrive != null && actorNaomiWatts != null)
                        actorMovies.Add(new ActorMovie { MovieId = mMulhollandDrive.Id, ActorId = actorNaomiWatts.Id, Name = "Betty Elms / Diane Selwyn" });

                    if (actorMovies.Any())
                    {
                        context.ActorMovies.AddRange(actorMovies);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}

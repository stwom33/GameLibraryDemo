using Microsoft.EntityFrameworkCore;
using GameLibrary.Models;
using GameLibrary.Enums;



namespace GameLibrary.Infrastructure
{
    public class LibraryContext : DbContext
    {

        public static readonly string dbName = "GameLibrary";
        
        public LibraryContext (DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }

        public static void SeedSampleData() 
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: LibraryContext.dbName)
                .Options;

            List<Game> games = new List<Game>();
            var game1 = new Game{
                        ID = 1234,
                        Title = "In the Hunt",
                        GameSystem = GameSystems.PlayStation,
                        ReleaseDate = new DateTime(1996, 3, 22),
                        Price = 120.0,
                        UPC = "752919470039"       
                };
            games.Add(game1);
            var game2 = new Game{
                        ID = 753,
                        Title = "Legend of Dragoon",
                        GameSystem = GameSystems.PlayStation,
                        ReleaseDate = new DateTime(2000, 6, 19),
                        Price = 80.0,
                        UPC = "711719449126"       
                };
            games.Add(game2);


            using (var context = new LibraryContext(options))
            {
                foreach(var game in games)
                {
                    context.Games.Add(game);
                }  
                context.SaveChanges();
            }
        }
    }
}
using System.Threading.Tasks;
using GameLibrary.APIs;
using GameLibrary.Models;
using GameLibrary.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using GameLibrary.Enums;

namespace GameLibrary.Services;

public class GameService : IGameService
{
    private readonly IPriceChartingAPIClient _priceChartingAPIClient;
    private readonly DbContextOptions<LibraryContext> _dbContextOptions;

    public GameService(IPriceChartingAPIClient? priceChartingAPIClient = null,
        DbContextOptions<LibraryContext>? dbContextOptions = null) 
    {
        _priceChartingAPIClient = priceChartingAPIClient ?? new PriceChartingAPIClient();
        _dbContextOptions = dbContextOptions ?? new DbContextOptionsBuilder<LibraryContext>()
            .UseInMemoryDatabase(databaseName: LibraryContext.dbName)
            .Options;
    }

    public Game? GetGameById(int id)
    {
        Game? result;
        using (var context = new LibraryContext(_dbContextOptions))
        {
            result = context.Games.FirstOrDefault(x => x.ID == id);
        }

        return result;   
    }

    public Game? GetGameByTitle(string title)
    {
        Game? result;
        using (var context = new LibraryContext(_dbContextOptions))
        {
            result = context.Games.FirstOrDefault(x => string.Equals(title, x.Title, StringComparison.OrdinalIgnoreCase));
        }

        return result;
    }

    public async Task<Game> UpdateGamePrice(Game game) 
    {
        var result = await _priceChartingAPIClient.GetPriceChartingProductByUPC(game.UPC);
        if (result["status"]?.ToString() != "error")
        {
            //cib-price is some integer "in pennies", to convert to USD, divide by 100;
            game.Price = (double)(int.Parse(result["cib-price"].ToString()) / 100.0);
        }

        using (var context = new LibraryContext(_dbContextOptions))
        {
            context.Games.Update(game);
            context.SaveChanges();
        }

        return game;  
    }

    public async Task<Game> CreateGameWithUPC(string upc)
    {
        var result = await _priceChartingAPIClient.GetPriceChartingProductByUPC(upc);
        if (result["status"]?.ToString() != "error")
        {
            var gameResult = CreateGameFromPriceChartingJson(result);
            using (var context = new LibraryContext(_dbContextOptions))
            {
                context.Games.Add(gameResult);
                context.SaveChanges();
            }
            return gameResult;
        }

        return null;
    }

    public Game CreateGameFromPriceChartingJson(JObject json)
    {
        return new Game {
            ID = 543,   // in the future, make the DB responsible for ID creation
            Title = json["product-name"]?.ToString() ?? "",
            GameSystem = MapToGameSystems(json["console-name"]?.ToString() ?? "Other"),
            ReleaseDate = DateTime.Parse(json["release-date"]?.ToString() ?? "1901-01-01"),
            Price = (double)(int.Parse(json["cib-price"]?.ToString() ?? "0") / 100.0),
            UPC = json["upc"]?.ToString() ?? "",
        };
    }

    public GameSystems MapToGameSystems(string system)
    {
        switch(system)
        {
            case "NES":
                return GameSystems.NES;
            case "Super Nintendo":
                return GameSystems.SuperNintendo;
            case "Playstation":
                return GameSystems.PlayStation;
            default:
                return GameSystems.Other;
        }
    }
}
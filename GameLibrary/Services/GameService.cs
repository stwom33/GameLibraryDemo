using System.Threading.Tasks;
using GameLibrary.APIs;
using GameLibrary.Models;
using GameLibrary.Infrastructure;
using Microsoft.EntityFrameworkCore;

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

    public async Task UpdateGamePrice(Game game) {
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
    }
}
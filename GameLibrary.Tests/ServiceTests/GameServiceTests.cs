using GameLibrary.Infrastructure;
using GameLibrary.Models;
using GameLibrary.Enums;
using GameLibrary.Services;
using Microsoft.EntityFrameworkCore;
using GameLibrary.APIs;
using Moq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace GameLibrary.Tests;

public class GameServiceTests
{
    private const string testDbName = "testingDB";
    private readonly DbContextOptions<LibraryContext> _dbTestContextOptions = new DbContextOptionsBuilder<LibraryContext>()
            .UseInMemoryDatabase(databaseName: testDbName)
            .Options;

    [SetUp]
    public void Setup()
    {
        List<Game> games = new List<Game>();

        games.Add(new Game {
            Title = "Resident Evil 2",
            ID = 12,
            UPC = "013388210237",
            GameSystem = GameSystems.PlayStation,
            Price = 43.02,
        });

        using (var context = new LibraryContext(_dbTestContextOptions))
        {
            foreach(var game in games)
            {
                context.Games.Add(game);
            }

            context.SaveChanges();
        }
    }

    [Test]
    public void CanGetGameByID() 
    {
        GameService gService = new GameService(null, _dbTestContextOptions);
        var result = gService.GetGameById(12);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ID, Is.EqualTo(12));
        Assert.That(result.Title, Is.EqualTo("Resident Evil 2"));
    }

    [Test]
    public async Task CanUpdatePrice()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>
        {
            {"status", "success"},
            {"cib-price", 4800},
        };       

        var mockResult = JObject.Parse(JsonConvert.SerializeObject(dict));

        var mockApiClient = new Mock<IPriceChartingAPIClient>();
            mockApiClient.Setup(x => x.GetPriceChartingProductByUPC(It.IsAny<string>())).ReturnsAsync(mockResult);

        GameService gService = new GameService(mockApiClient.Object, _dbTestContextOptions);

        var residentEvil2 = gService.GetGameById(12);
        await gService.UpdateGamePrice(residentEvil2);

        Assert.That(residentEvil2.Price, Is.EqualTo(48.00));
    }
}
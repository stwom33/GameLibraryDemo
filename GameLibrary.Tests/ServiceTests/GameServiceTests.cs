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

    [OneTimeSetUp]
    public void Init()
    {
        List<Game> games =
        [
            new Game {
                Title = "Resident Evil 2",
                ID = 12,
                UPC = "013388210237",
                GameSystem = GameSystems.PlayStation,
                Price = 43.02,
            },
        ];

        using (var context = new LibraryContext(_dbTestContextOptions))
        {
            foreach(var game in games)
            {
                context.Games.Add(game);
            }

            context.SaveChanges();
        }
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        using (var context = new LibraryContext(_dbTestContextOptions))
        {
            context.Database.EnsureDeleted();
        }
    }

    [Test]
    public void CanGetGameByID() 
    {
        //Arrange
        GameService gService = new GameService(null, _dbTestContextOptions);
        
        //Act
        var result = gService.GetGameById(12);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ID, Is.EqualTo(12));
        Assert.That(result.Title, Is.EqualTo("Resident Evil 2"));
    }

    [Test]
    public async Task CanUpdatePrice()
    {
        //Arrange
        Dictionary<string, object> dict = new Dictionary<string, object>
        {
            {"status", "success"},
            {"cib-price", 4800},
        };       
        var mockResult = JObject.Parse(JsonConvert.SerializeObject(dict));
        var mockApiClient = new Mock<IPriceChartingAPIClient>();
            mockApiClient.Setup(x => x.GetPriceChartingProductByUPC(It.IsAny<string>())).ReturnsAsync(mockResult);
        GameService gService = new GameService(mockApiClient.Object, _dbTestContextOptions);

        //Act
        var residentEvil2 = gService.GetGameById(12);
        await gService.UpdateGamePrice(residentEvil2);

        //Assert
        Assert.That(residentEvil2.ID, Is.EqualTo(12));
        Assert.That(residentEvil2.Price, Is.EqualTo(48.00));
    }

    [Test]
    public async Task CanCreateGameWithUPC()
    {
        //Arrange
        const string upc = "083717170297";
        Dictionary<string, object> dict = new Dictionary<string, object>
        {
            {"status", "success"},
            {"cib-price", 9499},
            {"console-name", "Playstation"},
            {"upc", upc},
            {"release-date", "1998-06-30"},
            {"product-name", "Azure Dreams"},
        };
        var mockResult = JObject.Parse(JsonConvert.SerializeObject(dict));
        var mockApiClient = new Mock<IPriceChartingAPIClient>();
            mockApiClient.Setup(x => x.GetPriceChartingProductByUPC(It.IsAny<string>())).ReturnsAsync(mockResult);
        GameService gService = new GameService(mockApiClient.Object, _dbTestContextOptions);    

        //Act
        var result = await gService.CreateGameWithUPC(upc);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.UPC, Is.EqualTo(upc));
        Assert.That(result.GameSystem, Is.EqualTo(GameSystems.PlayStation));
        Assert.That(result.Title, Is.EqualTo("Azure Dreams"));
        Assert.That(result.ReleaseDate, Is.EqualTo(DateTime.Parse("1998-06-30")));
        Assert.That(result.ID, Is.EqualTo(543));
    }
}
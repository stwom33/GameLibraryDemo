using GameLibrary.Enums;
using GameLibrary.Models;
using Newtonsoft.Json.Linq;

namespace GameLibrary.Services;

public interface IGameService
{
    public Game? GetGameById(int id);

    public Game? GetGameByTitle(string title);

    public Task<Game> UpdateGamePrice(Game game);

    public Task<Game> CreateGameWithUPC(string upc);

    public Game CreateGameFromPriceChartingJson(JObject json);

    public GameSystems MapToGameSystems(string system);

}
using GameLibrary.Models;

namespace GameLibrary.Services;

public interface IGameService
{
    public Game? GetGameById(int id);

    public Game? GetGameByTitle(string title);

    public Task UpdateGamePrice(Game game);

}
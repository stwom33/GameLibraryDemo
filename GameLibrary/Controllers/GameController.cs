using GameLibrary.Enums;
using GameLibrary.Models;
using GameLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private GameService _gameService = new GameService();

    [HttpGet("{id:int}")]
    public Game GetGameById(int id)
    {
        var game = _gameService.GetGameById(id);
        return game;
    }

    [HttpGet("title/{title}")]
    public Game GetGameByTitle(string title)
    {
        var game = _gameService.GetGameByTitle(title);
        return game;
    }

    [HttpPut("upc/{upc}")]
    public async Task<Game> AddGameByUPC(string upc)
    {
        var game = await _gameService.CreateGameWithUPC(upc);
        return game;
    }
}
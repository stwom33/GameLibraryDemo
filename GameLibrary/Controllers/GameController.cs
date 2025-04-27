using GameLibrary.Enums;
using GameLibrary.Models;
using GameLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    [HttpGet("{id:int}")]
    public Game GetGameById(int id)
    {
        GameService gService = new GameService();
        var game = gService.GetGameById(id);
        return game;
    }

    [HttpGet("title/{title}")]
    public Game GetGameByTitle(string title)
    {
        GameService gService = new GameService();
        var game = gService.GetGameByTitle(title);
        return game;
    }

    [HttpPut("upc/{upc}")]
    public async Task<Game> AddGameByUPC(string upc)
    {
        GameService gService = new GameService();
        var game = await gService.CreateGameWithUPC(upc);
        return game;
    }
}
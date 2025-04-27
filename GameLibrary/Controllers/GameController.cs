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
        // GET game from database
        GameService gService = new GameService();

        var game = gService.GetGameById(id);

        return game;
    }

    [HttpGet("title/{title}")]
    public Game GetGameByTitle(string title)
    {
        return new Game {
            ID = 432,
            Title = title,
            GameSystem = GameSystems.Genesis,
            ReleaseDate = new DateTime(1992, 12, 15),
            Price = 34.23
        };
    }
}
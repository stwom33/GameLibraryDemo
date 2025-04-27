using GameLibrary.Enums;

namespace GameLibrary.Models;

public class Game
{
    public int ID {get; set;}
    public string Title {get; set;}
    public DateTime ReleaseDate {get; set;}
    public GameSystems GameSystem {get; set;}
    public double Price {get; set;}
    public string UPC {get; set;}
}
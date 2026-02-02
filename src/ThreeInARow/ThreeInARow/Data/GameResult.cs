namespace ThreeInARow.Data;

/// <summary>
/// Результат одной игровой партии
/// </summary>
public class GameResult
{
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime PlayedAt { get; set; }
}
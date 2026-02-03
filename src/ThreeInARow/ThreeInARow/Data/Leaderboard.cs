using ThreeInARow.Data.Abstractions;

namespace ThreeInARow.Data;

/// <summary>
/// Таблица лидеров с результатами игроков
/// </summary>
public class Leaderboard : ILeaderboard
{
    private readonly List<GameResult> _results;

    public Leaderboard()
    {
        _results = new List<GameResult>();
    }

    /// <summary>
    /// Запрос: получить топ результатов
    /// </summary>
    public List<GameResult> GetTop(int n)
    {
        return _results
            .OrderByDescending(r => r.Score)
            .Take(n)
            .ToList();
    }

    /// <summary>
    /// Запрос: получить все результаты
    /// </summary>
    public List<GameResult> GetAll()
    {
        return _results
            .OrderByDescending(r => r.Score)
            .ToList();
    }

    /// <summary>
    /// Команда: добавить новый результат
    /// </summary>
    public void AddResult(string playerName, int score)
    {
        var result = new GameResult
        {
            PlayerName = playerName,
            Score = score,
            PlayedAt = DateTime.Now
        };

        _results.Add(result);
    }
}

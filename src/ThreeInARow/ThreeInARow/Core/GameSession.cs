using ThreeInARow.Core.Abstractions;

namespace ThreeInARow.Core;

/// <summary>
/// Представляет одну игровую партию
/// </summary>
public class GameSession : IGameSession
{
    private int _score;
    private bool _isFinished;
    private readonly string _playerName;
    private int _movesCount;
    private readonly int _maxMoves;
    private readonly int? _targetScore;

    public GameSession(string playerName, int maxMoves = 30, int? targetScore = null)
    {
        _playerName = playerName ?? "Player";
        _score = 0;
        _isFinished = false;
        _movesCount = 0;
        _maxMoves = maxMoves;
        _targetScore = targetScore;
    }

    /// <summary>
    /// Запрос: получить текущий счёт
    /// </summary>
    public int GetScore()
    {
        return _score;
    }

    /// <summary>
    /// Запрос: получить количество сделанных ходов
    /// </summary>
    public int GetMovesCount()
    {
        return _movesCount;
    }

    /// <summary>
    /// Запрос: получить максимальное количество ходов
    /// </summary>
    public int GetMaxMoves()
    {
        return _maxMoves;
    }

    /// <summary>
    /// Запрос: получить оставшееся количество ходов
    /// </summary>
    public int GetRemainingMoves()
    {
        return _maxMoves - _movesCount;
    }

    /// <summary>
    /// Запрос: получить целевой счёт (если установлен)
    /// </summary>
    public int? GetTargetScore()
    {
        return _targetScore;
    }

    /// <summary>
    /// Запрос: проверить, завершена ли партия
    /// </summary>
    public bool IsFinished()
    {
        return _isFinished;
    }

    /// <summary>
    /// Запрос: проверить, достигнута ли цель по очкам
    /// </summary>
    public bool IsTargetReached()
    {
        return _targetScore.HasValue && _score >= _targetScore.Value;
    }

    /// <summary>
    /// Запрос: проверить, закончились ли ходы
    /// </summary>
    public bool IsOutOfMoves()
    {
        return _movesCount >= _maxMoves;
    }

    /// <summary>
    /// Запрос: получить имя игрока
    /// </summary>
    public string GetPlayerName()
    {
        return _playerName;
    }

    /// <summary>
    /// Команда: начать партию
    /// </summary>
    public void Start()
    {
        _score = 0;
        _movesCount = 0;
        _isFinished = false;
    }

    /// <summary>
    /// Команда: завершить партию
    /// </summary>
    public void Finish()
    {
        _isFinished = true;
    }

    /// <summary>
    /// Команда: добавить очки
    /// Предусловие: очки не могут быть отрицательными
    /// </summary>
    public void AddScore(int value)
    {
        if (value < 0)
            throw new ArgumentException("Очки не могут быть отрицательными", nameof(value));

        _score += value;
    }

    /// <summary>
    /// Команда: увеличить счётчик ходов
    /// </summary>
    public void IncrementMoves()
    {
        _movesCount++;
    }
}
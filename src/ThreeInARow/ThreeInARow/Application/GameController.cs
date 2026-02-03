using ThreeInARow.Application.;
using ThreeInARow.Application.Abstractions;
using ThreeInARow.Core;
using ThreeInARow.Data;

namespace ThreeInARow.Application;

/// <summary>
/// Главный контроллер игры, управляет игровым процессом
/// </summary>
public class GameController : IGameController
{
    private readonly GameStateMachine _stateMachine;
    private readonly Leaderboard _leaderboard;
    private GameSession? _currentSession;
    private GameBoard? _currentBoard;

    public GameController(GameStateMachine stateMachine, Leaderboard leaderboard)
    {
        _stateMachine = stateMachine;
        _leaderboard = leaderboard;
    }

    /// <summary>
    /// Запрос: получить текущее состояние игры
    /// </summary>
    public GameState GetCurrentState()
    {
        return _stateMachine.GetState();
    }

    /// <summary>
    /// Запрос: получить текущую игровую доску
    /// </summary>
    public GameBoard? GetCurrentBoard()
    {
        return _currentBoard;
    }

    /// <summary>
    /// Запрос: получить текущую сессию
    /// </summary>
    public GameSession? GetCurrentSession()
    {
        return _currentSession;
    }

    /// <summary>
    /// Запрос: получить таблицу лидеров
    /// </summary>
    public Leaderboard GetLeaderboard()
    {
        return _leaderboard;
    }

    /// <summary>
    /// Команда: начать новую игру
    /// </summary>
    public bool StartGame(string playerName)
    {
        if (!_stateMachine.ChangeState(GameState.Playing))
            return false;

        _currentSession = new GameSession(playerName);
        _currentBoard = new GameBoard();
        _currentSession.Start();

        return true;
    }

    /// <summary>
    /// Команда: обработать ход игрока
    /// Ограничение: ход можно обработать только в состоянии Playing
    /// </summary>
    public MoveResult ProcessMove(int x1, int y1, int x2, int y2)
    {
        if (_stateMachine.GetState() != GameState.Playing)
            return MoveResult.InvalidState;

        if (_currentBoard == null || _currentSession == null)
            return MoveResult.InvalidState;

        // Пробуем поменять элементы
        if (!_currentBoard.Swap(x1, y1, x2, y2))
            return MoveResult.InvalidMove;

        // Проверяем, появились ли совпадения
        if (!_currentBoard.HasMatch())
        {
            // ВАЖНО: Откатываем ход, если совпадений нет
            // После этого состояние доски возвращается к исходному
            _currentBoard.Swap(x1, y1, x2, y2);
            return MoveResult.NoMatch;
        }

        // Удаляем совпадения и начисляем очки
        // RemoveMatches сразу заменяет удалённые элементы новыми
        int removedCount = _currentBoard.RemoveMatches();
        int points = CalculatePoints(removedCount);
        _currentSession.AddScore(points);

        return MoveResult.Success;
    }

    /// <summary>
    /// Команда: завершить игру
    /// </summary>
    public bool EndGame()
    {
        if (_stateMachine.GetState() != GameState.Playing)
            return false;

        if (_currentSession != null)
        {
            _currentSession.Finish();
            _leaderboard.AddResult(
                _currentSession.GetPlayerName(),
                _currentSession.GetScore()
            );
        }

        _stateMachine.ChangeState(GameState.Finished);
        return true;
    }

    /// <summary>
    /// Команда: вернуться в меню
    /// </summary>
    public bool ReturnToMenu()
    {
        return _stateMachine.ChangeState(GameState.Menu);
    }

    /// <summary>
    /// Команда: показать таблицу лидеров
    /// </summary>
    public bool ShowLeaderboard()
    {
        return _stateMachine.ChangeState(GameState.Leaderboard);
    }

    private int CalculatePoints(int matchCount)
    {
        // Базовые очки: 10 за каждый удалённый элемент
        return matchCount * 10;
    }
}

/// <summary>
/// Результат обработки хода
/// </summary>
public enum MoveResult
{
    Success,
    InvalidMove,
    NoMatch,
    InvalidState
}
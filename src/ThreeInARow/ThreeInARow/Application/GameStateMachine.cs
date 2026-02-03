using ThreeInARow.Application.Abstractions;

namespace ThreeInARow.Application;

/// <summary>
/// Управляет состояниями игры
/// </summary>
public class GameStateMachine : IGameStateMachine
{
    private GameState _currentState;

    public GameStateMachine()
    {
        _currentState = GameState.Menu;
    }

    /// <summary>
    /// Запрос: получить текущее состояние
    /// </summary>
    public GameState GetState()
    {
        return _currentState;
    }

    /// <summary>
    /// Команда: изменить состояние
    /// Ограничение: можно переходить только между разрешёнными состояниями
    /// </summary>
    public bool ChangeState(GameState newState)
    {
        if (!IsValidTransition(_currentState, newState))
            return false;

        _currentState = newState;
        return true;
    }

    private bool IsValidTransition(GameState from, GameState to)
    {
        return (from, to) switch
        {
            (GameState.Menu, GameState.Playing) => true,
            (GameState.Menu, GameState.Leaderboard) => true,
            (GameState.Playing, GameState.Finished) => true,
            (GameState.Playing, GameState.Menu) => true,
            (GameState.Finished, GameState.Menu) => true,
            (GameState.Finished, GameState.Leaderboard) => true,
            (GameState.Leaderboard, GameState.Menu) => true,
            _ => false
        };
    }
}

/// <summary>
/// Возможные состояния игры
/// </summary>
public enum GameState
{
    Menu,
    Playing,
    Finished,
    Leaderboard
}
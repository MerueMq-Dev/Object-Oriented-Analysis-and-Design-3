using ThreeInARow.Core;
using ThreeInARow.Data;

namespace ThreeInARow.Application.Abstractions;

/// <summary>
/// АТД: Контроллер игры
/// 
/// НАЗНАЧЕНИЕ:
///   Центральная точка управления игровым процессом
///   Координирует взаимодействие между игровой логикой, состояниями и данными
///   Обрабатывает действия пользователя и обеспечивает корректность процесса
/// 
/// ЗАПРОСЫ:
///   GetCurrentState() -> GameState
///     Возвращает текущее состояние игры
///     Постусловие: возвращаемое значение соответствует состоянию машины состояний
///     
///   GetCurrentBoard() -> GameBoard?
///     Возвращает игровое поле активной партии
///     Постусловие: возвращает объект поля если GetCurrentState() = Playing
///     Постусловие: возвращает null в остальных состояниях
///     
///   GetCurrentSession() -> GameSession?
///     Возвращает текущую игровую сессию
///     Постусловие: возвращает объект сессии если GetCurrentState() ∈ {Playing, Finished}
///     Постусловие: возвращает null в остальных состояниях
///     
///   GetLeaderboard() -> Leaderboard
///     Возвращает таблицу лидеров
///     Постусловие: всегда возвращает определённый объект таблицы
///     
/// КОМАНДЫ:
///   StartGame(playerName: string) -> bool
///     Инициирует новую игровую партию
///     Предусловие: playerName является непустой строкой
///     Постусловие: при успехе GetCurrentState() возвращает Playing
///     Постусловие: при успехе GetCurrentBoard() возвращает инициализированное поле
///     Постусловие: при успехе GetCurrentSession() возвращает новую сессию с нулевым счётом
///     Постусловие: возвращает true при успешном переходе в состояние Playing
///     
///   ProcessMove(x1: int, y1: int, x2: int, y2: int) -> MoveResult
///     Обрабатывает ход игрока путём обмена двух элементов
///     Предусловие: GetCurrentState() возвращает Playing
///     Предусловие: координаты находятся в пределах игрового поля
///     Постусловие: при результате Success счёт увеличивается
///     Постусловие: при результате NoMatch состояние поля не изменяется
///     Постусловие: при результате InvalidMove состояние поля не изменяется
///     Постусловие: счёт изменяется только при результате Success
///     
///   EndGame() -> bool
///     Завершает текущую партию и сохраняет результат
///     Предусловие: GetCurrentState() возвращает Playing
///     Постусловие: при успехе GetCurrentState() возвращает Finished
///     Постусловие: при успехе результат добавлен в GetLeaderboard()
///     Постусловие: при успехе GetCurrentSession().IsFinished() возвращает true
///     
///   ReturnToMenu() -> bool
///     Осуществляет переход в главное меню
///     Постусловие: при успехе GetCurrentState() возвращает Menu
///     
///   ShowLeaderboard() -> bool
///     Осуществляет переход к просмотру таблицы лидеров
///     Постусловие: при успехе GetCurrentState() возвращает Leaderboard
///     
/// ИНВАРИАНТЫ:
///   - Состояние контроллера согласовано с состоянием машины состояний
///   - Игровое поле существует только в состоянии Playing
///   - При активной игре сессия всегда определена
///   - Обработка ходов осуществляется исключительно в состоянии Playing
///   - Сохранение результатов происходит только при вызове EndGame()
/// </summary>
public interface IGameController
{
    // Запросы
    GameState GetCurrentState();
    GameBoard? GetCurrentBoard();
    GameSession? GetCurrentSession();
    Leaderboard GetLeaderboard();

    // Команды
    bool StartGame(string playerName);
    MoveResult ProcessMove(int x1, int y1, int x2, int y2);
    bool EndGame();
    bool ReturnToMenu();
    bool ShowLeaderboard();
}
using ThreeInARow.Application;
using ThreeInARow.Data;
using ThreeInARow.UI;

// Инициализация по схеме из архитектуры:
// 1. Создаём GameStateMachine
var stateMachine = new GameStateMachine();

// 2. Создаём Leaderboard
var leaderboard = new Leaderboard();

// 3. Создаём GameController
var controller = new GameController(stateMachine, leaderboard);

// 4. Создаём TUI и запускаем игру
var ui = new TerminalUI(controller);

try
{
    ui.Run();
}
catch (Exception ex)
{
    Console.Clear();
    Console.WriteLine("Произошла ошибка:");
    Console.WriteLine(ex.Message);
    Console.WriteLine();
    Console.WriteLine("Нажмите Enter для выхода...");
    Console.ReadLine();
}
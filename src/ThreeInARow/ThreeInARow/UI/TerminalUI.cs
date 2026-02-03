using Terminal.Gui;
using ThreeInARow.Application;
using ThreeInARow.Core;
using TuiApp = Terminal.Gui.Application;

namespace ThreeInARow.UI;

/// <summary>
/// TUI интерфейс
/// </summary>
public class TerminalUI
{
    private readonly GameController _controller;
    private Window? _mainWindow;
    private View? _boardView;
    private Label? _statusLabel;
    private Label? _scoreLabel;
    private int _cursorX = 0;
    private int _cursorY = 0;
    private (int x, int y)? _selectedCell = null;

    public TerminalUI(GameController controller)
    {
        _controller = controller;
    }

    public void Run()
    {
        TuiApp.Init();

        try
        {
            ShowMainMenu();
            TuiApp.Run();
        }
        finally
        {
            TuiApp.Shutdown();
        }
    }

    private void ShowMainMenu()
    {
        var menu = new Window("Три в Ряд - Главное меню")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        var titleLabel = new Label("╔════════════════════════════════════╗\n" +
                                  "║      ТРИ В РЯД - Match-3 Game     ║\n" +
                                  "╚════════════════════════════════════╝")
        {
            X = Pos.Center(),
            Y = 2
        };

        var startButton = new Button("1. Новая игра")
        {
            X = Pos.Center(),
            Y = 7
        };
        startButton.Clicked += () => StartNewGame();

        var leaderboardButton = new Button("2. Таблица лидеров")
        {
            X = Pos.Center(),
            Y = 9
        };
        leaderboardButton.Clicked += () => ShowLeaderboard();

        var exitButton = new Button("3. Выход")
        {
            X = Pos.Center(),
            Y = 11
        };
        exitButton.Clicked += () => TuiApp.RequestStop();

        var helpLabel = new Label("Используйте клавиши 1-3 или Tab + Enter для навигации")
        {
            X = Pos.Center(),
            Y = Pos.Bottom(menu) - 3,
            ColorScheme = Colors.Base
        };

        menu.Add(titleLabel, startButton, leaderboardButton, exitButton, helpLabel);
        
        menu.KeyPress += (e) =>
        {
            switch (e.KeyEvent.Key)
            {
                case Key.D1:
                    StartNewGame();
                    e.Handled = true;
                    break;
                case Key.D2:
                    ShowLeaderboard();
                    e.Handled = true;
                    break;
                case Key.D3:
                    TuiApp.RequestStop();
                    e.Handled = true;
                    break;
            }
        };

        TuiApp.Top.RemoveAll();
        TuiApp.Top.Add(menu);
    }

    private void StartNewGame()
    {
        var dialog = new Dialog("Новая игра", 50, 10);

        var nameLabel = new Label("Введите ваше имя:")
        {
            X = 1,
            Y = 1
        };

        var nameField = new TextField("Player")
        {
            X = 1,
            Y = 2,
            Width = Dim.Fill() - 1
        };

        var okButton = new Button("Начать игру")
        {
            X = Pos.Center(),
            Y = 5,
            IsDefault = true
        };

        okButton.Clicked += () =>
        {
            var playerName = nameField.Text.ToString();
            if (string.IsNullOrWhiteSpace(playerName))
                playerName = "Player";

            TuiApp.RequestStop();

            if (_controller.StartGame(playerName))
            {
                ShowGameBoard();
            }
        };

        var cancelButton = new Button("Отмена")
        {
            X = Pos.Center(),
            Y = 6
        };
        cancelButton.Clicked += () => TuiApp.RequestStop();

        dialog.Add(nameLabel, nameField, okButton, cancelButton);
        TuiApp.Run(dialog);
    }

    private void ShowGameBoard()
    {
        _mainWindow = new Window("Три в Ряд - Игра")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        _boardView = new View()
        {
            X = 2,
            Y = 1,
            Width = 50,
            Height = 20,
            CanFocus = true
        };

        _statusLabel = new Label("Выберите первую клетку стрелками и нажмите Enter")
        {
            X = 2,
            Y = Pos.Bottom(_boardView) + 1,
            Width = Dim.Fill() - 2
        };

        _scoreLabel = new Label("")
        {
            X = 2,
            Y = Pos.Bottom(_statusLabel) + 1,
            Width = Dim.Fill() - 2
        };

        var legendLabel = new Label(
            "Элементы: R-красный, B-синий, G-зелёный\n" +
            "         Y-жёлтый, P-фиолетовый, O-оранжевый\n\n" +
            "Управление:\n" +
            "  ← ↑ → ↓  - перемещение курсора\n" +
            "  Enter    - выбрать/подтвердить клетку\n" +
            "  Esc      - отменить выбор\n" +
            "  Q        - завершить игру и вернуться в меню")
        {
            X = 55,
            Y = 1,
            Width = Dim.Fill() - 55
        };

        _mainWindow.Add(_boardView, _statusLabel, _scoreLabel, legendLabel);

        _boardView.KeyPress += HandleGameKeyPress;

        TuiApp.Top.RemoveAll();
        TuiApp.Top.Add(_mainWindow);

        _boardView.SetFocus();

        UpdateGameDisplay();
    }

    private void HandleGameKeyPress(View.KeyEventEventArgs e)
    {
        var board = _controller.GetCurrentBoard();
        if (board == null) return;

        int size = board.GetSize();

        switch (e.KeyEvent.Key)
        {
            case Key.CursorLeft:
                _cursorX = Math.Max(0, _cursorX - 1);
                UpdateGameDisplay();
                e.Handled = true;
                break;

            case Key.CursorRight:
                _cursorX = Math.Min(size - 1, _cursorX + 1);
                UpdateGameDisplay();
                e.Handled = true;
                break;

            case Key.CursorUp:
                _cursorY = Math.Max(0, _cursorY - 1);
                UpdateGameDisplay();
                e.Handled = true;
                break;

            case Key.CursorDown:
                _cursorY = Math.Min(size - 1, _cursorY + 1);
                UpdateGameDisplay();
                e.Handled = true;
                break;

            case Key.Enter:
                HandleCellSelection();
                e.Handled = true;
                break;

            case Key.Esc:
                _selectedCell = null;
                _statusLabel!.Text = "Выбор отменён. Выберите первую клетку.";
                UpdateGameDisplay();
                e.Handled = true;
                break;

            case Key.Tab:
            case Key.BackTab:
                e.Handled = true;
                break;

            case Key.q:
            case Key.Q:
                _controller.EndGame();
                ShowMainMenu();
                e.Handled = true;
                break;
        }
    }

    private void HandleCellSelection()
    {
        if (_selectedCell == null)
        {
            _selectedCell = (_cursorX, _cursorY);
            _statusLabel!.Text = $"Выбрана клетка ({_cursorX}, {_cursorY}). Выберите соседнюю клетку.";
            UpdateGameDisplay();
        }
        else
        {
            var (x1, y1) = _selectedCell.Value;
            var x2 = _cursorX;
            var y2 = _cursorY;

            var result = _controller.ProcessMove(x1, y1, x2, y2);

            switch (result)
            {
                case MoveResult.Success:
                    _statusLabel!.Text = "✓ Отличный ход! Очки начислены!";
                    _selectedCell = null;
                    System.Threading.Thread.Sleep(500);
                    UpdateGameDisplay();
                    break;

                case MoveResult.InvalidMove:
                    _statusLabel!.Text = "✗ Неверный ход. Элементы должны быть соседними.";
                    _selectedCell = null;
                    break;

                case MoveResult.NoMatch:
                    _statusLabel!.Text = "✗ Этот ход не создаёт совпадений. Попробуйте другой.";
                    _selectedCell = null;
                    break;

                case MoveResult.InvalidState:
                    _statusLabel!.Text = "✗ Нельзя сделать ход сейчас.";
                    _selectedCell = null;
                    break;
            }

            UpdateGameDisplay();

            _boardView?.SetFocus();
        }
    }

    private void UpdateGameDisplay()
    {
        var board = _controller.GetCurrentBoard();
        var session = _controller.GetCurrentSession();

        if (board == null || session == null) return;

        _scoreLabel!.Text = $"Игрок: {session.GetPlayerName()} | Очки: {session.GetScore()}";

        DrawBoard(board);
    }

    private void DrawBoard(GameBoard board)
    {
        if (_boardView == null) return;

        _boardView.RemoveAll();

        int size = board.GetSize();
        int startX = 0;
        int startY = 0;

        var header = "    0   1   2   3   4   5   6   7";
        var headerLabel = new Label(header)
        {
            X = startX,
            Y = startY
        };
        _boardView.Add(headerLabel);

        var topBorder = "  ╔═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╗";
        var topBorderLabel = new Label(topBorder)
        {
            X = startX,
            Y = startY + 1
        };
        _boardView.Add(topBorderLabel);

        // Строки поля
        for (int y = 0; y < size; y++)
        {
            var rowText = $"{y} ║";

            for (int x = 0; x < size; x++)
            {
                var element = board.GetElement(x, y);
                var symbol = GetElementSymbol(element.GetType());

                if (x == _cursorX && y == _cursorY)
                {
                    symbol = $"[{symbol}]";
                }
                else if (_selectedCell.HasValue && _selectedCell.Value.x == x && _selectedCell.Value.y == y)
                {
                    symbol = $"<{symbol}>";
                }
                else
                {
                    symbol = $" {symbol} ";
                }

                rowText += symbol + "║";
            }

            var rowLabel = new Label(rowText)
            {
                X = startX,
                Y = startY + 2 + y * 2
            };

            if (y == _cursorY)
            {
                rowLabel.ColorScheme = Colors.Menu;
            }

            _boardView.Add(rowLabel);

            if (y < size - 1)
            {
                var separator = "  ╠═══╬═══╬═══╬═══╬═══╬═══╬═══╬═══╣";
                var separatorLabel = new Label(separator)
                {
                    X = startX,
                    Y = startY + 3 + y * 2
                };
                _boardView.Add(separatorLabel);
            }
        }

        var bottomBorder = "  ╚═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╝";
        var bottomBorderLabel = new Label(bottomBorder)
        {
            X = startX,
            Y = startY + 2 + size * 2 - 1
        };
        _boardView.Add(bottomBorderLabel);

        _boardView.SetNeedsDisplay();
    }

    private string GetElementSymbol(ElementType type)
    {
        return type switch
        {
            ElementType.Red => "R",
            ElementType.Blue => "B",
            ElementType.Green => "G",
            ElementType.Yellow => "Y",
            ElementType.Purple => "P",
            ElementType.Orange => "O",
            _ => " "
        };
    }

    private void ShowLeaderboard()
    {
        var window = new Window("Таблица лидеров")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        var leaderboard = _controller.GetLeaderboard();
        var results = leaderboard.GetTop(10);

        var text = "╔════════════════════════════════════╗\n" +
                   "║        ТАБЛИЦА ЛИДЕРОВ             ║\n" +
                   "╚════════════════════════════════════╝\n\n";

        if (results.Count == 0)
        {
            text += "Пока нет результатов.";
        }
        else
        {
            text += "  №  | Игрок              | Очки   | Дата\n";
            text += "─────┼────────────────────┼────────┼──────────────\n";

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                text += $" {i + 1,3} | {result.PlayerName,-18} | {result.Score,6} | {result.PlayedAt:dd.MM.yyyy}\n";
            }
        }

        var textView = new TextView()
        {
            X = 2,
            Y = 1,
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 4,
            Text = text,
            ReadOnly = true,
            CanFocus = true
        };

        var helpLabel = new Label("Нажмите Q или Esc для возврата в меню")
        {
            X = Pos.Center(),
            Y = Pos.Bottom(window) - 2,
            ColorScheme = Colors.Base
        };

        void CloseLeaderboard()
        {
            _controller.ReturnToMenu();
            ShowMainMenu();
        }

        textView.KeyPress += (e) =>
        {
            if (e.KeyEvent.Key == Key.Esc || e.KeyEvent.Key == Key.q || e.KeyEvent.Key == Key.Q)
            {
                CloseLeaderboard();
                e.Handled = true;
            }
        };

        window.KeyPress += (e) =>
        {
            if (e.KeyEvent.Key == Key.Esc || e.KeyEvent.Key == Key.q || e.KeyEvent.Key == Key.Q)
            {
                CloseLeaderboard();
                e.Handled = true;
            }
        };

        window.Add(textView, helpLabel);

        TuiApp.Top.RemoveAll();
        TuiApp.Top.Add(window);

        textView.SetFocus();
    }
}
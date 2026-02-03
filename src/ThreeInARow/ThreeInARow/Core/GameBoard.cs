using ThreeInARow.Core.Abstractions;

namespace ThreeInARow.Core;

/// <summary>
/// Игровое поле 8x8 с элементами
/// </summary>
public class GameBoard : IGameBoard
{
    private const int BoardSize = 8;
    private readonly GameElement[,] _board;

    public GameBoard()
    {
        _board = new GameElement[BoardSize, BoardSize];
        InitializeBoard();
    }

    /// <summary>
    /// Запрос: получить элемент в позиции (x, y)
    /// </summary>
    public GameElement GetElement(int x, int y)
    {
        if (!IsInside(x, y))
            throw new ArgumentOutOfRangeException($"Позиция ({x}, {y}) за пределами поля");

        return _board[x, y];
    }

    /// <summary>
    /// Запрос: проверить, находится ли позиция внутри поля
    /// </summary>
    public bool IsInside(int x, int y)
    {
        return x >= 0 && x < BoardSize && y >= 0 && y < BoardSize;
    }

    /// <summary>
    /// Запрос: проверить наличие совпадений на поле
    /// </summary>
    public bool HasMatch()
    {
        return FindMatches().Count > 0;
    }

    /// <summary>
    /// Команда: поменять два элемента местами
    /// </summary>
    public bool Swap(int x1, int y1, int x2, int y2)
    {
        if (!IsInside(x1, y1) || !IsInside(x2, y2))
            return false;

        // Проверяем, что элементы соседние
        if (!AreAdjacent(x1, y1, x2, y2))
            return false;

        var temp = _board[x1, y1];
        _board[x1, y1] = _board[x2, y2];
        _board[x2, y2] = temp;

        return true;
    }

    /// <summary>
    /// Команда: удалить найденные совпадения
    /// Возвращает количество удалённых элементов
    /// </summary>
    public int RemoveMatches()
    {
        var matches = FindMatches();
        int removedCount = matches.Count;

        foreach (var (x, y) in matches)
        {
            _board[x, y] = new GameElement(GetRandomElementType());
        }

        return removedCount;
    }

    /// <summary>
    /// Команда: заполнить пустые места новыми элементами
    /// (В минимальной версии не используется, так как RemoveMatches сразу создаёт новые элементы)
    /// </summary>
    public void FillEmpty()
    {
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if (_board[x, y] == null)
                {
                    _board[x, y] = new GameElement(GetRandomElementType());
                }
            }
        }
    }

    /// <summary>
    /// Запрос: получить размер поля
    /// </summary>
    public int GetSize()
    {
        return BoardSize;
    }

    private void InitializeBoard()
    {
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                _board[x, y] = new GameElement(GetRandomElementType());
            }
        }

        // Убираем начальные совпадения
        while (HasMatch())
        {
            RemoveMatches();
        }
    }

    private List<(int x, int y)> FindMatches()
    {
        var matches = new HashSet<(int x, int y)>();

        // Проверяем горизонтальные совпадения
        for (int y = 0; y < BoardSize; y++)
        {
            for (int x = 0; x < BoardSize - 2; x++)
            {
                var type = _board[x, y].GetType();
                if (_board[x + 1, y].GetType() == type && _board[x + 2, y].GetType() == type)
                {
                    matches.Add((x, y));
                    matches.Add((x + 1, y));
                    matches.Add((x + 2, y));
                }
            }
        }

        // Проверяем вертикальные совпадения
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize - 2; y++)
            {
                var type = _board[x, y].GetType();
                if (_board[x, y + 1].GetType() == type && _board[x, y + 2].GetType() == type)
                {
                    matches.Add((x, y));
                    matches.Add((x, y + 1));
                    matches.Add((x, y + 2));
                }
            }
        }

        return matches.ToList();
    }

    private bool AreAdjacent(int x1, int y1, int x2, int y2)
    {
        int dx = Math.Abs(x1 - x2);
        int dy = Math.Abs(y1 - y2);

        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    private ElementType GetRandomElementType()
    {
        var values = Enum.GetValues<ElementType>();
        return values[Random.Shared.Next(values.Length)];
    }
}
using ThreeInARow.Core.Abstractions;

namespace ThreeInARow.Core;


/// <summary>
/// Представляет элемент на игровом поле
/// </summary>
public class GameElement : IGameElement
{
    public ElementType Type { get; private set; }

    public GameElement(ElementType type)
    {
        Type = type;
    }

    /// <summary>
    /// Команда: изменить тип элемента
    /// </summary>
    public void SetType(ElementType type)
    {
        Type = type;
    }

    /// <summary>
    /// Запрос: получить тип элемента
    /// </summary>
    public ElementType GetType()
    {
        return Type;
    }
}

public enum ElementType
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple,
    Orange
}

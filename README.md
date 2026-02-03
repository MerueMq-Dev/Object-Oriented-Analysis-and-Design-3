# Обзор проекта "Три в Ряд"

## Быстрый старт

```bash
# Требуется: .NET 9 SDK
dotnet --version

# Сборка и запуск
cd src/ThreeInARow/ThreeInARow
dotnet restore
dotnet build
dotnet run
```

**Управление:**
- **Стрелки** ← ↑ → ↓ — перемещение курсора
- **Enter** — выбрать клетку / сделать ход
- **Esc** — отмена
- **Q** — выход

## Описание

Учебный прототип консольной игры "Три в ряд" на .NET 9 с архитектурой на основе **абстрактных типов данных (АТД)**.

## Технологии

- **.NET 9** — платформа
- **C# 12** — язык
- **Terminal.Gui 1.17.1** — текстовый интерфейс

## Архитектура

Четыре изолированных слоя:

```
UI (TerminalUI)
    
Application (GameController, GameStateMachine)
    
Core (GameBoard, GameSession, GameElement) + Data (Leaderboard)
```

**Принцип АТД:** Каждый класс = Интерфейс (контракт) + Реализация

## Структура файлов

```
ThreeInARow/
├── Core/               # Игровая логика
│   ├── IGameElement.cs, GameElement.cs
│   ├── IGameBoard.cs, GameBoard.cs
│   └── IGameSession.cs, GameSession.cs
├── Application/        # Управление
│   ├── IGameStateMachine.cs, GameStateMachine.cs
│   └── IGameController.cs, GameController.cs
├── Data/               # Хранение
│   └── ILeaderboard.cs, Leaderboard.cs
├── UI/                 # Интерфейс
│   └── TerminalUI.cs
└── Program.cs
```

## Игровой процесс

1. Выбрать элемент стрелками + **Enter**
2. Выбрать соседний элемент + **Enter**
3. Если 3+ в ряд → удаление + очки (10 за элемент)
4. Иначе → отмена обмена

**Элементы:** R (красный), B (синий), G (зелёный), Y (жёлтый), P (фиолетовый), O (оранжевый)

## Ключевые особенности

✓ Изоляция слоёв (Core не знает о UI)  
✓ Единая точка управления (GameController)  
✓ Интерфейсы АТД с формальными спецификациями  
✓ Контроль состояний (Menu → Playing → Finished)  
✓ Корректность ходов (откат при отсутствии совпадений)

## Требования

**Минимум:**
- .NET 9 SDK
- Windows 10+ / macOS 10.15+ / Linux
- 512 МБ ОЗУ

**Рекомендуется:**
- Windows Terminal / iTerm2 / GNOME Terminal
- 1 ГБ ОЗУ

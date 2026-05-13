using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsGame;

public partial class GameForm
{
    private const float LevelTimeLimitSeconds = 240f;
    private const int MazeSeed = 1337;

    private sealed class MazeCell
    {
        public bool Top = true;
        public bool Right = true;
        public bool Bottom = true;
        public bool Left = true;
        public bool Visited;
    }

    private enum Direction
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3
    }

    /// <summary>
    /// Строит лабиринт один раз для текущей клиентской области.
    /// Почему: Пользователь запросил один фиксированный лабиринт (одинаковый при каждом запуске), поэтому мы не перегенерируем его при перезапуске.
    /// </summary>
    private void EnsureMazeBuilt()
    {
        if (_isMazeBuilt)
        {
            return;
        }

        if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
        {
            return;
        }

        _obstacles.Clear();

        (int columns, int rows) = ChooseMazeGridSize(ClientSize);
        var cells = GenerateMazeCells(columns, rows, MazeSeed);

        float cellWidth = ClientSize.Width / (float)columns;
        float cellHeight = ClientSize.Height / (float)rows;

        AddMazeWallRectangles(cells, columns, rows, cellWidth, cellHeight);
        PlacePlayerAndFinish(columns, rows, cellWidth, cellHeight);
        ResetRun();

        _isMazeBuilt = true;
    }

    /// <summary>
    /// Сбрасывает текущую попытку без изменения планировки лабиринта.
    /// Почему: Позволяет повторить попытку прохождения на время, сохраняя лабиринт идентичным.
    /// </summary>
    private void ResetRun()
    {
        _player = _playerStart;
        _isWin = false;
        _isLose = false;
        _timeRemainingSeconds = LevelTimeLimitSeconds;
    }

    /// <summary>
    /// Выбирает подходящий размер сетки лабиринта для текущей клиентской области.
    /// Почему: Сохраняет стабильную сложность лабиринта на экранах разных размеров без необходимости использования ползунков в UI.
    /// </summary>
    private static (int columns, int rows) ChooseMazeGridSize(Size clientSize)
    {
        const int targetCellPixels = 48;
        int columns = Math.Clamp(clientSize.Width / targetCellPixels, 10, 60);
        int rows = Math.Clamp(clientSize.Height / targetCellPixels, 8, 45);
        return (columns, rows);
    }

    /// <summary>
    /// Строит идеальный лабиринт с использованием алгоритма Recursive Backtracker (на основе стека).
    /// Почему: Создает решаемый лабиринт со структурой одного связного пути, что отлично подходит для испытаний на время.
    /// </summary>
    private static MazeCell[,] GenerateMazeCells(int columns, int rows, int seed)
    {
        var cells = new MazeCell[columns, rows];
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                cells[x, y] = new MazeCell();
            }
        }

        var random = new Random(seed);
        var stack = new Stack<(int x, int y)>();

        cells[0, 0].Visited = true;
        stack.Push((0, 0));

        while (stack.Count > 0)
        {
            (int cx, int cy) = stack.Peek();
            var neighbors = GetUnvisitedNeighbors(cells, columns, rows, cx, cy);

            if (neighbors.Count == 0)
            {
                stack.Pop();
                continue;
            }

            (int nx, int ny, Direction dir) = neighbors[random.Next(neighbors.Count)];
            RemoveWallBetween(cells, cx, cy, nx, ny, dir);
            cells[nx, ny].Visited = true;
            stack.Push((nx, ny));
        }

        return cells;
    }

    private static List<(int x, int y, Direction dir)> GetUnvisitedNeighbors(
        MazeCell[,] cells,
        int columns,
        int rows,
        int x,
        int y)
    {
        var list = new List<(int x, int y, Direction dir)>(4);

        if (y > 0 && !cells[x, y - 1].Visited)
        {
            list.Add((x, y - 1, Direction.Top));
        }

        if (x < columns - 1 && !cells[x + 1, y].Visited)
        {
            list.Add((x + 1, y, Direction.Right));
        }

        if (y < rows - 1 && !cells[x, y + 1].Visited)
        {
            list.Add((x, y + 1, Direction.Bottom));
        }

        if (x > 0 && !cells[x - 1, y].Visited)
        {
            list.Add((x - 1, y, Direction.Left));
        }

        return list;
    }

    private static void RemoveWallBetween(MazeCell[,] cells, int cx, int cy, int nx, int ny, Direction dir)
    {
        switch (dir)
        {
            case Direction.Top:
                cells[cx, cy].Top = false;
                cells[nx, ny].Bottom = false;
                return;
            case Direction.Right:
                cells[cx, cy].Right = false;
                cells[nx, ny].Left = false;
                return;
            case Direction.Bottom:
                cells[cx, cy].Bottom = false;
                cells[nx, ny].Top = false;
                return;
            case Direction.Left:
                cells[cx, cy].Left = false;
                cells[nx, ny].Right = false;
                return;
            default:
                return;
        }
    }

    /// <summary>
    /// Преобразует стены лабиринта в прямоугольники препятствий, используемые существующей системой столкновений.
    /// Почему: Повторно использует текущую логику пересечения прямоугольников без внедрения новой физической системы.
    /// </summary>
    private void AddMazeWallRectangles(MazeCell[,] cells, int columns, int rows, float cellWidth, float cellHeight)
    {
        float thickness = Math.Max(4f, Math.Min(cellWidth, cellHeight) * 0.12f);

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                var cell = cells[x, y];
                float left = x * cellWidth;
                float top = y * cellHeight;
                float right = left + cellWidth;
                float bottom = top + cellHeight;

                if (cell.Top)
                {
                    _obstacles.Add(new RectangleF(left, top, cellWidth, thickness));
                }

                if (cell.Left)
                {
                    _obstacles.Add(new RectangleF(left, top, thickness, cellHeight));
                }

                if (x == columns - 1 && cell.Right)
                {
                    _obstacles.Add(new RectangleF(right - thickness, top, thickness, cellHeight));
                }

                if (y == rows - 1 && cell.Bottom)
                {
                    _obstacles.Add(new RectangleF(left, bottom - thickness, cellWidth, thickness));
                }
            }
        }
    }

    /// <summary>
    /// Размещает игрока в верхней левой ячейке, а финиш — ближе к нижней правой.
    /// Почему: Соответствует запрошенному направлению уровня и делает старт/цель очевидными для игрока.
    /// </summary>
    private void PlacePlayerAndFinish(int columns, int rows, float cellWidth, float cellHeight)
    {
        float marginX = cellWidth * 0.20f;
        float marginY = cellHeight * 0.20f;
        float playerWidth = Math.Max(12f, cellWidth * 0.55f);
        float playerHeight = Math.Max(12f, cellHeight * 0.55f);

        _playerStart = new RectangleF(marginX, marginY, playerWidth, playerHeight);
        _player = _playerStart;

        int finishCellX = Math.Max(0, columns - 2);
        int finishCellY = Math.Max(0, rows - 2);

        float finishLeft = finishCellX * cellWidth + marginX;
        float finishTop = finishCellY * cellHeight + marginY;
        float finishWidth = cellWidth - marginX * 2f;
        float finishHeight = cellHeight - marginY * 2f;
        _finish = new RectangleF(finishLeft, finishTop, finishWidth, finishHeight);
    }
}


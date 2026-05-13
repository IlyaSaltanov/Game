using System;
using System.Drawing;

namespace WinFormsGame;

public partial class GameForm
{
    /// <summary>
    /// Инициализирует статические объекты мира и позицию появления игрока.
    /// Почему: Хранит настройку карты в одном месте, чтобы настройка игрового процесса не смешивалась с UI или кодом цикла.
    /// </summary>
    private void InitializeGameWorld()
    {
        // Лабиринт строится при первом показе (Show), когда известен окончательный размер клиентской области.
    }

    /// <summary>
    /// Подключает события таймера и жизненного цикла формы, которые управляют игровым циклом.
    /// Почему: Делает привязку игрового цикла явной и отделяет ее от логики конструктора.
    /// </summary>
    private void InitializeGameLoop()
    {
        _timer.Tick += OnTick;
        Load += OnLoad;
        Shown += OnShown;
    }

    /// <summary>
    /// Запускает таймер при загрузке формы.
    /// Почему: Откладывает запуск цикла до завершения создания формы для стабильного тайминга кадров.
    /// </summary>
    private void OnLoad(object? sender, EventArgs e)
    {
        _lastTick = DateTime.Now;
        _timer.Start();
    }

    /// <summary>
    /// Переводит фокус клавиатуры на форму после ее появления и строит уровень.
    /// Почему: Гарантирует, что ввод с клавиатуры работает сразу, а лабиринт использует окончательный размер клиентской области.
    /// </summary>
    private void OnShown(object? sender, EventArgs e)
    {
        EnsureMazeBuilt();
        Focus();
    }
}


using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsGame;

    /// <summary>
    /// Отслеживает нажатые клавиши перемещения и предоставляет направление движения для обновлений игрового процесса.
    /// Почему: Изолирует состояние ввода от логики формы, чтобы код игры оставался сфокусированным на симуляции и отрисовке.
    /// </summary>
public sealed class InputController
{
    private readonly HashSet<Keys> _pressedKeys = new HashSet<Keys>();

    /// <summary>
    /// Отмечает клавишу клавиатуры как нажатую.
    /// Почему: Игровой цикл опирается на текущий снимок состояния клавиш, а не на перемещение по каждому событию.
    /// </summary>
    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        _pressedKeys.Add(e.KeyCode);
        e.Handled = true;
    }

    /// <summary>
    /// Отмечает клавишу клавиатуры как отпущенную.
    /// Почему: Предотвращает зависание состояния нажатия после отпускания клавиши.
    /// </summary>
    public void OnKeyUp(object? sender, KeyEventArgs e)
    {
        _pressedKeys.Remove(e.KeyCode);
        e.Handled = true;
    }

    /// <summary>
    /// Формирует нормализованное направление движения из текущих нажатых клавиш со стрелками.
    /// Почему: Хранит математику направлений в одном месте и предотвращает ускоренное движение по диагонали.
    /// </summary>
    /// <returns>Кортеж с направлениями X и Y в диапазоне -1..1.</returns>
    public (float x, float y) GetMoveDirection()
    {
        float inputX = 0f;
        float inputY = 0f;

        if (_pressedKeys.Contains(Keys.Left))
        {
            inputX -= 1f;
        }

        if (_pressedKeys.Contains(Keys.Right))
        {
            inputX += 1f;
        }

        if (_pressedKeys.Contains(Keys.Up))
        {
            inputY -= 1f;
        }

        if (_pressedKeys.Contains(Keys.Down))
        {
            inputY += 1f;
        }

        NormalizeDiagonal(ref inputX, ref inputY);
        return (inputX, inputY);
    }

    /// <summary>
    /// Нормализует диагональный вектор для сохранения постоянной скорости движения.
    /// Почему: Без нормализации движение по диагонали становится быстрее, чем по горизонтали или вертикали.
    /// </summary>
    private static void NormalizeDiagonal(ref float inputX, ref float inputY)
    {
        if (inputX == 0f || inputY == 0f)
        {
            return;
        }

        float scale = 1f / (float)Math.Sqrt(2);
        inputX *= scale;
        inputY *= scale;
    }
}

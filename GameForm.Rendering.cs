using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsGame;

public partial class GameForm
{
    /// <summary>
    /// Отрисовывает игровой мир и игрока каждый кадр.
    /// Почему: Изолирует отрисовку от обновлений игрового процесса для более понятного обслуживания.
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var graphics = e.Graphics;
        graphics.Clear(Color.Black);

        DrawObstacles(graphics);
        DrawFinish(graphics);
        DrawPlayer(graphics);
        DrawHud(graphics);
    }

    /// <summary>
    /// Отрисовывает все статические препятствия.
    /// Почему: Разделение позволяет сохранить основной метод отрисовки коротким и легко читаемым.
    /// </summary>
    private void DrawObstacles(Graphics graphics)
    {
        using var obstacleBrush = new SolidBrush(Color.DimGray);
        using var obstaclePen = new Pen(Color.Gray, 2);

        for (int i = 0; i < _obstacles.Count; i++)
        {
            graphics.FillRectangle(obstacleBrush, _obstacles[i]);
            graphics.DrawRectangle(obstaclePen, Rectangle.Round(_obstacles[i]));
        }
    }

    /// <summary>
    /// Отрисовывает прямоугольник игрока.
    /// Почему: Выделенный метод упрощает будущую замену спрайта игрока.
    /// </summary>
    private void DrawPlayer(Graphics graphics)
    {
        using var playerBrush = new SolidBrush(Color.Lime);
        graphics.FillRectangle(playerBrush, _player);
    }

    /// <summary>
    /// Отрисовывает финишную зону.
    /// Почему: Дает четкую визуальную цель для прохождения на время.
    /// </summary>
    private void DrawFinish(Graphics graphics)
    {
        using var finishBrush = new SolidBrush(Color.FromArgb(180, 60, 170, 255));
        using var finishPen = new Pen(Color.DeepSkyBlue, 2);
        graphics.FillRectangle(finishBrush, _finish);
        graphics.DrawRectangle(finishPen, Rectangle.Round(_finish));
    }

    /// <summary>
    /// Отрисовывает текст интерфейса (управление + таймер + состояние победы/поражения).
    /// Почему: Геймплей на время требует постоянной обратной связи по времени и простого процесса перезапуска.
    /// </summary>
    private void DrawHud(Graphics graphics)
    {
        using var font = new Font("Segoe UI", 12);

        string baseText = $"Время: {(int)Math.Ceiling(_timeRemainingSeconds)}   Стрелки: двигать   Esc: выйти   R: рестарт (без смены лабиринта)";
        graphics.DrawString(baseText, font, Brushes.White, 10, 10);

        if (_isWin)
        {
            using var bigFont = new Font("Segoe UI", 28, FontStyle.Bold);
            graphics.DrawString("ПОБЕДА!", bigFont, Brushes.Lime, 10, 46);
        }
        else if (_isLose)
        {
            using var bigFont = new Font("Segoe UI", 28, FontStyle.Bold);
            graphics.DrawString("ВРЕМЯ ВЫШЛО!", bigFont, Brushes.OrangeRed, 10, 46);
        }
    }
}


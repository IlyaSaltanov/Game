using System;
using System.Windows.Forms;

namespace WinFormsGame;

public partial class GameForm
{
    /// <summary>
    /// Продвигает симуляцию на один кадр и запрашивает перерисовку.
    /// Почему: Централизует расчет времени кадра, чтобы обновление и отрисовка оставались синхронизированными.
    /// </summary>
    private void OnTick(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        float deltaSeconds = (float)(now - _lastTick).TotalSeconds;
        _lastTick = now;

        UpdateGame(deltaSeconds);
        Invalidate();
    }
}


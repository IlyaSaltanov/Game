using System.Drawing;
using System.Windows.Forms;

namespace WinFormsGame;

public partial class GameForm
{
    /// <summary>
    /// Настраивает параметры окна для формы игры.
    /// Почему: Восстанавливает стандартные элементы управления окном (закрыть/свернуть/развернуть), чтобы приложение всегда можно было свернуть или закрыть.
    /// </summary>
    private void ConfigureWindow()
    {
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.Sizable;
        ControlBox = true;
        MinimizeBox = true;
        MaximizeBox = true;
        WindowState = FormWindowState.Maximized;

        Text = "WinForms Simple Game Template";
        DoubleBuffered = true;
        KeyPreview = true;

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.UserPaint |
            ControlStyles.ResizeRedraw,
            true);
    }
}


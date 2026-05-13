namespace WinFormsGame;

public partial class GameForm
{
    /// <summary>
    /// Регистрирует обработчики клавиатуры для ввода перемещения.
    /// Почему: Хранит привязку ввода в одном месте и избегает загромождения конструктора.
    /// </summary>
    private void InitializeInputHandling()
    {
        KeyDown += (_, e) =>
        {
            if (e.KeyCode != System.Windows.Forms.Keys.Escape)
            {
                return;
            }

            Close();
            e.Handled = true;
        };

        KeyDown += (_, e) =>
        {
            if (e.KeyCode != System.Windows.Forms.Keys.R)
            {
                return;
            }

            ResetRun();
            e.Handled = true;
        };

        KeyDown += _inputController.OnKeyDown;
        KeyUp += _inputController.OnKeyUp;
    }
}


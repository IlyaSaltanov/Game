namespace WinFormsGame;

static class Program
{
    /// <summary>
    ///  Главная точка входа для приложения.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Чтобы настроить конфигурацию приложения, например, задать настройки высокого DPI или шрифт по умолчанию,
        // см. https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new GameForm());
    }    
}
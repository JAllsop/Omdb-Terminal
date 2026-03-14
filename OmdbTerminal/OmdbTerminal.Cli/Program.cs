using OmdbTerminal.Cli.Gui;
using Terminal.Gui;

namespace OmdbTerminal.Cli;

class Program
{
    static void Main(string[] args)
    {
        Application.Init();

        // Define a dark theme color scheme
        Colors.Base.Normal = Application.Driver.MakeAttribute(Color.Gray, Color.DarkGray);
        Colors.Base.Focus = Application.Driver.MakeAttribute(Color.White, Color.Black);
        Colors.Base.HotNormal = Application.Driver.MakeAttribute(Color.BrightCyan, Color.DarkGray);
        Colors.Base.HotFocus = Application.Driver.MakeAttribute(Color.BrightCyan, Color.Black);

        Colors.Dialog.Normal = Application.Driver.MakeAttribute(Color.Gray, Color.Black);
        Colors.Dialog.Focus = Application.Driver.MakeAttribute(Color.White, Color.DarkGray);
        Colors.Dialog.HotNormal = Application.Driver.MakeAttribute(Color.BrightCyan, Color.Black);
        Colors.Dialog.HotFocus = Application.Driver.MakeAttribute(Color.BrightCyan, Color.DarkGray);

        Colors.Menu.Normal = Application.Driver.MakeAttribute(Color.Gray, Color.Black);
        Colors.Menu.Focus = Application.Driver.MakeAttribute(Color.White, Color.DarkGray);
        Colors.Menu.HotNormal = Application.Driver.MakeAttribute(Color.BrightCyan, Color.Black);
        Colors.Menu.HotFocus = Application.Driver.MakeAttribute(Color.BrightCyan, Color.DarkGray);

        Application.Top.ColorScheme = Colors.Base;

        Application.Run(new VersionSelectWindow());
        Application.Shutdown();
    }
}
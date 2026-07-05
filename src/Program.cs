using LuaResourceManager.Commands;
using LuaResourceManager.Tui;
using Spectre.Console;

namespace LuaResourceManager;

class Program
{
    static int Main(string[] args)
    {
        AnsiConsole.Clear();

        while (true)
        {
            var commandName = MainMenu.Show();

            switch (commandName)
            {
                case "New Resource":
                    NewCommand.Execute(args);
                    break;

                case "Create Web":
                    CreateWebCommand.Execute(args);
                    break;

                case "Convert PX":
                    ConvertPxCommand.Execute(args);
                    break;

                case "Exit":
                    return 0;
            }
        }
    }
}

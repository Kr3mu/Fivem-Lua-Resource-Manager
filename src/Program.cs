using LuaResourceManager.Commands;
using LuaResourceManager.Helpers;
using Spectre.Console;

namespace LuaResourceManager;

class Program
{
    static int Main(string[] args)
    {
        AnsiConsole.Clear();

        while (true)
        {
            var commandName = Tui.ShowMainMenu();

            switch (commandName)
            {
                case "New Resource":
                    NewCommand.Execute(args);
                    break;

                case "Exit":
                    return 0;
            }
        }
    }
}

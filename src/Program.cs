using LuaResourceManager.Commands;
using LuaResourceManager.Tui;
using Spectre.Console;

namespace LuaResourceManager;

class Program
{
    static int Main(string[] args)
    {
        Console.Write("\u001b[?1049h");
        Console.CancelKeyPress += (_, _) =>
        {
            Console.Write("\u001b[?1049l");
            AnsiConsole.Cursor.Show();
        };

        try
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
        finally
        {
            Console.Write("\u001b[?1049l");
        }
    }
}

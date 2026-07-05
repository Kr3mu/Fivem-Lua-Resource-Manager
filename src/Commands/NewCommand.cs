using Spectre.Console;
using LuaResourceManager.Helpers;

namespace LuaResourceManager.Commands;

public static class NewCommand
{
    public static int Execute(string[] args)
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(
            new FigletText("LRM")
                .Color(Color.Orange1)
                .Centered()
        );

        AnsiConsole.Write(
            new Rule("[orange1]New Resource[/]")
                .Centered()
        );

        AnsiConsole.WriteLine();

        var name = AnsiConsole.Ask<string>("[orange1]Resource name:[/]");

        ConsoleHelper.Success($"Resource '{name}' would be created here.");

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
        Console.ReadKey(true);

        return 0;
    }
}

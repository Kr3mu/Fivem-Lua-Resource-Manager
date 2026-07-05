using Spectre.Console;

namespace LuaResourceManager.Helpers;

public static class Tui
{
    private readonly record struct MenuOption(string Name, string Description);

    public static string ShowMainMenu()
    {
        AnsiConsole.Cursor.Hide();

        try
        {
            var options = new[]
            {
                new MenuOption("New Resource", "Create a new FiveM Lua resource"),
                new MenuOption("Exit", "Quit the application")
            };

            var selected = 0;
            var lastWidth = -1;
            var lastHeight = -1;

            while (true)
            {
                var width = GetConsoleWidth();
                var height = GetConsoleHeight();

                if (width != lastWidth || height != lastHeight)
                {
                    lastWidth = width;
                    lastHeight = height;
                    Render(options, selected);
                }

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                        case ConsoleKey.K:
                            selected = (selected - 1 + options.Length) % options.Length;
                            Render(options, selected);
                            break;

                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                        case ConsoleKey.J:
                            selected = (selected + 1) % options.Length;
                            Render(options, selected);
                            break;

                        case ConsoleKey.Enter:
                            return options[selected].Name;
                    }
                }

                Thread.Sleep(50);
            }
        }
        finally
        {
            AnsiConsole.Cursor.Show();
        }
    }

    private static void Render(MenuOption[] options, int selected)
    {
        Console.Write("\u001b[H");

        AnsiConsole.Write(
            new FigletText("LRM")
                .Color(Color.Orange1)
                .Centered()
        );

        AnsiConsole.Write(
            new Rule("[orange1]FiveM Lua Resource Manager[/]")
                .Centered()
        );

        AnsiConsole.WriteLine();

        var table = new Table()
            .Border(TableBorder.None)
            .Expand();

        table.AddColumn(new TableColumn("[orange1]Command[/]").Width(15).Centered());
        table.AddColumn(new TableColumn("[orange1]Description[/]"));

        for (var i = 0; i < options.Length; i++)
        {
            var option = options[i];
            var isSelected = i == selected;
            var prefix = isSelected ? "[orange1]\u003e[/]" : " ";
            var name = isSelected ? $"[orange1]{option.Name}[/]" : option.Name;
            var desc = isSelected ? $"[orange1]{option.Description}[/]" : option.Description;

            table.AddRow($" {prefix} {name}", desc);
        }

        var panel = new Panel(table)
            .Header(" Main Menu ", Justify.Center)
            .BorderColor(Color.Orange1)
            .Padding(1, 0);

        panel.Width = Math.Min(70, Math.Max(20, Console.WindowWidth - 4));

        AnsiConsole.Write(
            Align.Center(panel, VerticalAlignment.Top)
                .Width(Console.WindowWidth)
        );

        AnsiConsole.WriteLine();
        AnsiConsole.Write(
            Align.Center(
                new Markup("[grey]↑/↓ • W/S • J/K • Enter[/]"),
                VerticalAlignment.Top
            ).Width(Console.WindowWidth)
        );

        Console.Write("\u001b[J");
    }

    private static int GetConsoleWidth()
    {
        try
        {
            return Console.WindowWidth;
        }
        catch
        {
            return 80;
        }
    }

    private static int GetConsoleHeight()
    {
        try
        {
            return Console.WindowHeight;
        }
        catch
        {
            return 24;
        }
    }
}

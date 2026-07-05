using Spectre.Console;

namespace LuaResourceManager.Helpers;

public static class ConsoleHelper
{
    public static void Success(string message)
    {
        AnsiConsole.MarkupLine($"[green]✓ {message}[/]");
    }

    public static void Error(string message)
    {
        AnsiConsole.MarkupLine($"[orange1]✗ {message}[/]");
    }
}

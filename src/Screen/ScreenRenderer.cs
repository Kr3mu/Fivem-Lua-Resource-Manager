using Spectre.Console;

namespace LuaResourceManager.Screen;

public static class ScreenRenderer
{
    private static int _lastWidth = -1;
    private static int _lastHeight = -1;

    public static bool ForceClearNextFrame { get; set; }

    public static void BeginFrame()
    {
        var width = GetConsoleWidth();
        var height = GetConsoleHeight();
        var resized = width != _lastWidth || height != _lastHeight;

        _lastWidth = width;
        _lastHeight = height;

        var clear = resized || ForceClearNextFrame;
        ForceClearNextFrame = false;

        Console.Write(clear ? "\u001b[2J\u001b[H" : "\u001b[H");
        AnsiConsole.Cursor.Hide();
    }

    public static void EndFrame()
    {
        Console.Write("\u001b[J");
    }

    public static void RequestFullClear()
    {
        ForceClearNextFrame = true;
    }

    public static void RenderHeader(string subtitle)
    {
        AnsiConsole.Write(
            new FigletText("LRM")
                .Color(Color.Orange1)
                .Centered()
        );

        AnsiConsole.Write(
            new Rule($"[orange1]{subtitle}[/]")
                .Centered()
        );

        AnsiConsole.WriteLine();
    }

    public static void RenderFooter(string text)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(
            Align.Center(
                new Markup($"[grey]{text}[/]"),
                VerticalAlignment.Top
            ).Width(Console.WindowWidth)
        );
    }

    public static void RenderCentered(Panel panel)
    {
        AnsiConsole.Write(
            Align.Center(panel, VerticalAlignment.Top)
                .Width(Console.WindowWidth)
        );
    }

    public static int GetConsoleWidth()
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

    public static int GetConsoleHeight()
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

using Spectre.Console;

namespace LuaResourceManager.Screen;

public static class ScreenRenderer
{
    public static void BeginFrame()
    {
        Console.Write("\u001b[H");
        AnsiConsole.Cursor.Hide();
    }

    public static void EndFrame()
    {
        Console.Write("\u001b[J");
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

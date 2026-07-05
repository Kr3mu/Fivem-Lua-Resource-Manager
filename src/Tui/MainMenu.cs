using LuaResourceManager.Screen;

namespace LuaResourceManager.Tui;

public static class MainMenu
{
    public static string Show()
    {
        using var _ = new CursorScope();

        var options = new[] { "New Resource", "Exit" };
        var selected = 0;
        var lastWidth = -1;
        var lastHeight = -1;

        while (true)
        {
            var width = ScreenRenderer.GetConsoleWidth();
            var height = ScreenRenderer.GetConsoleHeight();

            if (width != lastWidth || height != lastHeight)
            {
                lastWidth = width;
                lastHeight = height;
                TuiRenderer.RenderMainMenu(options, selected, width);
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
                        TuiRenderer.RenderMainMenu(options, selected, width);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.J:
                        selected = (selected + 1) % options.Length;
                        TuiRenderer.RenderMainMenu(options, selected, width);
                        break;

                    case ConsoleKey.Enter:
                        return options[selected];
                }
            }

            Thread.Sleep(50);
        }
    }
}

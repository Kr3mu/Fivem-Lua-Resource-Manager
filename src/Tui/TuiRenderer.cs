using LuaResourceManager.Screen;
using Spectre.Console;

namespace LuaResourceManager.Tui;

internal static class TuiRenderer
{
    public static void RenderMainMenu(IReadOnlyList<string> options, int selected, int width)
    {
        ScreenRenderer.BeginFrame();

        ScreenRenderer.RenderHeader("FiveM Lua Resource Manager");

        var table = new Table()
            .Border(TableBorder.None)
            .Expand();

        table.AddColumn(new TableColumn("[orange1]Command[/]").Width(15).Centered());
        table.AddColumn(new TableColumn("[orange1]Description[/]"));

        for (var i = 0; i < options.Count; i++)
        {
            var option = options[i];
            var isSelected = i == selected;
            var prefix = isSelected ? "[orange1]\u003e[/]" : " ";
            var name = isSelected ? $"[orange1]{option}[/]" : option;
            var desc = isSelected ? $"[orange1]{GetDescription(option)}[/]" : GetDescription(option);

            table.AddRow($" {prefix} {name}", desc);
        }

        var panel = new Panel(table)
            .Header(" Main Menu ", Justify.Center)
            .BorderColor(Color.Orange1)
            .Padding(1, 0);

        panel.Width = Math.Min(70, Math.Max(20, width - 4));

        ScreenRenderer.RenderCentered(panel);

        ScreenRenderer.RenderFooter("↑/↓ • W/S • J/K • Enter");

        ScreenRenderer.EndFrame();
    }

    private static string GetDescription(string option) => option switch
    {
        "New Resource" => "Create a new FiveM Lua resource",
        "Create Web" => "Scaffold Svelte + Tailwind web app",
        "Exit" => "Quit the application",
        _ => ""
    };
}

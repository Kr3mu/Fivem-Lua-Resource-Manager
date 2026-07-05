using LuaResourceManager.Screen;
using Spectre.Console;

namespace LuaResourceManager.Forms;

internal static class ResourceFormRenderer
{
    public static void Render(IReadOnlyList<ResourceFormRow> rows, int selected, string? error)
    {
        ScreenRenderer.BeginFrame();

        ScreenRenderer.RenderHeader("New Resource");

        var table = new Table()
            .Border(TableBorder.None)
            .Expand();

        table.AddColumn(new TableColumn("[orange1]Field[/]").Width(25));
        table.AddColumn(new TableColumn("[orange1]Value[/]"));

        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var isSelected = i == selected;

            if (row is Button)
            {
                var label = isSelected ? $"[orange1]> {row.Label}[/]" : $"  {row.Label}";
                table.AddRow("", label);
            }
            else
            {
                var label = isSelected ? $"[orange1]{row.Label}[/]" : row.Label;
                var value = isSelected ? $"[orange1]{row.DisplayValue}[/]" : row.DisplayValue;
                table.AddRow(label, value);
            }
        }

        if (!string.IsNullOrEmpty(error))
        {
            table.AddRow("", $"[orange1]{error}[/]");
        }

        var panel = new Panel(table)
            .Header(" Resource Form ", Justify.Center)
            .BorderColor(Color.Orange1)
            .Padding(1, 0);

        panel.Width = Math.Min(70, Math.Max(20, ScreenRenderer.GetConsoleWidth() - 4));

        ScreenRenderer.RenderCentered(panel);

        ScreenRenderer.RenderFooter("↑/↓ • W/S • J/K • Enter (toggle/check) • Esc");

        ScreenRenderer.EndFrame();
    }
}

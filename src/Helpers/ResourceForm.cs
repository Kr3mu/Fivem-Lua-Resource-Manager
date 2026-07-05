using System.Text.RegularExpressions;
using Spectre.Console;

namespace LuaResourceManager.Helpers;

public record ResourceFormResult(
    string Name,
    string Author,
    string Description,
    bool UseFxv2Oal,
    bool CreateConfigs,
    bool UseDoubleQuotes);

public static partial class ResourceForm
{
    [GeneratedRegex("^[a-zA-Z0-9_-]+$")]
    private static partial Regex ValidResourceName();

    private abstract class Row(string label)
    {
        public string Label { get; } = label;
        public abstract string DisplayValue { get; }
    }

    private sealed class TextField(string label, string value) : Row(label)
    {
        public string Value { get; set; } = value;
        public override string DisplayValue => string.IsNullOrEmpty(Value) ? "—" : Value;
    }

    private sealed class Toggle(string label, bool value) : Row(label)
    {
        public bool Value { get; set; } = value;
        public override string DisplayValue => Value ? "[[x]] yes" : "[[ ]] no";
    }

    private sealed class Button(string label) : Row(label)
    {
        public override string DisplayValue => "";
    }

    public static ResourceFormResult? Show()
    {
        AnsiConsole.Cursor.Hide();

        try
        {
            var rows = new List<Row>
            {
                new TextField("Resource Name", ""),
                new TextField("Author", ""),
                new TextField("Description", ""),
                new Toggle("Use experimental fxv2_oal", true),
                new Toggle("Create configs folder", true),
                new Toggle("Use double quotes", false),
                new Button("Create"),
                new Button("Cancel")
            };

            var selected = 0;
            string? error = null;

            while (true)
            {
                Render(rows, selected, error);
                error = null;

                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(50);
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    case ConsoleKey.K:
                        selected = (selected - 1 + rows.Count) % rows.Count;
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.J:
                        selected = (selected + 1) % rows.Count;
                        break;

                    case ConsoleKey.Escape:
                        return null;

                    case ConsoleKey.Enter:
                        var row = rows[selected];

                        switch (row)
                        {
                            case TextField field:
                                var newValue = PromptForField(field.Label, field.Value, field.Label == "Resource Name");
                                field.Value = field.Label == "Resource Name" ? newValue.ToLowerInvariant() : newValue;
                                break;

                            case Toggle toggle:
                                toggle.Value = !toggle.Value;
                                break;

                            case Button { Label: "Create" }:
                                var result = ValidateAndBuild(rows);

                                if (result is null)
                                {
                                    error = "Resource name cannot be empty and may only contain letters, numbers, underscores, and hyphens.";
                                    break;
                                }

                                if (ConfirmSummary(result))
                                {
                                    return result;
                                }

                                break;

                            case Button { Label: "Cancel" }:
                                return null;
                        }

                        break;
                }
            }
        }
        finally
        {
            AnsiConsole.Cursor.Show();
        }
    }

    private static ResourceFormResult? ValidateAndBuild(List<Row> rows)
    {
        var name = ((TextField)rows[0]).Value.Trim();
        var author = ((TextField)rows[1]).Value.Trim();
        var description = ((TextField)rows[2]).Value.Trim();
        var fxv2 = ((Toggle)rows[3]).Value;
        var configs = ((Toggle)rows[4]).Value;
        var quotes = ((Toggle)rows[5]).Value;

        if (string.IsNullOrWhiteSpace(name) || !ValidResourceName().IsMatch(name))
        {
            return null;
        }

        return new ResourceFormResult(name.ToLowerInvariant(), author, description, fxv2, configs, quotes);
    }

    private static string PromptForField(string label, string currentValue, bool required)
    {
        var prompt = new TextPrompt<string>($"[orange1]{label}:[/]");

        if (!required)
        {
            prompt.AllowEmpty();
        }
        else
        {
            prompt.Validate(value =>
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return ValidationResult.Error($"{label} cannot be empty.");
                }

                if (!ValidResourceName().IsMatch(value))
                {
                    return ValidationResult.Error($"{label} can only contain letters, numbers, underscores, and hyphens.");
                }

                return ValidationResult.Success();
            });
        }

        if (!string.IsNullOrEmpty(currentValue))
        {
            prompt.DefaultValue(currentValue);
        }

        return AnsiConsole.Prompt(prompt);
    }

    private static bool ConfirmSummary(ResourceFormResult result)
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(
            new FigletText("LRM")
                .Color(Color.Orange1)
                .Centered()
        );

        AnsiConsole.Write(
            new Rule("[orange1]Summary[/]")
                .Centered()
        );

        AnsiConsole.WriteLine();

        var table = new Table()
            .Border(TableBorder.None)
            .Expand();

        table.AddColumn(new TableColumn("[orange1]Field[/]").Width(20));
        table.AddColumn(new TableColumn("[orange1]Value[/]"));

        table.AddRow("Resource Name", result.Name);
        table.AddRow("Author", string.IsNullOrEmpty(result.Author) ? "[grey]—[/]" : result.Author);
        table.AddRow("Description", string.IsNullOrEmpty(result.Description) ? "[grey]—[/]" : result.Description);
        table.AddRow("fxv2_oal", result.UseFxv2Oal ? "yes" : "no");
        table.AddRow("Configs", result.CreateConfigs ? "yes" : "no");
        table.AddRow("Double quotes", result.UseDoubleQuotes ? "yes" : "no");

        var panel = new Panel(table)
            .Header(" Review ", Justify.Center)
            .BorderColor(Color.Orange1)
            .Padding(1, 0);

        panel.Width = Math.Min(70, Math.Max(20, Console.WindowWidth - 4));

        AnsiConsole.Write(
            Align.Center(panel, VerticalAlignment.Top)
                .Width(Console.WindowWidth)
        );

        AnsiConsole.WriteLine();

        return AnsiConsole.Confirm("Create resource?");
    }

    private static void Render(List<Row> rows, int selected, string? error)
    {
        Console.Write("\u001b[H");
        AnsiConsole.Cursor.Hide();

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

        panel.Width = Math.Min(70, Math.Max(20, Console.WindowWidth - 4));

        AnsiConsole.Write(
            Align.Center(panel, VerticalAlignment.Top)
                .Width(Console.WindowWidth)
        );

        AnsiConsole.WriteLine();
        AnsiConsole.Write(
            Align.Center(
                new Markup("[grey]↑/↓ • W/S • J/K • Enter (toggle/check) • Esc[/]"),
                VerticalAlignment.Top
            ).Width(Console.WindowWidth)
        );

        Console.Write("\u001b[J");
    }
}

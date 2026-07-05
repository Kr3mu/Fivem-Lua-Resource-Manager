using LuaResourceManager.Helpers;
using LuaResourceManager.Screen;
using Spectre.Console;

namespace LuaResourceManager.Forms;

internal static class ResourceFormInput
{
    public static string PromptForField(string label, string currentValue, bool required)
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

                if (!ResourceValidation.ResourceNameRegex().IsMatch(value))
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

        Console.WriteLine();
        var result = AnsiConsole.Prompt(prompt);
        Console.Write("\u001b[2J\u001b[H");
        Console.Out.Flush();
        return result;
    }

    public static bool ConfirmSummary(ResourceFormResult result)
    {
        AnsiConsole.Clear();

        ScreenRenderer.RenderHeader("Summary");

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

        panel.Width = Math.Min(70, Math.Max(20, ScreenRenderer.GetConsoleWidth() - 4));

        ScreenRenderer.RenderCentered(panel);

        AnsiConsole.WriteLine();

        return AnsiConsole.Confirm("Create resource?");
    }
}

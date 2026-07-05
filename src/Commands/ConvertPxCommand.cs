using LuaResourceManager.Helpers;
using Spectre.Console;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LuaResourceManager.Commands;

public static class ConvertPxCommand
{
    private static readonly Regex PxPattern = new(@"(\d+\.?\d*)px", RegexOptions.Compiled);

    public static int Execute(string[] args)
    {
        AnsiConsole.Clear();

        var unit = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[orange1]Convert px to which unit?[/]")
                .AddChoices("vh", "rem"));

        var width = AnsiConsole.Ask<int>("Enter base screen [green]width[/] in pixels:");
        var height = AnsiConsole.Ask<int>("Enter base screen [green]height[/] in pixels:");

        if (height <= 0 || width <= 0)
        {
            ConsoleHelper.Error("Base resolution width and height must be greater than 0.");
            WaitForKey();
            return 1;
        }

        var oneVh = (double)height / 100.0;
        double pxPerUnit;

        if (unit == "vh")
        {
            pxPerUnit = oneVh;
        }
        else
        {
            var remInVh = AnsiConsole.Ask<double>("How many vh is [green]1rem[/]? (e.g. 1.2):");

            if (remInVh <= 0)
            {
                ConsoleHelper.Error("1rem value must be greater than 0.");
                WaitForKey();
                return 1;
            }

            pxPerUnit = remInVh * oneVh;
        }

        var resourcePath = Directory.GetCurrentDirectory();
        var webPath = Path.Combine(resourcePath, "web");

        if (!Directory.Exists(webPath))
        {
            ConsoleHelper.Error("No 'web' folder found in this resource.");
            WaitForKey();
            return 1;
        }

        var extensions = new[] { ".css", ".scss", ".less", ".svelte", ".tsx", ".ts", ".jsx", ".js", ".vue" };
        var excludedDirs = new[] { "node_modules", "dist", ".git" };
        var files = Directory
            .EnumerateFiles(webPath, "*.*", SearchOption.AllDirectories)
            .Where(file =>
            {
                var relativePath = Path.GetRelativePath(webPath, file);
                var parts = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                return parts.Any(excludedDirs.Contains) == false &&
                       extensions.Contains(Path.GetExtension(file).ToLowerInvariant());
            })
            .ToList();

        var replacedCount = 0;
        var fileCount = 0;

        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            var newContent = PxPattern.Replace(content, match => ReplacePxValue(match, unit, pxPerUnit));

            if (newContent != content)
            {
                File.WriteAllText(file, newContent);
                fileCount++;
                replacedCount += PxPattern.Matches(content).Count;
            }
        }

        if (fileCount > 0)
        {
            ConsoleHelper.Success($"Replaced {replacedCount} px values in {fileCount} file(s) with {unit}.");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]No px values found to convert.[/]");
        }

        WaitForKey();
        return 0;
    }

    private static string ReplacePxValue(Match match, string unit, double pxPerUnit)
    {
        var pxValue = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        var converted = pxValue / pxPerUnit;
        var rounded = Math.Round(converted, 3);
        var formatted = rounded.ToString("0.###", CultureInfo.InvariantCulture);
        return $"{formatted}{unit}";
    }

    private static void WaitForKey()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
        Console.ReadKey(true);
    }
}

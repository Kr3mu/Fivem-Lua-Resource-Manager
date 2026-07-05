using LuaResourceManager.Forms;
using LuaResourceManager.Helpers;
using LuaResourceManager.ResourceGeneration;
using Spectre.Console;

namespace LuaResourceManager.Commands;

public static class NewCommand
{
    public static int Execute(string[] args)
    {
        AnsiConsole.Clear();

        var result = ResourceForm.Show();

        if (result is null)
        {
            return 0;
        }

        try
        {
            ResourceCreator.Create(result);
            ConsoleHelper.Success($"Created resource '{result.Name}'");
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error(ex.Message);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
        Console.ReadKey(true);

        return 0;
    }
}

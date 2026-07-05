using LuaResourceManager.Forms;
using LuaResourceManager.Helpers;
using LuaResourceManager.WebGeneration;
using Spectre.Console;

namespace LuaResourceManager.Commands;

public static class CreateWebCommand
{
    public static int Execute(string[] args)
    {
        AnsiConsole.Clear();

        var result = WebForm.Show();

        if (result is null)
        {
            return 0;
        }

        try
        {
            WebCreator.Create(result);
            ConsoleHelper.Success("Created web app for this resource");
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

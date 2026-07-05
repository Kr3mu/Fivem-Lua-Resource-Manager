using Spectre.Console;

namespace LuaResourceManager.Forms;

public static class WebForm
{
    public static WebFormResult? Show()
    {
        var font = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[orange1]Choose a font[/]")
                .AddChoices("Inter", "Poppins", "Plus Jakarta Sans"));

        var packageManager = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[orange1]Choose a package manager[/]")
                .AddChoices("npm", "pnpm", "bun"));

        var (fontFamily, fontUrl) = font switch
        {
            "Inter" => ("Inter", "https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&display=swap"),
            "Poppins" => ("Poppins", "https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap"),
            "Plus Jakarta Sans" => ("Plus Jakarta Sans", "https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:ital,wght@0,200..800;1,200..800&display=swap"),
            _ => throw new InvalidOperationException($"Unknown font: {font}")
        };

        return new WebFormResult(font, fontFamily, fontUrl, packageManager);
    }
}

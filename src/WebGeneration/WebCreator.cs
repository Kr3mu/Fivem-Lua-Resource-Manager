using LuaResourceManager.Forms;
using Spectre.Console;

namespace LuaResourceManager.WebGeneration;

public static class WebCreator
{
    public static void Create(WebFormResult result)
    {
        var resourcePath = Directory.GetCurrentDirectory();
        var webPath = Path.Combine(resourcePath, "web");
        var manifestPath = Path.Combine(resourcePath, "fxmanifest.lua");
        var resourceName = new DirectoryInfo(resourcePath).Name;

        if (Directory.Exists(webPath))
        {
            throw new IOException("A 'web' folder already exists in this resource.");
        }

        AnsiConsole.Status()
            .Start("Creating Vite Svelte app...", ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ProcessRunner.RunViteCreate(result.PackageManager, resourcePath);
                return 0;
            });

        AnsiConsole.Status()
            .Start("Installing Tailwind CSS...", ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ProcessRunner.RunTailwindInstall(result.PackageManager, webPath);
                return 0;
            });

        AnsiConsole.Status()
            .Start("Initializing Tailwind CSS...", ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ProcessRunner.RunTailwindInit(result.PackageManager, webPath);
                return 0;
            });

        File.WriteAllText(Path.Combine(webPath, "tailwind.config.js"), WebTemplate.TailwindConfig);
        File.WriteAllText(Path.Combine(webPath, "vite.config.ts"), WebTemplate.ViteConfig);

        DeleteDirectory(Path.Combine(webPath, "public"));

        CleanDirectory(Path.Combine(webPath, "src", "assets"));
        CleanDirectory(Path.Combine(webPath, "src", "lib"));

        DeleteFile(Path.Combine(webPath, "src", "app.css"));
        ClearFile(Path.Combine(webPath, "src", "App.svelte"));

        UpdateMainTsCssImport(webPath);

        File.WriteAllText(
            Path.Combine(webPath, "src", "App.css"),
            WebTemplate.AppCss
                .Replace("__FONT_URL__", result.FontUrl)
                .Replace("__FONT_FAMILY__", result.FontFamily));

        Directory.CreateDirectory(Path.Combine(webPath, "src", "stores"));
        Directory.CreateDirectory(Path.Combine(webPath, "src", "actions"));
        Directory.CreateDirectory(Path.Combine(webPath, "src", "components"));
        Directory.CreateDirectory(Path.Combine(webPath, "src", "pages"));

        var libPath = Path.Combine(webPath, "src", "lib");
        File.WriteAllText(Path.Combine(libPath, "index.ts"), WebTemplate.IndexTs);
        File.WriteAllText(Path.Combine(libPath, "types.ts"), WebTemplate.TypesTs);
        File.WriteAllText(Path.Combine(libPath, "fetchNui.ts"), WebTemplate.FetchNuiTs.Replace("__RESOURCE_NAME__", resourceName));
        File.WriteAllText(Path.Combine(libPath, "useNuiEvent.ts"), WebTemplate.UseNuiEventTs);
        File.WriteAllText(Path.Combine(libPath, "debugData.ts"), WebTemplate.DebugDataTs);

        if (File.Exists(manifestPath))
        {
            FxManifestUpdater.Update(manifestPath);
        }
    }

    private static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    private static void CleanDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        foreach (var file in Directory.GetFiles(path))
        {
            File.Delete(file);
        }
    }

    private static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static void ClearFile(string path)
    {
        if (File.Exists(path))
        {
            File.WriteAllText(path, "");
        }
    }

    private static void UpdateMainTsCssImport(string webPath)
    {
        var mainTsPath = Path.Combine(webPath, "src", "main.ts");

        if (!File.Exists(mainTsPath))
        {
            return;
        }

        var content = File.ReadAllText(mainTsPath);
        content = content.Replace("./app.css", "./App.css");
        File.WriteAllText(mainTsPath, content);
    }
}

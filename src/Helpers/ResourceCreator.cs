using System.Text;

namespace LuaResourceManager.Helpers;

public static class ResourceCreator
{
    public static void Create(ResourceFormResult resource)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), resource.Name);

        if (Directory.Exists(path))
        {
            throw new IOException($"A folder named '{resource.Name}' already exists.");
        }

        Directory.CreateDirectory(path);
        Directory.CreateDirectory(Path.Combine(path, "client"));
        Directory.CreateDirectory(Path.Combine(path, "server"));

        if (resource.CreateConfigs)
        {
            Directory.CreateDirectory(Path.Combine(path, "configs"));
        }

        File.WriteAllText(Path.Combine(path, "fxmanifest.lua"), GenerateManifest(resource));
        File.WriteAllText(Path.Combine(path, "client", "init.lua"), "");
        File.WriteAllText(Path.Combine(path, "server", "init.lua"), "");

        if (resource.CreateConfigs)
        {
            File.WriteAllText(Path.Combine(path, "configs", "shared.lua"), GenerateConfig("Shared"));
            File.WriteAllText(Path.Combine(path, "configs", "client.lua"), GenerateConfig("Client"));
            File.WriteAllText(Path.Combine(path, "configs", "server.lua"), GenerateConfig("Server"));
        }
    }

    private static string GenerateManifest(ResourceFormResult resource)
    {
        var builder = new StringBuilder();

        var quote = resource.UseDoubleQuotes ? '"' : '\'';

        builder.AppendLine($"fx_version {quote}cerulean{quote}");
        builder.AppendLine($"game {quote}gta5{quote}");
        builder.AppendLine();

        if (!string.IsNullOrWhiteSpace(resource.Description))
        {
            builder.AppendLine($"description {quote}{Escape(resource.Description, quote)}{quote}");
        }

        if (!string.IsNullOrWhiteSpace(resource.Author))
        {
            builder.AppendLine($"author {quote}{Escape(resource.Author, quote)}{quote}");
        }

        if (resource.UseFxv2Oal)
        {
            builder.AppendLine($"use_experimental_fxv2_oal {quote}yes{quote}");
        }

        builder.AppendLine();
        builder.AppendLine($"shared_script {quote}@ox_lib/init.lua{quote}");
        builder.AppendLine();
        builder.AppendLine($"client_script {quote}client/init.lua{quote}");
        builder.AppendLine($"server_script {quote}server/init.lua{quote}");

        var files = new List<string>();

        if (resource.CreateConfigs)
        {
            files.Add($"{quote}configs/shared.lua{quote}");
            files.Add($"{quote}configs/client.lua{quote}");
        }

        files.Add($"{quote}client/**{quote}");

        builder.AppendLine();
        builder.AppendLine("files {");
        builder.AppendLine("    " + string.Join(",\n    ", files));
        builder.AppendLine("}");

        return builder.ToString();
    }

    private static string GenerateConfig(string className)
    {
        return $"---@class {className}Config\n---@type {className}Config\nreturn {{}}\n";
    }

    private static string Escape(string value, char quote)
    {
        return value.Replace(quote.ToString(), "\\" + quote);
    }
}

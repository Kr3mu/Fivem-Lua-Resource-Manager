using System.Text;
using LuaResourceManager.Forms;

namespace LuaResourceManager.ResourceGeneration;

internal static class ManifestGenerator
{
    public static string Generate(ResourceFormResult resource)
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

    private static string Escape(string value, char quote)
    {
        return value.Replace(quote.ToString(), "\\" + quote);
    }
}

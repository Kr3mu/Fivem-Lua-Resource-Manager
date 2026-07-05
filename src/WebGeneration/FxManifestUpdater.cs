using System.Text;

namespace LuaResourceManager.WebGeneration;

internal static class FxManifestUpdater
{
    public static void Update(string manifestPath)
    {
        var manifest = File.ReadAllText(manifestPath);
        var quote = DetectQuoteStyle(manifest);

        manifest = EnsureWebFiles(manifest, quote);
        manifest = EnsureUiPage(manifest, quote);

        File.WriteAllText(manifestPath, manifest);
    }

    private static char DetectQuoteStyle(string manifest)
    {
        foreach (var c in manifest)
        {
            if (c is '"' or '\'')
            {
                return c;
            }
        }

        return '\'';
    }

    private static string EnsureWebFiles(string manifest, char quote)
    {
        var webFiles = new[] { "web/dist/index.html", "web/dist/**" };
        var entries = string.Join(",\n    ", webFiles.Select(file => $"{quote}{file}{quote}"));

        var filesStart = manifest.IndexOf("files {");

        if (filesStart == -1)
        {
            var builder = new StringBuilder(manifest);
            builder.AppendLine();
            builder.AppendLine("files {");
            builder.AppendLine("    " + entries);
            builder.AppendLine("}");
            return builder.ToString();
        }

        var filesEnd = manifest.IndexOf('}', filesStart);

        if (filesEnd == -1)
        {
            return manifest;
        }

        var content = manifest[filesStart..filesEnd];
        var separator = content.TrimEnd().EndsWith("{") ? "    " : ",\n    ";

        return manifest.Insert(filesEnd, separator + entries);
    }

    private static string EnsureUiPage(string manifest, char quote)
    {
        var devLine = $"ui_page {quote}http://localhost:3000{quote}";
        var distLine = $"-- ui_page {quote}web/dist/index.html{quote}";

        var index = manifest.IndexOf("ui_page");

        if (index == -1)
        {
            return devLine + "\n" + distLine + "\n" + manifest;
        }

        var endOfLine = manifest.IndexOf('\n', index);

        if (endOfLine == -1)
        {
            endOfLine = manifest.Length;
        }

        return manifest[..index] + devLine + "\n" + distLine + manifest[endOfLine..];
    }
}

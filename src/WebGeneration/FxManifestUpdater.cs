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

    internal static char DetectQuoteStyle(string manifest)
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

        var filesStart = manifest.IndexOf("files {");

        if (filesStart == -1)
        {
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine("files {");
            foreach (var file in webFiles)
            {
                builder.AppendLine($"    {quote}{file}{quote}");
            }
            builder.AppendLine("}");
            return AppendAtEnd(manifest, builder.ToString());
        }

        var filesEnd = manifest.IndexOf('}', filesStart);

        if (filesEnd == -1)
        {
            return manifest;
        }

        var content = manifest.Substring(filesStart + "files {".Length, filesEnd - filesStart - "files {".Length);
        var existingEntries = new List<string>();

        foreach (var rawLine in content.Split('\n'))
        {
            var line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line) || line == ",")
            {
                continue;
            }

            line = line.TrimEnd(',');

            if (line.Length >= 2 && ((line[0] == '"' && line[^1] == '"') || (line[0] == '\'' && line[^1] == '\'')))
            {
                line = line[1..^1];
            }

            if (!string.IsNullOrWhiteSpace(line) && !existingEntries.Contains(line))
            {
                existingEntries.Add(line);
            }
        }

        foreach (var webFile in webFiles)
        {
            if (!existingEntries.Contains(webFile))
            {
                existingEntries.Add(webFile);
            }
        }

        var filesBuilder = new StringBuilder();
        filesBuilder.AppendLine("files {");

        for (var i = 0; i < existingEntries.Count; i++)
        {
            var comma = i < existingEntries.Count - 1 ? "," : string.Empty;
            filesBuilder.AppendLine($"    {quote}{existingEntries[i]}{quote}{comma}");
        }

        filesBuilder.AppendLine("}");

        var before = manifest[..filesStart];
        var after = manifest[(filesEnd + 1)..];

        return before + filesBuilder + after;
    }

    private static string EnsureUiPage(string manifest, char quote)
    {
        var devLine = $"ui_page {quote}http://localhost:3000{quote}";
        var distLine = $"-- ui_page {quote}web/dist/index.html{quote}";

        var lines = manifest.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
        lines.RemoveAll(line =>
        {
            var trimmed = line.Trim();
            return trimmed.StartsWith("ui_page ", StringComparison.Ordinal) ||
                   trimmed.StartsWith("-- ui_page ", StringComparison.Ordinal);
        });

        var builder = new StringBuilder();
        foreach (var line in lines)
        {
            builder.AppendLine(line);
        }

        while (builder.Length > 0 && (builder[^1] == '\n' || builder[^1] == '\r'))
        {
            builder.Length--;
        }

        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(devLine);
        builder.AppendLine(distLine);

        return builder.ToString();
    }

    private static string AppendAtEnd(string manifest, string content)
    {
        if (!manifest.EndsWith(Environment.NewLine))
        {
            manifest += Environment.NewLine;
        }

        return manifest + content;
    }
}

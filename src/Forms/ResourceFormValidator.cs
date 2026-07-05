using LuaResourceManager.Helpers;

namespace LuaResourceManager.Forms;

internal static class ResourceFormValidator
{
    public static ResourceFormResult? TryBuildResult(IReadOnlyList<ResourceFormRow> rows)
    {
        var name = ((TextField)rows[0]).Value.Trim();
        var author = ((TextField)rows[1]).Value.Trim();
        var description = ((TextField)rows[2]).Value.Trim();
        var fxv2 = ((Toggle)rows[3]).Value;
        var configs = ((Toggle)rows[4]).Value;
        var quotes = ((Toggle)rows[5]).Value;

        if (string.IsNullOrWhiteSpace(name) || !ResourceValidation.ResourceNameRegex().IsMatch(name))
        {
            return null;
        }

        return new ResourceFormResult(name.ToLowerInvariant(), author, description, fxv2, configs, quotes);
    }
}

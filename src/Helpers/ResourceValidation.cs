using System.Text.RegularExpressions;

namespace LuaResourceManager.Helpers;

public static partial class ResourceValidation
{
    [GeneratedRegex("^[a-zA-Z0-9_-]+$")]
    public static partial Regex ResourceNameRegex();
}

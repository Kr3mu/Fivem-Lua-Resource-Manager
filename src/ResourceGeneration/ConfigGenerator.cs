namespace LuaResourceManager.ResourceGeneration;

internal static class ConfigGenerator
{
    public static string Generate(string className)
    {
        return $"---@class {className}Config\n---@type {className}Config\nreturn {{}}\n";
    }
}

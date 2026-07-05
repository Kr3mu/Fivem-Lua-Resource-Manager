namespace LuaResourceManager.Forms;

public record ResourceFormResult(
    string Name,
    string Author,
    string Description,
    bool UseFxv2Oal,
    bool CreateConfigs,
    bool UseDoubleQuotes);

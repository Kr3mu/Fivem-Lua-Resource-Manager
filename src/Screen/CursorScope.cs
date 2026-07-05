using Spectre.Console;

namespace LuaResourceManager.Screen;

public readonly struct CursorScope : IDisposable
{
    public CursorScope() => AnsiConsole.Cursor.Hide();

    public void Dispose() => AnsiConsole.Cursor.Show();
}

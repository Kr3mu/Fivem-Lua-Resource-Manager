using LuaResourceManager.Screen;

namespace LuaResourceManager.Forms;

public static class ResourceForm
{
    public static ResourceFormResult? Show()
    {
        using var _ = new CursorScope();

        var rows = new List<ResourceFormRow>
        {
            new TextField("Resource Name", ""),
            new TextField("Author", ""),
            new TextField("Description", ""),
            new Toggle("Use experimental fxv2_oal", true),
            new Toggle("Create configs folder", true),
            new Toggle("Use double quotes", false),
            new Button("Create"),
            new Button("Cancel")
        };

        var selected = 0;
        string? error = null;
        var lastWidth = -1;
        var lastHeight = -1;
        var needsRender = true;

        while (true)
        {
            var width = ScreenRenderer.GetConsoleWidth();
            var height = ScreenRenderer.GetConsoleHeight();

            if (width != lastWidth || height != lastHeight || needsRender)
            {
                lastWidth = width;
                lastHeight = height;
                needsRender = false;
                ResourceFormRenderer.Render(rows, selected, error);
            }

            if (Console.KeyAvailable)
            {
                error = null;
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    case ConsoleKey.K:
                        selected = (selected - 1 + rows.Count) % rows.Count;
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.J:
                        selected = (selected + 1) % rows.Count;
                        break;

                    case ConsoleKey.Escape:
                        return null;

                    case ConsoleKey.Enter:
                        var row = rows[selected];

                        switch (row)
                        {
                            case TextField field:
                                var newValue = ResourceFormInput.PromptForField(field.Label, field.Value, field.Label == "Resource Name");
                                field.Value = field.Label == "Resource Name" ? newValue.ToLowerInvariant() : newValue;
                                break;

                            case Toggle toggle:
                                toggle.Value = !toggle.Value;
                                break;

                            case Button { Label: "Create" }:
                                var result = ResourceFormValidator.TryBuildResult(rows);

                                if (result is null)
                                {
                                    error = "Resource name cannot be empty and may only contain letters, numbers, underscores, and hyphens.";
                                    break;
                                }

                                if (ResourceFormInput.ConfirmSummary(result))
                                {
                                    return result;
                                }

                                break;

                            case Button { Label: "Cancel" }:
                                return null;
                        }

                        break;
                }

                needsRender = true;
            }
            else
            {
                Thread.Sleep(50);
            }
        }
    }
}

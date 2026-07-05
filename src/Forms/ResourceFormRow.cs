namespace LuaResourceManager.Forms;

internal abstract class ResourceFormRow(string label)
{
    public string Label { get; } = label;
    public abstract string DisplayValue { get; }
}

internal sealed class TextField(string label, string value) : ResourceFormRow(label)
{
    public string Value { get; set; } = value;
    public override string DisplayValue => string.IsNullOrEmpty(Value) ? "—" : Value;
}

internal sealed class Toggle(string label, bool value) : ResourceFormRow(label)
{
    public bool Value { get; set; } = value;
    public override string DisplayValue => Value ? "[[x]] yes" : "[[ ]] no";
}

internal sealed class Button(string label) : ResourceFormRow(label)
{
    public override string DisplayValue => "";
}

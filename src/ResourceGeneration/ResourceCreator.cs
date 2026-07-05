using LuaResourceManager.Forms;

namespace LuaResourceManager.ResourceGeneration;

public static class ResourceCreator
{
    public static void Create(ResourceFormResult resource)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), resource.Name);

        if (Directory.Exists(path))
        {
            throw new IOException($"A folder named '{resource.Name}' already exists.");
        }

        Directory.CreateDirectory(path);
        Directory.CreateDirectory(Path.Combine(path, "client"));
        Directory.CreateDirectory(Path.Combine(path, "server"));

        if (resource.CreateConfigs)
        {
            Directory.CreateDirectory(Path.Combine(path, "configs"));
        }

        File.WriteAllText(Path.Combine(path, "fxmanifest.lua"), ManifestGenerator.Generate(resource));
        File.WriteAllText(Path.Combine(path, "client", "init.lua"), "");
        File.WriteAllText(Path.Combine(path, "server", "init.lua"), "");

        if (resource.CreateConfigs)
        {
            File.WriteAllText(Path.Combine(path, "configs", "shared.lua"), ConfigGenerator.Generate("Shared"));
            File.WriteAllText(Path.Combine(path, "configs", "client.lua"), ConfigGenerator.Generate("Client"));
            File.WriteAllText(Path.Combine(path, "configs", "server.lua"), ConfigGenerator.Generate("Server"));
        }
    }
}

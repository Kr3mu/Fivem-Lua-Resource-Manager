using System.Diagnostics;
using System.Text;

namespace LuaResourceManager.WebGeneration;

internal static class ProcessRunner
{
    public static int Run(string workingDirectory, string executable, string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {executable} {arguments}",
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        using var process = Process.Start(psi);

        if (process is null)
        {
            throw new InvalidOperationException($"Failed to start process: {executable} {arguments}");
        }

        var output = new StringBuilder();
        var error = new StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                output.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                error.AppendLine(e.Data);
            }
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Command failed with exit code {process.ExitCode}: {executable} {arguments}\n{error}{output}");
        }

        return process.ExitCode;
    }

    public static int RunViteCreate(string packageManager, string workingDirectory)
    {
        var args = packageManager switch
        {
            "npm" => "create vite@latest web -- --template svelte-ts",
            "pnpm" => "create vite web --template svelte-ts",
            "bun" => "create vite web --template svelte-ts",
            _ => throw new InvalidOperationException($"Unknown package manager: {packageManager}")
        };

        return Run(workingDirectory, packageManager, args);
    }

    public static int RunTailwindInstall(string packageManager, string workingDirectory)
    {
        var args = packageManager switch
        {
            "npm" => "install -D tailwindcss@3 postcss autoprefixer",
            "pnpm" => "add -D tailwindcss@3 postcss autoprefixer",
            "bun" => "add -D tailwindcss@3 postcss autoprefixer",
            _ => throw new InvalidOperationException($"Unknown package manager: {packageManager}")
        };

        return Run(workingDirectory, packageManager, args);
    }

    public static int RunTailwindInit(string packageManager, string workingDirectory)
    {
        var executable = packageManager switch
        {
            "npm" => "npx",
            "pnpm" => "pnpm",
            "bun" => "bunx",
            _ => throw new InvalidOperationException($"Unknown package manager: {packageManager}")
        };

        return Run(workingDirectory, executable, "tailwindcss init -p");
    }
}

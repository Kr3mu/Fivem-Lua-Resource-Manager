using Spectre.Console;
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
            RedirectStandardInput = true,
            CreateNoWindow = true,
        };

        using var process = Process.Start(psi);

        if (process is null)
        {
            throw new InvalidOperationException($"Failed to start process: {executable} {arguments}");
        }

        process.StandardInput.Close();
        var output = new StringBuilder();
        var error = new StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                AnsiConsole.WriteLine(e.Data);
                output.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                AnsiConsole.WriteLine(e.Data);
                error.AppendLine(e.Data);
            }
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        var timeout = TimeSpan.FromMinutes(5);
        if (!process.WaitForExit((int)timeout.TotalMilliseconds))
        {
            process.Kill(entireProcessTree: true);
            throw new InvalidOperationException(
                $"Command timed out after {timeout.TotalMinutes} minutes: {executable} {arguments}\n{error}{output}");
        }

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Command failed with exit code {process.ExitCode}: {executable} {arguments}\n{error}{output}");
        }

        return process.ExitCode;
    }

    public static int RunViteCreate(string packageManager, string workingDirectory)
    {
        var (executable, args) = packageManager switch
        {
            "npm" => ("npx", "--yes create-vite@latest web --template svelte-ts --no-install"),
            "pnpm" => ("pnpm", "dlx create-vite@latest web --template svelte-ts --no-install"),
            "bun" => ("bun", "x create-vite@latest web --template svelte-ts --no-install"),
            _ => throw new InvalidOperationException($"Unknown package manager: {packageManager}")
        };

        return Run(workingDirectory, executable, args);
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

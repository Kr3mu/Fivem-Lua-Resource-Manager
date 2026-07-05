# Lua Resource Manager (LRM)

> **Disclaimer:** This tool was generated with AI assistance. It is provided as-is and may contain bugs or unexpected behavior. Use it at your own risk and review generated code before deploying it to production.

LRM is a terminal-based CLI tool for scaffolding and managing FiveM Lua resources. It helps automate the boring parts of resource setup so you can focus on writing actual game logic.

## Features

- **New Resource** — Create a new FiveM Lua resource with the standard folder structure and `fxmanifest.lua`.
- **Create Web** — Scaffold a Svelte + Tailwind CSS web app inside a resource, wired up for NUI.
- **Convert PX** — Convert pixel values in your web files to `vh` or `rem` based on a base resolution.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- For created resources: [ox_lib](https://github.com/overextended/ox_lib) and optionally [oxmysql](https://github.com/overextended/oxmysql)

## Development

Run the tool locally in development mode:

```bash
dotnet run
```

## Build

Build a single-file standalone executable for Windows:

```bash
dotnet publish -c Release -r win-x64 --self-contained true \
  /p:PublishSingleFile=true /p:EnableCompressionInSingleFile=true
```

The executable will be located at:

```
bin/Release/net10.0/win-x64/publish/LuaResourceManager.exe
```

## Usage

1. Navigate to the folder where you want to work (or into an existing resource).
2. Run `dotnet run`.
3. Use the arrow keys or `W`/`S` to navigate the menu and `Enter` to select an option.

## Notes

- Generated resources follow the single-entry-point pattern (`client/init.lua`, `server/init.lua`).
- Web resources are built with Vite + Svelte + Tailwind CSS and output to `web/dist`.
- Run `npm run build` inside the `web` folder when you are ready to build the production NUI assets.

## License

This project is open source. See the repository for license details.

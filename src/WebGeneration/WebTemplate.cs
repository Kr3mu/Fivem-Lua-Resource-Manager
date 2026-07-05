namespace LuaResourceManager.WebGeneration;

internal static class WebTemplate
{
    public const string TailwindConfig = """"
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{svelte,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
"""";

    public const string ViteConfig = """"
import { defineConfig } from "vite";
import { svelte } from "@sveltejs/vite-plugin-svelte";
import { fileURLToPath, URL } from "node:url";

// https://vite.dev/config/
export default defineConfig({
    base: "./",
    plugins: [svelte()],
    resolve: {
        alias: {
            $assets: fileURLToPath(new URL("./src/assets", import.meta.url)),
            $components: fileURLToPath(new URL("./src/components", import.meta.url)),
            $lib: fileURLToPath(new URL("./src/lib", import.meta.url)),
            $stores: fileURLToPath(new URL("./src/stores", import.meta.url)),
            $pages: fileURLToPath(new URL("./src/pages", import.meta.url)),
        },
    },
    build: {
        outDir: "dist",
    },
    server: {
        port: 3000,
    },
});
"""";

    public const string AppCss = """"
@import url("__FONT_URL__");

@tailwind base;
@tailwind components;
@tailwind utilities;

:root {
    font-family: "__FONT_FAMILY__", sans-serif !important;
    font-synthesis: none !important;
    text-rendering: optimizeLegibility !important;
    -webkit-font-smoothing: antialiased !important;
    -moz-osx-font-smoothing: grayscale !important;
    color-scheme: normal !important;
    background-color: transparent !important;
    overflow: hidden !important;
    height: 100vh !important;
    width: 100% !important;
    margin: 0 !important;
    padding: 0 !important;
    font-size: 1.2vh;
}

* {
    box-sizing: border-box;
}

*::-webkit-scrollbar {
    display: none;
}

* {
    scrollbar-width: none;
    outline: none !important;
    user-select: none;
}

body {
    margin: 0;
    padding: 0;
    background-color: transparent;
    overflow: hidden;
}
"""";

    public const string IndexTs = """"
export function IsEnvBrowser(): boolean {
    return !(window as any).invokeNative;
}
"""";

    public const string TypesTs = """"
export interface NuiMessage<T = unknown> {
    action: string;
    data: T;
}

export interface NuiDebugEvent<T = unknown> {
    data: T;
    delay?: number;
}

export type NuiEventHandler<T = any> = (data: T) => void;
"""";

    public const string FetchNuiTs = """"
import { IsEnvBrowser } from "./index";

/**
 * @param eventName - The endpoint eventname to target
 * @param data - Data you wish to send in the NUI Callback
 * @param browserData - Data you wish to return when in browser
 *
 * @return returnData - A promise for the data sent back by the NuiCallbacks CB argument
 */
export async function fetchNui<T = any>(
    eventName: string,
    data: unknown = {},
    browserData?: T,
): Promise<T> {
    if (IsEnvBrowser())
        return await new Response(JSON.stringify(browserData)).json();

    const options = {
        method: "post",
        headers: {
            "Content-Type": "application/json; charset=UTF-8",
        },
        body: JSON.stringify(data),
    };

    const resourceName = (window as any).GetParentResourceName
        ? (window as any).GetParentResourceName()
        : "__RESOURCE_NAME__";

    const resp = await fetch(`https://${resourceName}/${eventName}`, options);
    return await resp.json();
}
"""";

    public const string UseNuiEventTs = """"
import { onDestroy } from "svelte";
import type { NuiDebugEvent, NuiEventHandler, NuiMessage } from "./types";
import { IsEnvBrowser } from "./index";

const eventListeners = new Map<string, NuiEventHandler[]>();

const eventListener = (event: MessageEvent<NuiMessage>) => {
    const { action, data } = event.data;
    const handlers = eventListeners.get(action);

    if (handlers) {
        handlers.forEach((handler) => handler(data));
    }
};

window.addEventListener("message", eventListener);

/**
 * A function that manage events listeners for receiving data from the client scripts
 * @param action The specific `action` that should be listened for.
 * @param handler The callback function that will handle data relayed by this function
 * @param debug An optional object or array of objects that will be used to simulate the event in the browser
 *
 * @example
 * useNuiEvent<{visibility: true, wasVisible: 'something'}>('setVisible', (data) => {
 *   // whatever logic you want
 * }, { data: { visibility: true, wasVisible: 'something' }, delay: 1000 });
 *
 **/
export function useNuiEvent<T = unknown>(
    action: string,
    handler: NuiEventHandler<T>,
    debug?: NuiDebugEvent<T> | NuiDebugEvent<T>[],
) {
    if (IsEnvBrowser() && debug) {
        const debugEvents = Array.isArray(debug) ? debug : [debug];
        debugEvents.forEach(({ data, delay }) => {
            if (delay) {
                setTimeout(() => handler(data), delay);
            } else {
                handler(data);
            }
        });
        return;
    }

    const handlers = eventListeners.get(action) || [];
    handlers.push(handler);
    eventListeners.set(action, handlers);

    onDestroy(() => {
        const handlers = eventListeners.get(action) || [];

        eventListeners.set(
            action,
            handlers.filter((h) => h !== handler),
        );
    });
}
"""";

    public const string DebugDataTs = """"
import { IsEnvBrowser } from "./index";
import type { NuiMessage } from "./types";

/**
 * Emulates dispatching an event using SendNuiMessage in the lua scripts.
 * This is used when developing in browser
 *
 * @param events - The event you want to cover
 * @param timer - How long until it should trigger (ms)
 */
export const debugData = <P>(events: NuiMessage<P>[], timer = 1000): void => {
    if (IsEnvBrowser()) {
        for (const event of events) {
            setTimeout(() => {
                window.dispatchEvent(
                    new MessageEvent("message", {
                        data: {
                            action: event.action,
                            data: event.data,
                        },
                    }),
                );
            }, timer);
        }
    }
};
"""";
}

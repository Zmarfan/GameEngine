﻿using SDL2;

namespace GameEngine.engine.core.cursor; 

public class Cursor {
    public static bool IsActive { get; private set; }
    private static Cursor _self = null!;
    private readonly nint _window;
    private readonly nint _cursor;
    
    private unsafe Cursor(CursorSettings settings, nint window) {
        _window = window;
        if (settings.customCursorSettings == null) {
            return;
        }
        
        SDL.SDL_Surface* surface = GetSurface(settings.customCursorSettings);
        int x = Math.Min((int)(surface->w * settings.customCursorSettings.xHotSpot), surface->w - 1);
        int y = Math.Min((int)(surface->h * settings.customCursorSettings.yHotSpot), surface->h - 1);
        _cursor = SDL.SDL_CreateColorCursor((nint)surface, x, y);
        if (_cursor == nint.Zero) {
            throw new Exception($"Unable to set cursor to provided image: {settings.customCursorSettings.imageSource} due to: {SDL.SDL_GetError()}");
        }
        SDL.SDL_SetCursor(_cursor);
        SDL.SDL_FreeSurface((nint)surface);
    }

    internal static void Init(CursorSettings settings, nint window) {
        if (_self != null) {
            throw new Exception("You can not init more than one cursor!");
        }

        _self = new Cursor(settings, window);
        SetActive(settings.enabled);
        ConfineMouseToWindow(settings.confine);
    }
    
    internal static void Clean() {
        SDL.SDL_FreeCursor(_self._cursor);
        _self = null!;
    }

    public static void SetActive(bool active) {
        if (SDL.SDL_ShowCursor(active ? SDL.SDL_ENABLE : SDL.SDL_DISABLE) < 0) {
            throw new Exception($"Unable to set cursor active state due to: {SDL.SDL_GetError()}");
        }

        if (SDL.SDL_SetRelativeMouseMode(active ? SDL.SDL_bool.SDL_FALSE : SDL.SDL_bool.SDL_TRUE) != 0) {
            throw new Exception($"Unable to set relative mouse mode due to: {SDL.SDL_GetError()}");
        }
        IsActive = active;
    }

    public static void ConfineMouseToWindow(bool confine) {
        SDL.SDL_SetWindowMouseGrab(_self._window, confine ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
    }
    
    private static unsafe SDL.SDL_Surface* GetSurface(CustomCursorSettings settings) {
        SDL.SDL_Surface* surface = (SDL.SDL_Surface*)SDL_image.IMG_Load(settings.imageSource);
        if (surface->h % 8 != 0 || surface->w % 8 != 0) {
            throw new Exception($"The dimensions of the cursor image has to be divisible by 8! Provided: {surface->w}x{surface->h}");
        }
        return surface;
    }
}
﻿using SDL2;
using GameEngine.engine.data;
using Color = GameEngine.engine.data.Color;

namespace GameEngine.engine.core.renderer.textures; 

internal class StoredTexture {
    public readonly unsafe SDL.SDL_Surface* surface;
    public readonly nint texture;
    public readonly Color[,] pixels;
    public readonly bool fromFile;

    public unsafe StoredTexture(SDL.SDL_Surface* surface, nint texture, Color[,] pixels, bool fromFile) {
        this.surface = surface;
        this.texture = texture;
        this.pixels = pixels;
        this.fromFile = fromFile;
    }
}
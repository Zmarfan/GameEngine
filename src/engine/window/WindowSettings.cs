using GameEngine.engine.data;
using GameEngine.engine.window.menu;

namespace GameEngine.engine.window; 

public class WindowSettings {
    internal static readonly Vector2Int START_NATIVE_RESOLUTION = new(-1, -1); 
    
    internal Vector2Int resolution;
    public readonly bool allowFullscreen;
    public readonly bool startFullscreen;
    public readonly WindowMenu? windowMenu;
    public readonly string? iconSrc;

    internal WindowSettings(Vector2Int resolution, bool allowFullscreen, bool startFullscreen, WindowMenu? windowMenu, string? iconSrc) {
        this.resolution = resolution;
        this.allowFullscreen = allowFullscreen;
        this.startFullscreen = startFullscreen && allowFullscreen && windowMenu != null;
        this.windowMenu = windowMenu;
        this.iconSrc = iconSrc;
    }
}
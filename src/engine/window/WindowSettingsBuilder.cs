using GameEngine.engine.data;
using GameEngine.engine.window.menu;

namespace GameEngine.engine.window; 

public class WindowSettingsBuilder {
    private Vector2Int _resolution = WindowSettings.START_NATIVE_RESOLUTION;
    private bool _allowFullscreen = true;
    private bool _startFullscreen = true;
    private WindowMenu? _windowMenu;
    private string? _iconSrc;
    
    public static WindowSettingsBuilder Builder() {
        return new WindowSettingsBuilder();
    }

    public WindowSettings Build() {
        return new WindowSettings(_resolution, _allowFullscreen, _startFullscreen, _windowMenu, _iconSrc);
    }

    public WindowSettingsBuilder SetResolution(Vector2Int size) {
        _resolution = size;
        return this;
    }

    public WindowSettingsBuilder SetAllowFullscreen(bool allow) {
        _allowFullscreen = allow;
        return this;
    }

    public WindowSettingsBuilder SetStartFullscreen(bool startFullscreen) {
        _startFullscreen = startFullscreen;
        return this;
    }

    public WindowSettingsBuilder SetMenu(WindowMenu menu) {
        _windowMenu = menu;
        return this;
    }

    public WindowSettingsBuilder SetIconSrc(string src) {
        _iconSrc = src;
        return this;
    }
}
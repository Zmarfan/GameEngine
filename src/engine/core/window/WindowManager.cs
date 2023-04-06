using SDL2;
using GameEngine.engine.data;
using GameEngine.engine.window;

namespace GameEngine.engine.core.window; 

public class WindowManager {
    public delegate void ResolutionChangedDelegate(Vector2Int resolution);
    public static event ResolutionChangedDelegate? ResolutionChangedEvent;
    
    private static WindowManager _self = null!;

    private readonly nint _window;
    private readonly HashSet<Vector2Int> _supportedResolutions;
    private readonly WindowSettings _settings;
    private bool _isFullscreen;

    private WindowManager(nint window, WindowSettings settings) {
        _window = window;
        _supportedResolutions = CalculateSupportedResolutions(window);
        _settings = settings;
    }

    internal static void Init(nint window, WindowSettings settings) {
        if (_self != null) {
            throw new Exception("There can only be one Window Manager!");
        }

        _self = new WindowManager(window, settings);
        
        if (!settings.allowFullscreen) {
            return;
        }
        
        SetResolution(settings.resolution == WindowSettings.START_NATIVE_RESOLUTION
            ? CalculateNativeResolution(window)
            : settings.resolution);
        if (settings.startFullscreen) {
            ToggleFullScreen();
        }
    }

    public static void ToggleFullScreen() {
        if (!_self._settings.allowFullscreen) {
            return;
        }
        uint flag = _self._isFullscreen ? 0 : (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
        if (SDL.SDL_SetWindowFullscreen(_self._window, flag) != 0) {
            throw new Exception($"Unable to change window fullScreen mode due to: {SDL.SDL_GetError()}");
        }
        _self._isFullscreen = !_self._isFullscreen;
    }

    public static void SetResolution(Vector2Int resolution) {
        if (resolution.x <= 0 || resolution.y <= 0) {
            throw new ArgumentException("Resolution must be > 0 per x and y axis");
        }
        if (_self._settings.allowFullscreen && !_self._supportedResolutions.Contains(resolution)) {
            resolution = TurnResolutionToClosestSupportedResolution(resolution);
        }
        SDL.SDL_SetWindowSize(_self._window, resolution.x, resolution.y);
        SDL.SDL_SetWindowPosition(_self._window, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED);
        _self._settings.resolution = resolution;
        ResolutionChangedEvent?.Invoke(resolution);
    }

    public static Vector2Int CurrentResolution => _self._settings.resolution;
    
    private static HashSet<Vector2Int> CalculateSupportedResolutions(nint window) {
        int displayIndex = SDL.SDL_GetWindowDisplayIndex(window);
        int displayModeCount = SDL.SDL_GetNumDisplayModes(displayIndex);
        HashSet<Vector2Int> supportedResolutions = new();
        for (int i = 0; i < displayModeCount; i++) {
            SDL.SDL_GetDisplayMode(displayIndex, i, out SDL.SDL_DisplayMode mode);
            supportedResolutions.Add(new Vector2Int(mode.w, mode.h));
        }

        return supportedResolutions;
    }
    
    private static Vector2Int CalculateNativeResolution(nint window) {
        SDL.SDL_GetDesktopDisplayMode(SDL.SDL_GetWindowDisplayIndex(window), out SDL.SDL_DisplayMode mode);
        return new Vector2Int(mode.w, mode.h);
    }
    
    private static Vector2Int TurnResolutionToClosestSupportedResolution(Vector2Int resolution) {
        int closestWidth = _self._supportedResolutions.MinBy(r => Math.Abs(r.x - resolution.x)).x;
        return _self._supportedResolutions
            .Where(r => r.x == closestWidth)
            .MinBy(r => Math.Abs(r.y - resolution.y));
    }
}
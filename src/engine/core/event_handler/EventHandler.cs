using SDL2;
using GameEngine.engine.core.input.listener;
using GameEngine.engine.core.window;
using GameEngine.engine.data;
using GameEngine.engine.logger;
using GameEngine.engine.window;
using Button = GameEngine.engine.core.input.listener.Button;

namespace GameEngine.engine.core.event_handler; 

internal class EventHandler {
    public event EventVoidDelegate? QuitEvent;
    public event ButtonEventDelegate? KeyDownEvent;
    public event ButtonEventDelegate? KeyUpEvent;
    public event MouseMovementEventDelegate? MouseMovementEvent;

    private readonly WindowSettings _settings;
    
    public EventHandler(WindowSettings settings) {
        _settings = settings;
    }
    
    public void HandleEvents() {
        try {
            HandleNewEvents();
        }
        catch (Exception e) {
            Logger.Error(e, "There was an issue with the event handling this frame");
        }
    }

    private void HandleNewEvents() {
        while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
        {
            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    QuitEvent?.Invoke();
                    break;
                case SDL.SDL_EventType.SDL_WINDOWEVENT: {
                    if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED) {
                        _settings.resolution = new Vector2Int(e.window.data1, e.window.data2);
                    }
                    break;
                }
                case SDL.SDL_EventType.SDL_KEYDOWN: {
                    if (IsEnterFullScreen(e.key.keysym)) {
                        WindowManager.ToggleFullScreen();
                    }

                    if (SdlInputCodeToButton.SCANCODE_TO_BUTTON.TryGetValue(e.key.keysym.scancode, out Button value)) {
                        KeyDownEvent?.Invoke(value);
                    }
                    break;
                }
                case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN: {
                    if (SdlInputCodeToButton.MOUSE_BUTTON_TO_BUTTON.TryGetValue(e.button.button, out Button value)) {
                        KeyDownEvent?.Invoke(value);
                    }
                    break;
                }
                case SDL.SDL_EventType.SDL_KEYUP: {
                    if (SdlInputCodeToButton.SCANCODE_TO_BUTTON.TryGetValue(e.key.keysym.scancode, out Button value)) {
                        KeyUpEvent?.Invoke(value);
                    }
                    break;
                }
                case SDL.SDL_EventType.SDL_MOUSEBUTTONUP: {
                    if (SdlInputCodeToButton.MOUSE_BUTTON_TO_BUTTON.TryGetValue(e.button.button, out Button value)) {
                        KeyUpEvent?.Invoke(value);
                    }
                    break;
                }
                case SDL.SDL_EventType.SDL_MOUSEMOTION: {
                    float relativeXPosition = e.motion.x / (float)_settings.resolution.x;
                    float relativeYPosition = (_settings.resolution.y - e.motion.y) / (float)_settings.resolution.y;
                    MouseMovementEvent?.Invoke(new Vector2(relativeXPosition, relativeYPosition), new Vector2(e.motion.xrel, -e.motion.yrel));
                    break;
                }
                default:
                    return;
            }
        }
    }

    private static bool IsEnterFullScreen(SDL.SDL_Keysym key) {
        return key is { scancode: SDL.SDL_Scancode.SDL_SCANCODE_RETURN, mod: SDL.SDL_Keymod.KMOD_LALT };
    }
}
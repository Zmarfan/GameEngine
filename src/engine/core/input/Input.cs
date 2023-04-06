using GameEngine.engine.camera;
using GameEngine.engine.core.input.listener;
using GameEngine.engine.data;
using GameEngine.engine.helper;
using GameEngine.engine.window;
using Button = GameEngine.engine.core.input.listener.Button;
using EventHandler = GameEngine.engine.core.event_handler.EventHandler;

namespace GameEngine.engine.core.input; 

public class Input {
    private static Input _self = null!;

    public static Vector2 MouseWorldPosition =>
        Camera.Main.ScreenToWorldMatrix.ConvertPoint(new Vector2(
            _mouseScreenPosition.x * _windowSettings.resolution.x,
            (1 - _mouseScreenPosition.y) * _windowSettings.resolution.y)
        );

    public static Vector2 MouseCameraPosition =>
        Camera.Main.ScreenToUiMatrix.ConvertPoint(
            new Vector2(_mouseScreenPosition.x * _windowSettings.resolution.x, _windowSettings.resolution.y - _mouseScreenPosition.y * _windowSettings.resolution.y)
        );

    public static Vector2 MouseDirection { get; private set; }
    private static Vector2 _mouseScreenPosition = new(0, 1);
    private static WindowSettings _windowSettings = null!;
    
    private readonly Dictionary<string, InputListener> _listenersByName;
    private readonly Dictionary<Button, List<InputListener>> _listenersByButton = new();

    private Input(WindowSettings settings, EventHandler eventHandler, List<InputListener> listeners) {
        _windowSettings = settings;
        
        _listenersByName = listeners.ToDictionary(l => l.name, l => l);
        listeners.ForEach(listener => {
            AddListenerByButton(listener.positiveButton, listener);
            if (listener.negativeButton != null) {
                AddListenerByButton((Button)listener.negativeButton, listener);
            }
            if (listener.altPositiveButton != null) {
                AddListenerByButton((Button)listener.altPositiveButton, listener);
            }
            if (listener.altNegativeButton != null) {
                AddListenerByButton((Button)listener.altNegativeButton, listener);
            }
        });
        foreach (Button button in Enum.GetValues(typeof(Button))) {
            string name = GetButtonName(button);
            _listenersByName.Add(name, InputListenerBuilder.Builder(name, button).Build());
        }

        eventHandler.KeyDownEvent += ButtonDownEventListener;
        eventHandler.KeyUpEvent += ButtonUpEventListener;
        eventHandler.MouseMovementEvent += (position, direction) => {
            _mouseScreenPosition = position;
            MouseDirection += direction;
        };
    }

    internal static Input Init(WindowSettings settings, EventHandler eventHandler, List<InputListener> listeners) {
        if (_self != null) {
            throw new Exception("There can only be one input manager!");
        }

        _self = new Input(settings, eventHandler, listeners);
        return _self;
    }

    internal void Update(float deltaTime) {
        foreach ((string _, InputListener listener) in _self._listenersByName) {
            listener.UpdateAxis(deltaTime);
        }
    }

    internal void FrameReset() {
        MouseDirection = Vector2.Zero();
        foreach ((string _, InputListener listener) in _self._listenersByName) {
            listener.FrameReset();
        }
    }

    public static bool GetKeyDown(Button key) {
        return _self._listenersByName[GetButtonName(key)].GetButtonDown();
    }

    public static bool GetKeyUp(Button key) {
        return _self._listenersByName[GetButtonName(key)].GetButtonUp();
    }
    
    public static bool GetKey(Button key) {
        return _self._listenersByName[GetButtonName(key)].GetButton();
    }
    
    public static bool GetButtonDown(string listenerName) {
        return _self._listenersByName[listenerName].GetButtonDown();
    }

    public static bool GetButtonUp(string listenerName) {
        return _self._listenersByName[listenerName].GetButtonUp();
    }
    
    public static bool GetButton(string listenerName) {
        return _self._listenersByName[listenerName].GetButton();
    }
    
    public static Vector2 GetAxis(string listenerName) {
        return _self._listenersByName[listenerName].GetAxis();
    }

    private void ButtonDownEventListener(Button button) {
        _listenersByName[GetButtonName(button)].SetButtonDown(button);
        if (_listenersByButton.TryGetValue(button, out List<InputListener>? listeners)) {
            foreach (InputListener listener in listeners) {
                listener.SetButtonDown(button);
            }
        }
    }
    
    private void ButtonUpEventListener(Button button) {
        _listenersByName[GetButtonName(button)].SetButtonUp(button);
        if (_listenersByButton.TryGetValue(button, out List<InputListener>? listeners)) {
            foreach (InputListener listener in listeners) {
                listener.SetButtonUp(button);
            }
        }
    }
    
    private void AddListenerByButton(Button button, InputListener listener) {
        if (!_listenersByButton.ContainsKey(button)) {
            _listenersByButton.Add(button, ListUtils.Empty<InputListener>());
        }
        _listenersByButton[button].Add(listener);
    }

    private static string GetButtonName(Button button) {
        return $"ENGINE_INTERNAL_KEY_{button}";
    }
}
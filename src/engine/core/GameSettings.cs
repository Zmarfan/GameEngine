using GameEngine.engine.core.assets;
using GameEngine.engine.core.audio;
using GameEngine.engine.core.cursor;
using GameEngine.engine.core.gizmos;
using GameEngine.engine.core.input.listener;
using GameEngine.engine.core.update.physics.settings;
using GameEngine.engine.scene;
using GameEngine.engine.window;
using GameEngine.engine.window.menu;

namespace GameEngine.engine.core; 

public record GameSettings(
    bool debug,
    string title,
    Assets assets,
    WindowSettings windowSettings,
    List<Scene> scenes, 
    List<InputListener> inputListeners,
    PhysicsSettings physicsSettings,
    List<string> sortLayers,
    AudioSettings audioSettings,
    CursorSettings cursorSettings,
    GizmoSettings gizmoSettings
) {
    public readonly bool debug = debug;
    public readonly string title = title;
    public readonly Assets assets = assets;
    public readonly WindowSettings windowSettings = windowSettings;
    public readonly List<Scene> scenes = scenes;
    public readonly List<InputListener> inputListeners = inputListeners;
    public readonly PhysicsSettings physicsSettings = physicsSettings;
    public readonly List<string> sortLayers = sortLayers;
    public readonly AudioSettings audioSettings = audioSettings;
    public readonly CursorSettings cursorSettings = cursorSettings;
    public readonly GizmoSettings gizmoSettings = gizmoSettings;
}
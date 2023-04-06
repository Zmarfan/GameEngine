using GameEngine.engine.core.assets;
using GameEngine.engine.core.audio;
using GameEngine.engine.core.cursor;
using GameEngine.engine.core.gizmos;
using GameEngine.engine.core.input.listener;
using GameEngine.engine.core.renderer.textures;
using GameEngine.engine.core.update.physics.layers;
using GameEngine.engine.core.update.physics.settings;
using GameEngine.engine.helper;
using GameEngine.engine.scene;
using GameEngine.engine.window;
using GameEngine.engine.window.menu;

namespace GameEngine.engine.core; 

public class GameSettingsBuilder {
    private bool _debug = true;
    private string _title = "My Game";
    private Assets _assets = new(ListUtils.Empty<AssetDeclaration>(), ListUtils.Empty<AssetDeclaration>(), ListUtils.Empty<AssetDeclaration>());
    private WindowSettings _windowSettings = WindowSettingsBuilder.Builder().Build();
    private readonly List<Scene> _scenes = new();
    private readonly List<InputListener> _inputListeners = new();
    private PhysicsSettings _physicsSettings = PhysicsSettingsBuilder
        .Builder(
            ListUtils.Of(LayerMask.DEFAULT, LayerMask.IGNORE_RAYCAST),
            ListUtils.Of(LayerMask.DEFAULT, LayerMask.IGNORE_RAYCAST)
        )
        .Build();
    private readonly List<string> _sortLayers = new();
    private AudioSettings _audioSettings = new(Volume.Max(), ListUtils.Empty<AudioChannel>());
    private CursorSettings _cursorSettings = new(true, true);
    private GizmoSettings _gizmoSettings = GizmoSettingsBuilder.Builder().Build();

    public static GameSettingsBuilder Builder() {
        return new GameSettingsBuilder();
    }

    public GameSettings Build() {
        return new GameSettings(_debug, _title, _assets, _windowSettings, _scenes, _inputListeners, _physicsSettings, _sortLayers, _audioSettings, _cursorSettings, _gizmoSettings);
    }

    public GameSettingsBuilder SetAssets(Assets assets) {
        _assets = assets;
        return this;
    }
    
    public GameSettingsBuilder SetDebugMode() {
        _debug = true;
        return this;
    } 
    
    public GameSettingsBuilder SetBuildMode() {
        _debug = false;
        return this;
    } 
        
    public GameSettingsBuilder SetTitle(string title) {
        _title = title;
        return this;
    } 
    
    public GameSettingsBuilder SetWindowSettings(WindowSettings windowSettings) {
        _windowSettings = windowSettings;
        return this;
    }

    public GameSettingsBuilder AddScenes(IEnumerable<Scene> scenes) {
        _scenes.AddRange(scenes);
        return this;
    }
    
    public GameSettingsBuilder SetAudioSettings(AudioSettings audioSettings) {
        _audioSettings = audioSettings;
        return this;
    }
    
    public GameSettingsBuilder AddInputListeners(IEnumerable<InputListener> listeners) {
        _inputListeners.AddRange(listeners);
        return this;
    }
    
    public GameSettingsBuilder SetPhysics(PhysicsSettings settings) {
        _physicsSettings = settings;
        return this;
    }
    
    public GameSettingsBuilder AddSortLayers(IEnumerable<string> layers) {
        _sortLayers.AddRange(layers);
        return this;
    }

    public GameSettingsBuilder SetCursorSettings(CursorSettings settings) {
        _cursorSettings = settings;
        return this;
    }

    public GameSettingsBuilder SetGizmoSettings(GizmoSettings settings) {
        _gizmoSettings = settings;
        return this;
    }
}
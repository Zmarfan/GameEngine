using System.Diagnostics;
using SDL2;
using GameEngine.engine.camera;
using GameEngine.engine.core.audio;
using GameEngine.engine.core.cursor;
using GameEngine.engine.core.game_object_handler;
using GameEngine.engine.core.gizmos;
using GameEngine.engine.core.input;
using GameEngine.engine.core.renderer;
using GameEngine.engine.core.update;
using GameEngine.engine.core.update.physics.layers;
using GameEngine.engine.core.window;
using GameEngine.engine.logger;
using GameEngine.engine.scene;
using EventHandler = GameEngine.engine.core.event_handler.EventHandler;

namespace GameEngine.engine.core; 

public class Game {
    private const float MAX_FPS = 120;

    private bool _isRunning;
    private readonly EventHandler _eventHandler;
    private readonly UpdateHandler _updateHandler;
    private readonly GameRenderer _gameRenderer;
    private readonly AudioHandler _audioHandler;
    private readonly Stopwatch _actionFrameWatch = new();
    private readonly Stopwatch _totalFrameWatch = new();
    private float _deltaTime;

    private readonly SceneData _sceneData = new();
    private readonly GameSettings _settings;

    public Game(GameSettings settings) {
        _settings = settings;
        
        if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) != 0) {
            throw new Exception();
        }
        
        _eventHandler = new EventHandler(settings.windowSettings);
        Input inputHandler = Input.Init(settings.windowSettings, _eventHandler, settings.inputListeners);
        _updateHandler = new UpdateHandler(inputHandler, _sceneData);
        _gameRenderer = new GameRenderer(settings, _sceneData);
        _eventHandler.QuitEvent += () => _isRunning = false;
        _audioHandler = AudioHandler.Init(settings.audioSettings, settings.assets.audioDeclarations);
        LayerMask.Init(settings.physicsSettings.layersToCollisionLayers);
        Gizmos.Init(settings.gizmoSettings);
        SceneManager.Init(settings.scenes, LoadScene);
        
        _isRunning = true;
    }

    public void Run() {
        while (_isRunning) {
            _totalFrameWatch.Restart();
            
            _actionFrameWatch.Restart();
            RunFrame();
            _actionFrameWatch.Stop();
    
            SDL.SDL_Delay((uint)Math.Max(1000 / MAX_FPS - new TimeSpan(_actionFrameWatch.ElapsedTicks).TotalMilliseconds, 0));
            _totalFrameWatch.Stop();
            _deltaTime = (float)new TimeSpan(_totalFrameWatch.ElapsedTicks).TotalSeconds;
        }
        Clean();
    }

    private void RunFrame() {
        try {
            _eventHandler.HandleEvents();
            _updateHandler.Start();
            _updateHandler.UpdateLoops(_deltaTime);
            _gameRenderer.Render();
        }
        catch (Exception e) {
            Logger.Error(e, "An issue occured during this frame");
        }
    }

    private void LoadScene(Scene scene) {
        _sceneData.gameObjectHandler?.DestroyScene();
        Camera.CreateMainCamera(_settings.windowSettings);
        scene.CameraInitializer.Invoke(Camera.Main);
        _sceneData.gameObjectHandler = new GameObjectHandler(scene.CreateWorldGameObjectRoot(), scene.CreateSceneGameObjectRoot());
    }
    
    private void Clean() {
        _audioHandler.Clean();
        _gameRenderer.Clean();
        SDL.SDL_Quit();
    }
}
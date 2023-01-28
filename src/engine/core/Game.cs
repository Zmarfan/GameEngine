﻿using System.Diagnostics;
using SDL2;
using Worms.engine.core.audio;
using Worms.engine.core.cursor;
using Worms.engine.core.game_object_handler;
using Worms.engine.core.input;
using Worms.engine.core.renderer;
using Worms.engine.core.update;
using Worms.engine.logger;
using Worms.engine.scene;
using EventHandler = Worms.engine.core.event_handler.EventHandler;

namespace Worms.engine.core; 

public class Game {
    private const float MAX_FPS = 120;

    private bool _isRunning;
    private readonly EventHandler _eventHandler;
    private readonly UpdateHandler _updateHandler;
    private readonly Renderer _renderer;
    private readonly Stopwatch _actionFrameWatch = new();
    private readonly Stopwatch _totalFrameWatch = new();
    private float _deltaTime = 0;

    private readonly SceneData _sceneData = new();
    private readonly GameSettings _settings;

    public Game(GameSettings settings) {
        _settings = settings;
        
        if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) != 0) {
            throw new Exception();
        }
        
        SceneManager.Init(settings.scenes, LoadScene);
        _updateHandler = new UpdateHandler(_sceneData);
        
        _renderer = new Renderer(settings, _sceneData);
        _eventHandler = new EventHandler(settings);
        _eventHandler.QuitEvent += () => _isRunning = false;
        _eventHandler.ToggleFullscreenEvent += _renderer.ToggleFullScreen;
        AudioHandler.Init(settings.audioSettings);
        Input.Init(settings, _sceneData, _eventHandler, settings.inputListeners);
        Cursor.Init(settings.cursorSettings);
        
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
            _updateHandler.Awake();
            _updateHandler.Start();
            _updateHandler.UpdateLoops(_deltaTime);
            Input.FrameReset();
            _renderer.Render();
        }
        catch (Exception e) {
            Logger.Error(e, "An issue occured during this frame");
        }
    }

    private void LoadScene(Scene scene) {
        _sceneData.camera = scene.CreateCamera();
        _sceneData.camera.Init(_settings);
        _sceneData.camera.Awake();
        _sceneData.gameObjectHandler = new GameObjectHandler(scene.CreateWorldGameObjectRoot(), scene.CreateSceneGameObjectRoot());
    }
    
    private void Clean() {
        Cursor.Clean();
        AudioHandler.Clean();
        _renderer.Clean();
        SDL.SDL_Quit();
    }
}
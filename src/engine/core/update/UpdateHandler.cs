﻿using System.Diagnostics;
using GameEngine.engine.core.game_object_handler;
using GameEngine.engine.core.input;
using GameEngine.engine.core.update.physics;
using GameEngine.engine.core.update.physics.updating;
using GameEngine.engine.game_object;
using GameEngine.engine.game_object.scripts;
using GameEngine.engine.logger;
using GameEngine.engine.scene;

namespace GameEngine.engine.core.update; 

internal class UpdateHandler {
    private const float FIXED_UPDATE_CYCLE_TIME = 0.02f;

    private readonly PhysicsUpdateHandler _physicsUpdateHandler;
    private readonly Input _inputHandler;
    private readonly SceneData _sceneData;
    private GameObjectHandler GameObjectHandler => _sceneData.gameObjectHandler;
    private float _fixedUpdateAcc;
    private float _deltaTime;

    public UpdateHandler(Input inputHandler, SceneData sceneData) {
        _inputHandler = inputHandler;
        _sceneData = sceneData;
        _fixedUpdateAcc = 0;
        _physicsUpdateHandler = new PhysicsUpdateHandler(sceneData);
    }
    
    public void Awake() {
        foreach ((GameObject _, TrackObject obj) in GameObjectHandler.objects) {
            foreach (Script script in obj.Scripts) {
                try {
                    if (!script.HasRunAwake) {
                        script.Awake();
                    }
                }
                catch (Exception e) {
                    Logger.Error(e, $"An exception occured in {script} during the Awake callback");
                }
                script.HasRunAwake = true;
            }
        }
        GameObjectHandler.FrameCleanup();
    }
    
    public void Start() {
        foreach ((GameObject _, TrackObject obj) in GameObjectHandler.objects) {
            if (!obj.isActive) {
                return;
            }
            
            foreach (Script script in obj.Scripts) {
                try {
                    if (script is { IsActive: true, HasRunStart: false }) {
                        script.Start();
                    }
                }
                catch (Exception e) {
                    Logger.Error(e, $"An exception occured in {script} during the Start callback");
                }
                script.HasRunStart = true;
            }
        }
        GameObjectHandler.FrameCleanup();
    }
    
    public void UpdateLoops(float deltaTime) {
        UpdateFrameTimeData(deltaTime);
        _inputHandler.Update(_deltaTime);
        while (_fixedUpdateAcc > FIXED_UPDATE_CYCLE_TIME) {
            FixedUpdate();
            GameObjectHandler.FrameCleanup();
            _physicsUpdateHandler.Update();
            _fixedUpdateAcc -= FIXED_UPDATE_CYCLE_TIME;
        }
        Update();
        _inputHandler.FrameReset();
        GameObjectHandler.FrameCleanup();
    }

    private void FixedUpdate() {
        foreach ((GameObject _, TrackObject obj) in GameObjectHandler.objects) {
            if (!obj.isActive) {
                return;
            }
            
            foreach (Script script in obj.Scripts) {
                try {
                    if (script.IsActive) {
                        script.FixedUpdate(FIXED_UPDATE_CYCLE_TIME);
                    }
                }
                catch (Exception e) {
                    Logger.Error(e, $"An exception occured in {script} during the Fixed Update callback");
                }
            }
        }
    }
    
    private void Update() {
        foreach ((GameObject _, TrackObject obj) in GameObjectHandler.objects) {
            if (!obj.isActive) {
                return;
            }
            
            foreach (Script script in obj.Scripts) {
                try {
                    if (script.IsActive) {
                        script.Update(_deltaTime);
                    }
                }
                catch (Exception e) {
                    Logger.Error(e, $"An exception occured in {script} during the Update callback");
                }
            }
        }
    }
    
    private void UpdateFrameTimeData(float deltaTime) {
        _deltaTime = deltaTime;
        _fixedUpdateAcc += _deltaTime;
    }
}
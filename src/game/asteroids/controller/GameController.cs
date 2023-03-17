﻿using Worms.engine.camera;
using Worms.engine.core.window;
using Worms.engine.data;
using Worms.engine.game_object;
using Worms.engine.game_object.components.physics.colliders;
using Worms.engine.game_object.scripts;
using Worms.engine.helper;
using Worms.game.asteroids.asteroids;
using Worms.game.asteroids.names;
using Worms.game.asteroids.player;

namespace Worms.game.asteroids.controller; 

public class GameController : Script {
    private const float FAR_AWAY = 10000;
    private const float PLAY_AREA_BORDER = 100f;

    private Vector2 _playArea;
    private List<PolygonCollider> _colliders = null!;
    private Transform _player = null!;

    private bool _respawnPlayer = false;
    private ClockTimer _respawnTimer = new(3);
    private long _round = 0;
    
    public GameController() {
        PlayerBase.PlayerDieEvent += PlayerDied;
    }

    public override void Awake() {
        _colliders = GetComponents<PolygonCollider>();
        Camera.Main.Size = 2f;
        ResolutionChanged(WindowManager.CurrentResolution);
        WindowManager.ResolutionChangedEvent += ResolutionChanged;
    }

    public override void Start() {
        SpawnPlayer();
        SpawnAsteroidWave();
    }

    public override void Update(float deltaTime) {
        HandlePlayerRespawn(deltaTime);

        if (AllEnemiesCleared()) {
            SpawnAsteroidWave();
        }
    }

    private bool AllEnemiesCleared() {
        return false; // count children, also make it possible to have more than one collider as trigger/collider
    }

    private void HandlePlayerRespawn(float deltaTime) {
        _respawnTimer.Time += deltaTime;
        if (_respawnPlayer && _respawnTimer.Expired()) {
            _respawnPlayer = false;
            SpawnPlayer();
        }
    }
    
    private void SpawnAsteroidWave() {
        long spawnAmount = 3 + _round++;
        for (int i = 0; i < spawnAmount; i++) {
            AsteroidFactory.Create(Transform.GetRoot(), AsteroidType.BIG, GetRandomPositionAlongBorder());
        }
    }
    
    private Vector2 GetRandomPositionAlongBorder() {
        Vector2 position;
        float p = RandomUtil.GetRandomValueBetweenTwoValues(0, _playArea.x * 2 + _playArea.y * 2);
        if (p < _playArea.x + _playArea.y) {
            if (p < _playArea.x) {
                position.x = p;
                position.y = 0;
            }
            else {
                position.x = _playArea.x;
                position.y = p - _playArea.x;
            }
        }
        else {
            p -= _playArea.x + _playArea.y;
            if (p < _playArea.x) {
                position.x = _playArea.x - p;
                position.y = _playArea.y;
            }
            else {
                position.x = 0;
                position.y = _playArea.y - (p - _playArea.x);
            }
        }

        return position - new Vector2(_playArea.x, _playArea.y) / 2f;
    }

    
    public override void OnTriggerEnter(Collider collider) {
        Vector2 pos = collider.Transform.Parent!.Position;
        List<Vector2> corners = collider.GetLocalCorners()
            .Select(c => collider.Transform.LocalToWorldMatrix.ConvertPoint(c))
            .ToList();
        float maxY = Math.Abs(corners.MaxBy(c => Math.Abs(c.y)).y);
        float maxX = Math.Abs(corners.MaxBy(c => Math.Abs(c.x)).x);
        
        Vector2 half = _playArea / 2f;
        
        if (maxY > half.y) {
            pos = new Vector2(pos.x, -pos.y + Math.Sign(pos.y) * (maxY - half.y + 5));
        }

        if (collider.gameObject.Tag == TagNames.ENEMY) {
            collider.Transform.Parent!.gameObject.Destroy();
            return;
        }

        if (maxX > _playArea.x / 2f) {
            pos = new Vector2(-pos.x + Math.Sign(pos.x) * (maxX - half.x + 5), pos.y);
        }

        collider.Transform.Parent!.Position = pos;
    }

    private void ResolutionChanged(Vector2Int resolution) {
        _playArea = new Vector2(resolution.x + PLAY_AREA_BORDER, resolution.y + PLAY_AREA_BORDER) * Camera.Main.Size;
        float minX = -_playArea.x / 2;
        float maxX = _playArea.x / 2;
        float minY = -_playArea.y / 2;
        float maxY = _playArea.y / 2;
        _colliders[0].Vertices = new Vector2[] { new(-FAR_AWAY, minY), new(-FAR_AWAY, maxY), new(minX, maxY), new(minX, minY) };
        _colliders[1].Vertices = new Vector2[] { new(-FAR_AWAY, maxY), new(-FAR_AWAY, FAR_AWAY), new(FAR_AWAY, FAR_AWAY), new(FAR_AWAY, maxY) };
        _colliders[2].Vertices = new Vector2[] { new(maxX, maxY), new(FAR_AWAY, maxY), new(FAR_AWAY, minY), new(maxX, minY) };
        _colliders[3].Vertices = new Vector2[] { new(FAR_AWAY, minY), new(FAR_AWAY, -FAR_AWAY), new(-FAR_AWAY, -FAR_AWAY), new(-FAR_AWAY, minY) };
    }

    private void PlayerDied() {
        _respawnPlayer = true;
        _respawnTimer.Reset();
    }
    
    private void SpawnPlayer() {
        _player = PlayerFactory.Create(Transform.GetRoot());
    }
}
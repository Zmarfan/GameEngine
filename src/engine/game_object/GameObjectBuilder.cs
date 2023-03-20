﻿using GameEngine.engine.core.update.physics.layers;
using GameEngine.engine.data;
using GameEngine.engine.game_object.components;

namespace GameEngine.engine.game_object; 

public class GameObjectBuilder {
    private readonly string _name;
    private string _tag = "default";
    private string _layer = LayerMask.DEFAULT;
    private Transform? _parent;
    private bool _positionLocal = true;
    private Vector2 _position = Vector2.Zero();
    private bool _rotationLocal = true;
    private Rotation _rotation = Rotation.Identity();
    private bool _scaleLocal = true;
    private Vector2 _scale = Vector2.One();
    private bool _isActive = true;
    private readonly List<Component> _components = new();

    private GameObjectBuilder(string name, Transform? parent) {
        _name = name;
        _parent = parent;
    }

    public GameObject Build() {
        Transform transform = new(_parent, _position, _rotation, _scale, _positionLocal, _rotationLocal, _scaleLocal);
        _components.Add(transform);
        GameObject gameObject = new(_name, _tag, LayerMask.NameToLayer(_layer), _isActive, _components);
        _components.ForEach(component => component.InitComponent(gameObject));
        return gameObject;
    }

    public static GameObject Root() {
        return new GameObjectBuilder("root", null).Build();
    }

    public static GameObjectBuilder Builder(string name, GameObject? parent = null) {
        return new GameObjectBuilder(name, parent?.Transform);
    }

    public void SetParent(Transform parent) {
        _parent = parent;
    }
    
    public GameObjectBuilder SetTag(string tag) {
        _tag = tag;
        return this;
    }
    
    public GameObjectBuilder SetLayer(string layer) {
        _layer = layer;
        return this;
    }
    
    public GameObjectBuilder SetLocalPosition(Vector2 position) {
        _position = position;
        _positionLocal = true;
        return this;
    }

    public GameObjectBuilder SetLocalRotation(Rotation rotation) {
        _rotation = rotation;
        _rotationLocal = true;
        return this;
    }

    public GameObjectBuilder SetLocalScale(Vector2 scale) {
        _scale = scale;
        _scaleLocal = true;
        return this;
    }
    
    public GameObjectBuilder SetPosition(Vector2 position) {
        _position = position;
        _positionLocal = false;
        return this;
    }

    public GameObjectBuilder SetRotation(Rotation rotation) {
        _rotation = rotation;
        _rotationLocal = false;
        return this;
    }

    public GameObjectBuilder SetScale(Vector2 scale) {
        _scale = scale;
        _scaleLocal = false;
        return this;
    }

    public GameObjectBuilder SetIsActive(bool isActive) {
        _isActive = isActive;
        return this;
    }

    public GameObjectBuilder SetComponent(ToggleComponent component) {
        _components.Add(component);
        return this;
    }
}
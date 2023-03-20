﻿using System.Text;
using GameEngine.engine.core.game_object_handler;
using GameEngine.engine.game_object.components;
using GameEngine.engine.helper;

namespace GameEngine.engine.game_object; 

public class GameObject : Object {
    public delegate void GameObjectUpdate(GameObject gameObject);
    public delegate void ComponentAdd(ToggleComponent component);
    public static event GameObjectUpdate? GameObjectActiveEvent;
    public static event ComponentAdd? GameObjectComponentAdd;
    
    public string Name { get; set; }
    public string Tag { get; set; }
    public int Layer { get; set; }

    public bool IsActive {
        get => _isActive;
        set {
            bool changed = _isActive != value;
            _isActive = value;
            if (changed) {
                GameObjectActiveEvent?.Invoke(this);
            }
        }
    }
    private bool _isActive = true;

    public Transform Transform => _transform ??= GetComponent<Transform>();
    public readonly List<Component> components;
    private Transform? _transform;
    
    public GameObject(string name, string tag, int layer, bool isActive, List<Component> components) {
        Name = name;
        Tag = tag;
        Layer = layer;
        IsActive = isActive;
        this.components = components;
    }

    public T GetComponent<T>() where T : Component {
        try {
            return (T)components.First(static component => component is T);
        }
        catch (InvalidOperationException) {
            throw new Exception($"Unable to get component: {typeof(T)} from gameObject: {Name}");
        }
    }
    
    public List<T> GetComponents<T>() where T : Component {
        return components.Where(static component => component is T).Select(component => (T)component).ToList();
    }
    
    public bool TryGetComponent<T>(out T component) where T : Component {
        if (!components.Any(static component => component is T)) {
            component = default!;
            return false;
        }

        component = GetComponent<T>();
        return true;
    }
        
    public T GetComponentInChildren<T>() where T : Component {
        try {
            return GetComponentsInChildren<T>().First();
        }
        catch (InvalidOperationException) {
            throw new Exception($"Unable to get component: {typeof(T)} from gameObject children: {Name}");
        }
    }
    
    public List<T> GetComponentsInChildren<T>() where T : Component {
        List<T> found = new();
        if (TryGetComponent(out T selfComponent)) {
            found.Add(selfComponent);
        }
        GetComponentsInChildrenRecursively(ref found);

        return found;
    }

    private void GetComponentsInChildrenRecursively<T>(ref List<T> found) where T : Component {
        foreach (GameObject child in Transform.children.Select(child => child.gameObject)) {
            if (child.TryGetComponent(out T component)) {
                found.Add(component);
            }
            
            child.GetComponentsInChildrenRecursively(ref found);
        }
    }
    
    public T AddComponent<T>(T component) where T : ToggleComponent {
        component.InitComponent(this);
        GameObjectComponentAdd?.Invoke(component);
        return component;
    }
    
    public override string ToString() {
        string componentsString = components.Count == 0 ? string.Empty : components
            .Select(component => $"{component}\n")
            .Aggregate(new StringBuilder("Components: "), (acc, entry) => acc.Append(entry))
            .ToString();
        return $"GameObject: {Name}, IsActive: {IsActive}, Components: {componentsString}";
    }
}
﻿using Worms.engine.data;

namespace Worms.engine.game_object.components.colliders; 

public abstract class Collider : ToggleComponent {
    public static readonly Color GIZMO_COLOR = new(0.1059f, 0.949f, 0.3294f);
    
    public bool isTrigger;
    public Vector2 offset;

    protected Collider(bool isActive, bool isTrigger, Vector2 offset) : base(isActive) {
        this.isTrigger = isTrigger;
        this.offset = offset;
    }

    public abstract bool IsPointInside(Vector2 p);
}
﻿using Worms.engine.core.game_object_handler;
using Worms.engine.core.update.physics.layers;
using Worms.engine.data;
using Worms.engine.game_object;
using Worms.engine.game_object.components.physics;
using Worms.engine.game_object.components.physics.colliders;

namespace Worms.engine.core.update.physics.updating; 

public static class CollisionResolver {
    private const float POSITIONAL_CORRECTION_AMOUNT_FRACTION = 0.2f;
    private const float POSITIONAL_CORRECTION_SLOP_FRACTION = 0.01f;
    
    public static void ResolveCollisions(
        TrackObject obj,
        Dictionary<GameObject, TrackObject> objects,
        ref HashSet<Tuple<RigidBody, RigidBody>> checkedPairs
    ) {
        if (obj.Collider is not { IsActive: true } || obj.Collider.state == ColliderState.TRIGGER || obj.RigidBody == null) {
            return;
        }

        foreach ((GameObject gameObject, TrackObject checkObj) in objects) {
            if (!checkObj.isActive
                || checkObj.RigidBody == null
                || obj.Collider.gameObject == gameObject
                || !LayerMask.CanLayersInteract(obj.Collider.gameObject.Layer, gameObject.Layer)
                || checkObj.Collider is not { IsActive: true }
                || checkObj.Collider.state == ColliderState.TRIGGER
                || PairIsAlreadyChecked(obj.RigidBody, checkObj.RigidBody, checkedPairs)
               ) {
                continue;
            }

            checkedPairs.Add(new Tuple<RigidBody, RigidBody>(obj.RigidBody, checkObj.RigidBody));
            
            if (IntersectUtils.DoCollidersIntersect(obj.Collider, checkObj.Collider)) {
                ResolveCollision(obj.RigidBody, checkObj.RigidBody, 20, Vector2.Left()); // FIX NORMAL
            }
        }
    }

    private static bool PairIsAlreadyChecked(RigidBody obj1, RigidBody obj2, IEnumerable<Tuple<RigidBody, RigidBody>> checkedPairs) {
        return checkedPairs.Any(pair =>
            Equals(pair, new Tuple<RigidBody, RigidBody>(obj1, obj2))
            || Equals(pair, new Tuple<RigidBody, RigidBody>(obj2, obj1)));
    }
    
    private static void ResolveCollision(RigidBody a, RigidBody b, float penetration, Vector2 collisionNormal) {
        float velocityAlongNormal = Vector2.Dot(b.velocity - a.velocity, collisionNormal);
        if (velocityAlongNormal > 0) {
            return;
        }

        float bounciness = Math.Min(a.bounciness, b.bounciness);
        float j = -(1 + bounciness) * velocityAlongNormal;
        j /= a.InverseMass + b.InverseMass;
        Vector2 impulse = j * collisionNormal;
        a.velocity -= a.InverseMass * impulse;
        b.velocity += b.InverseMass * impulse;

        PositionalCorrection(a, b, penetration, collisionNormal);
    }

    private static void PositionalCorrection(RigidBody a, RigidBody b, float penetration, Vector2 normal) {
        Vector2 correction = Math.Max(penetration - POSITIONAL_CORRECTION_SLOP_FRACTION, 0)
            / (a.InverseMass + b.InverseMass) * POSITIONAL_CORRECTION_AMOUNT_FRACTION * normal;
        a.Transform.Position -= a.InverseMass * correction;
        b.Transform.Position += b.InverseMass * correction;
    }
}
﻿using Worms.engine.data;
using Worms.engine.game_object.components;
using Worms.engine.game_object.components.physics.colliders;

namespace Worms.engine.core.update.physics; 

public static class TriggerIntersectUtils {
    private const int CIRCLE_TO_POLYGON_POINT_COUNT = 15;
    
    public static bool DoesBoxOnBoxOverlap(BoxCollider c1, BoxCollider c2) {
        return c1.Transform.Rotation == c2.Transform.Rotation
            ? DoRectanglesOverlap(c1, c2)
            : DoConvexPolygonsOverlap(c1.WorldCorners, c2.WorldCorners);
    }

    public static bool DoesCircleOnCircleOverlap(CircleCollider c1, CircleCollider c2) {
        if (IsScaleUniform(c1) && IsScaleUniform(c2)) {
            return DoCirclesOverlap(c1.Center, c2.Center, c1.radius * c1.Transform.Scale.x, c2.radius * c2.Transform.Scale.x);
        }

        // We first do a check to see if total bounding circles overlap to avoid unnecessary checking if they don't
        if (DoCirclesOverlap(c1.Center, c2.Center, c1.BoundingRadius, c2.BoundingRadius)) {
            // This check is an approximation and NOT exact mathematically. Ellipses are weird and hard, no fun ):
            // Here we transform one of the ellipses to a convex polygon and then we check if the circle intersect it 
            return DoCirclePolygonOverlap(c1, c2, c2.GetCircleAsPoints(CIRCLE_TO_POLYGON_POINT_COUNT));
        }

        return false;
    }

    public static bool DoesPixelOnPixelOverlap(PixelCollider c1, PixelCollider c2) {
        PixelCollider looper = c1.Width * c1.Height < c2.Width * c2.Height ? c1 : c2;
        PixelCollider checker = looper == c1 ? c2 : c1;
        
        for (int x = 0; x < looper.Width; x++) {
            for (int y = 0; y < looper.Height; y++) {
                if (!looper.pixels[x, y].IsOpaque) {
                    continue;
                }

                Vector2 world = looper.Transform.LocalToWorldMatrix.ConvertPoint(looper.PixelToLocal(new Vector2Int(x, y)));
                if (checker.IsPointInside(world)) {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool DoesBoxOnCircleOverlap(CircleCollider c1, BoxCollider c2) {
        return DoCirclePolygonOverlap(c1, c2, c2.WorldCorners);
    }

    public static bool DoesBoxOnPixelOverlap(Collider c1, Collider c2) {
        return false;
    }

    public static bool DoesCircleOnPixelOverlap(Collider c1, Collider c2) {
        return false;
    }
    
    private static bool DoRectanglesOverlap(BoxCollider c1, BoxCollider c2) {
        Vector2 c2BottomLeft = c1.Transform.WorldToLocalMatrix.ConvertPoint(
            c2.Transform.LocalToWorldMatrix.ConvertPoint(c2.BottomLeftLocal)
        );
        Vector2 c2TopRight = c1.Transform.WorldToLocalMatrix.ConvertPoint(
            c2.Transform.LocalToWorldMatrix.ConvertPoint(c2.TopRightLocal)
        );
        return c1.BottomLeftLocal.x < c2TopRight.x
               && c1.TopRightLocal.x > c2BottomLeft.x
               && c1.BottomLeftLocal.y < c2TopRight.y
               && c1.TopRightLocal.y > c2BottomLeft.y;
    }
    
    private static bool DoConvexPolygonsOverlap(List<Vector2> c1Points, List<Vector2> c2Points) {
        foreach (List<Vector2> points in new[] { c1Points, c2Points })
        {
            for (int i1 = 0; i1 < points.Count; i1++)
            {
                int i2 = (i1 + 1) % points.Count;
                Vector2 normal = new(points[i2].y - points[i1].y, points[i1].x - points[i2].x);

                FindMinMaxBoxPointsAlongNormal(c1Points, normal, out float minA, out float maxA);
                FindMinMaxBoxPointsAlongNormal(c2Points, normal, out float minB, out float maxB);

                if (maxA < minB || maxB < minA) {
                    return false;
                }
            }
        }
        return true;
    }
    
    private static void FindMinMaxBoxPointsAlongNormal(IEnumerable<Vector2> corners, Vector2 normal, out float min, out float max) {
        min = float.MaxValue;
        max = float.MinValue;
        foreach (float projected in corners.Select(p => Vector2.Dot(normal, p))) {
            min = projected < min ? projected : min;
            max = projected > max ? projected : max;
        }
    }

    private static bool DoCirclesOverlap(
        Vector2 c1Center,
        Vector2 c2Center,
        float c1Radius,
        float c2Radius
    ) {
        Vector2 distance = c1Center - c2Center;
        float radiusSum = c1Radius + c2Radius;
        return distance.x * distance.x + distance.y * distance.y <= radiusSum * radiusSum;
    }

    private static bool DoCirclePolygonOverlap(CircleCollider c1, Collider c2, IReadOnlyList<Vector2> c2Points) {
        if (c1.IsPointInside(c2.Center) || c2.IsPointInside(c1.Center)) {
            return true;
        }

        int fromIndex = c2Points.Count - 1;
        for (int toIndex = 0; toIndex < c2Points.Count; toIndex++) {
            Vector2 origin = c1.Transform.WorldToLocalMatrix.ConvertPoint(c2Points[fromIndex]);
            Vector2 direction = c1.Transform.WorldToLocalMatrix.ConvertPoint(c2Points[toIndex]) - origin;
            if (PhysicsUtils.LineCircleIntersection(origin, direction, c1.offset, c1.radius, out _)) {
                return true;
            }
            fromIndex = toIndex;
        }
        
        return false;
    }
    
    private static bool IsScaleUniform(Component c) {
        return Math.Abs(c.Transform.Scale.x - c.Transform.Scale.y) < 0.001f;
    }
}
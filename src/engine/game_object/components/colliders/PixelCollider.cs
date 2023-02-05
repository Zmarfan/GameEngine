﻿using System.Diagnostics;
using Worms.engine.core.gizmos;
using Worms.engine.core.update;
using Worms.engine.data;

namespace Worms.engine.game_object.components.colliders; 

public class PixelCollider : Collider {
    private const int NORMAL_CHECK_DEPTH = 4;
    private const int MIN_SURROUNDING_PIXELS_FOR_VALID_NORMAL = 16;
    
    public Color[,] pixels;

    public bool flipX;
    public bool flipY; 
    private int Width => pixels.GetLength(0);
    private int Height => pixels.GetLength(1);
    private int EvenWidthOffset => (Width + (flipX ? 0 : 1)) % 2;
    private int EvenHeightOffset => (Height + (flipY ? 0 : 1)) % 2;

    private int FlipXSign => flipX ? -1 : 1;
    private int FlipYSign => flipY ? 1 : -1;

    public PixelCollider(
        bool isActive,
        Color[,] pixels,
        bool flipX,
        bool flipY,
        bool isTrigger,
        Vector2 offset
    ) : base(isActive, isTrigger, new Vector2((int)offset.x, (int)offset.y)) {
        this.flipX = flipX;
        this.flipY = flipY;
        this.pixels = pixels;
    }

    public override bool IsPointInside(Vector2 p) {
        p = Transform.WorldToLocalMatrix.ConvertPoint(p);
        Vector2 pixel = LocalToPixel(p);
        if (!PixelIsInTexture(pixel)) {
            return false;
        }
        return pixels[(int)pixel.x, (int)pixel.y].IsOpaque;
    }
    
    public override ColliderHit? Raycast(Vector2 origin, Vector2 direction) {
        origin = Transform.WorldToLocalMatrix.ConvertPoint(origin);
        Vector2 to = origin + Transform.WorldToLocalMatrix.ConvertVector(direction);
        origin = LocalToPixel(origin);
        to = LocalToPixel(to);

        if (PixelIsInTexture(origin) && pixels[(int)origin.x, (int)origin.y].IsOpaque) {
            return null;
        }
        
        Vector2? pixel = CalculatePointLineHits((int)origin.x, (int)origin.y, (int)to.x, (int)to.y);
        if (!pixel.HasValue) {
            return null;
        }

        return new ColliderHit(
            Transform.LocalToWorldMatrix.ConvertPoint(PixelToLocal(pixel.Value)),
            Transform.LocalToWorldMatrix.ConvertVector(CalculateNormal((int)pixel.Value.x, (int)pixel.Value.y)).Normalized
        );
    }

    private Vector2 CalculateNormal(int pixelX, int pixelY) {
        Vector2 point = new(pixelX, -pixelY);
        Vector2 inverseNormal = Vector2.Zero();
        int surroundingPixels = 0;
        
        for (int x = Math.Max(pixelX - NORMAL_CHECK_DEPTH, 0); x <= Math.Min(pixelX + NORMAL_CHECK_DEPTH, Width - 1); x++) {
            for (int y = Math.Max(pixelY - NORMAL_CHECK_DEPTH, 0); y <= Math.Min(pixelY + NORMAL_CHECK_DEPTH, Height - 1); y++) {
                if (!pixels[x, y].IsOpaque || (x == pixelX && y == pixelY)) {
                    continue;
                }

                surroundingPixels++;
                inverseNormal += new Vector2(x, -y) - point;
            }
        }

        if (surroundingPixels < MIN_SURROUNDING_PIXELS_FOR_VALID_NORMAL) {
            return Vector2.Zero();
        }

        return -inverseNormal;
    }

    private bool PixelIsInTexture(Vector2 pixel) {
        return pixel.x >= 0 && pixel.x < Width && pixel.y >= 0 && pixel.y < Height;
    }
    
    private Vector2 LocalToPixel(Vector2 p) {
        return new Vector2(
            FlipXSign * ((int)Math.Round(p.x) - (int)offset.x) + Width / 2 - EvenWidthOffset,
            FlipYSign * ((int)Math.Round(p.y) - (int)offset.y) + Height / 2 - EvenHeightOffset
        );
    }
    
    private Vector2 PixelToLocal(Vector2 p) {
        return new Vector2(
            FlipXSign * (p.x - Width / 2f + EvenWidthOffset) + offset.x,
            FlipYSign * (p.y - Height / 2f + EvenHeightOffset) + offset.y
        );
    }
    
    private Vector2? CalculatePointLineHits(int x, int y, int x2, int y2) {
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        dx1 = w switch {
            < 0 => -1,
            > 0 => 1,
            _ => dx1
        };
        dy1 = h switch {
            < 0 => -1,
            > 0 => 1,
            _ => dy1
        };
        dx2 = w switch {
            < 0 => -1,
            > 0 => 1,
            _ => dx2
        };

        int longest = Math.Abs(w) ;
        int shortest = Math.Abs(h) ;
        
        if (!(longest > shortest)) {
            longest = Math.Abs(h) ;
            shortest = Math.Abs(w) ;
            dy2 = h switch {
                < 0 => -1,
                > 0 => 1,
                _ => dy2
            };
            dx2 = 0;            
        }
        int numerator = longest >> 1 ;
        for (int i = 0; i <= longest; i++) {
            if (PixelIsInTexture(new Vector2(x, y)) && pixels[x, y].IsOpaque) {
                return new Vector2(x, y);
            }
            numerator += shortest;
            if (!(numerator < longest)) {
                numerator -= longest;
                x += dx1;
                y += dy1;
            } else {
                x += dx2;
                y += dy2;
            }
        }

        return null;
    }
}
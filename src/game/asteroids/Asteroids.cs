﻿using Worms.engine.core;
using Worms.engine.core.assets;
using Worms.engine.core.audio;
using Worms.engine.core.cursor;
using Worms.engine.core.gizmos;
using Worms.engine.core.input.listener;
using Worms.engine.core.renderer.textures;
using Worms.engine.core.update.physics.layers;
using Worms.engine.core.update.physics.settings;
using Worms.engine.helper;
using Worms.game.asteroids.scenes;

namespace Worms.game.asteroids; 

public static class Asteroids {
    public static Game CreateGame() {
        return new Game(GameSettingsBuilder
            .Builder()
            .SetTitle("Asteroids")
            .SetAssets(DefineAssets())
            .SetAudioSettings(new AudioSettings(Volume.Max(), ListUtils.Of(
                new AudioChannel("effects", Volume.Max()),
                new AudioChannel("music", Volume.Max())
            )))
            .SetDebugMode()
            .SetWindowWidth(1280)
            .SetWindowHeight(720)
            .AddScenes(ListUtils.Of(Scene1.GetScene()))
            .AddInputListeners(ListUtils.Of(
                InputListenerBuilder
                    .Builder(InputNames.ROTATE, Button.D)
                    .SetNegativeButton(Button.A)
                    .SetAltPositiveButton(Button.RIGHT)
                    .SetAltNegativeButton(Button.LEFT)
                    .Build(),
                InputListenerBuilder
                    .Builder(InputNames.THRUST, Button.W)
                    .SetAltPositiveButton(Button.UP)
                    .SetSensitivity(2)
                    .Build(),
                InputListenerBuilder.Builder(InputNames.FIRE, Button.SPACE).Build()
            ))
            .SetPhysics(PhysicsSettingsBuilder
                .Builder(ListUtils.Of(LayerMask.DEFAULT), ListUtils.Of(LayerMask.IGNORE_RAYCAST))
                .AddLayer(LayerNames.PLAY_AREA_OBJECT, ListUtils.Of(LayerNames.PLAY_AREA_OBJECT))
                .Build()
            )
            .SetCursorSettings(new CursorSettings(false, new CustomCursorSettings(Path("test\\cursor.png"))))
            .SetGizmoSettings(GizmoSettingsBuilder
                .Builder()
                .ShowColliders(false)
                .ShowBoundingBoxes(false)
                .ShowPolygonTriangles(true)
                .Build()
            )
            .Build()
        );
    }
    
    private static Assets DefineAssets() {
        return AssetsBuilder
            .Builder()
            .AddTextures(ListUtils.Of(
                new AssetDeclaration(Path("asteroids\\textures\\big_asteroid_1.png"), TextureNames.BIG_ASTEROID_1),
                new AssetDeclaration(Path("asteroids\\textures\\big_asteroid_2.png"), TextureNames.BIG_ASTEROID_2),
                new AssetDeclaration(Path("asteroids\\textures\\big_asteroid_3.png"), TextureNames.BIG_ASTEROID_3),
                new AssetDeclaration(Path("asteroids\\textures\\medium_asteroid_1.png"), TextureNames.MEDIUM_ASTEROID_1),
                new AssetDeclaration(Path("asteroids\\textures\\medium_asteroid_2.png"), TextureNames.MEDIUM_ASTEROID_2),
                new AssetDeclaration(Path("asteroids\\textures\\medium_asteroid_3.png"), TextureNames.MEDIUM_ASTEROID_3),
                new AssetDeclaration(Path("asteroids\\textures\\small_asteroid_1.png"), TextureNames.SMALL_ASTEROID_1),
                new AssetDeclaration(Path("asteroids\\textures\\small_asteroid_2.png"), TextureNames.SMALL_ASTEROID_2),
                new AssetDeclaration(Path("asteroids\\textures\\small_asteroid_3.png"), TextureNames.SMALL_ASTEROID_3),
                new AssetDeclaration(Path("asteroids\\textures\\player.png"), TextureNames.PLAYER),
                new AssetDeclaration(Path("asteroids\\textures\\enemy.png"), TextureNames.ENEMY),
                new AssetDeclaration(Path("asteroids\\textures\\shot.png"), TextureNames.SHOT)
            ))
            .Build();
    }
    
    private static string Path(string path) {
        return $"{Directory.GetCurrentDirectory()}\\src\\assets\\{path}";
    }
}
﻿using Worms.engine.data;
using Worms.engine.game_object.components.rendering.texture_renderer;
using Worms.engine.game_object.scripts;

namespace Worms.game; 

public class TriggerScriptTest : Script {
    private TextureRenderer _textureRenderer = null!;
    
    public TriggerScriptTest() : base(true) {
    }

    public override void Start() {
        _textureRenderer = GetComponent<TextureRenderer>();
    }

    public override void OnMouseEnter() {
        _textureRenderer.Color = new Color(1, 1, 1, 0.5f);
    }

    public override void OnMouseClick() {
        Console.WriteLine("click");
    }
    
    public override void OnMouseExit() {
        _textureRenderer.Color = Color.WHITE;
    }
}
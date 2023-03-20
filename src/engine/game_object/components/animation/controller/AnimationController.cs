﻿using GameEngine.engine.game_object.components.animation.animation;
using GameEngine.engine.game_object.scripts;

namespace GameEngine.engine.game_object.components.animation.controller; 

public class AnimationController : Script {
    private readonly Dictionary<string, Animation> _animationsByTriggers;
    private Animation? _currentAnimation;
    
    public AnimationController(
        Animation? startAnimation,
        IEnumerable<Tuple<string, Animation>> animationsWithTriggers,
        bool isActive,
        string name
    ) : base(isActive, name) {
        _animationsByTriggers = animationsWithTriggers.ToDictionary(a => a.Item1, a => a.Item2);
        _currentAnimation = startAnimation;
    }
    
    public void Stop() {
        _currentAnimation = null;
    }

    public void SetTrigger(string trigger) {
        if (!_animationsByTriggers.ContainsKey(trigger)) {
            throw new ArgumentException($"There exist no trigger in this animationController with the name: {trigger}");
        }

        _currentAnimation = _animationsByTriggers[trigger];
        _currentAnimation.Reset();
    }
    
    public override void Start() {
        foreach ((string _, Animation animation) in _animationsByTriggers) {
            animation.Init(gameObject);
        }
    }

    public override void Update(float deltaTime) {
        _currentAnimation?.Play(deltaTime);
    }
}
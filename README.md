# GameEngine - Currently without name

This vision behind this project was to construct a 2D game engine using only C# and a C# port of [SDL](https://www.libsdl.org/). This includes things such as: 
- Graphics, rendering and texture handling
- Audio and mixers
- Physics
- Assets
- Gameobject hierarchy with translation, rotation and scale
- Rotational matrixes and quaternions
- Camera controls
- Particle systems
- Scenes
- Gizmos
- Save system
- Cursor control
- Fixed and non-fixed update loops and event callback functions

## How to use
The goal is to only write code with no interfaces or UI but to have the game engine as library to easily implement functionality without the need to handle low level concepts such as audio channels, render calls, object handling etc.

## Games using the engine
Two example games has been developed utilizing the game engine:

### [Minesweeper Clone](https://github.com/Zmarfan/Asteroids)
![minesweeper-img](https://github.com/Zmarfan/GameEngine/blob/main/src/images/minesweeper.png?raw=true)

### [Asteroids Clone](https://github.com/Zmarfan/Minesweeper)
![asteroids-img-0](https://github.com/Zmarfan/GameEngine/blob/main/src/images/asteroids_0.png?raw=true)
![asteroids-img-1](https://github.com/Zmarfan/GameEngine/blob/main/src/images/asteroids_1.png?raw=true)

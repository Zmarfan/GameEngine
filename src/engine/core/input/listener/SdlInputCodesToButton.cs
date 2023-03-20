﻿using SDL2;

namespace GameEngine.engine.core.input.listener; 

public static class SdlInputCodeToButton {
    public static readonly Dictionary<SDL.SDL_Scancode, Button> SCANCODE_TO_BUTTON = new() {
        { SDL.SDL_Scancode.SDL_SCANCODE_SPACE, Button.SPACE },
        { SDL.SDL_Scancode.SDL_SCANCODE_KP_SPACE, Button.SPACE },
        { SDL.SDL_Scancode.SDL_SCANCODE_RETURN, Button.RETURN },
        { SDL.SDL_Scancode.SDL_SCANCODE_LCTRL, Button.CTRL },
        { SDL.SDL_Scancode.SDL_SCANCODE_RCTRL, Button.CTRL },
        { SDL.SDL_Scancode.SDL_SCANCODE_LSHIFT, Button.SHIFT },
        { SDL.SDL_Scancode.SDL_SCANCODE_RSHIFT, Button.SHIFT },
        { SDL.SDL_Scancode.SDL_SCANCODE_CAPSLOCK, Button.CAPS_LOCK },
        { SDL.SDL_Scancode.SDL_SCANCODE_TAB, Button.TAB },
        { SDL.SDL_Scancode.SDL_SCANCODE_KP_TAB, Button.TAB },
        { SDL.SDL_Scancode.SDL_SCANCODE_LALT, Button.ALT },
        { SDL.SDL_Scancode.SDL_SCANCODE_RALT, Button.ALT },
        { SDL.SDL_Scancode.SDL_SCANCODE_KP_ENTER, Button.ENTER },
        { SDL.SDL_Scancode.SDL_SCANCODE_BACKSPACE, Button.BACKSPACE },
        { SDL.SDL_Scancode.SDL_SCANCODE_KP_BACKSPACE, Button.BACKSPACE },
        { SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE, Button.ESCAPE },
        { SDL.SDL_Scancode.SDL_SCANCODE_LEFT, Button.LEFT },
        { SDL.SDL_Scancode.SDL_SCANCODE_RIGHT, Button.RIGHT },
        { SDL.SDL_Scancode.SDL_SCANCODE_UP, Button.UP },
        { SDL.SDL_Scancode.SDL_SCANCODE_DOWN, Button.DOWN },
        { SDL.SDL_Scancode.SDL_SCANCODE_1, Button.NUM_1 },
        { SDL.SDL_Scancode.SDL_SCANCODE_2, Button.NUM_2 },
        { SDL.SDL_Scancode.SDL_SCANCODE_3, Button.NUM_3 },
        { SDL.SDL_Scancode.SDL_SCANCODE_4, Button.NUM_4 },
        { SDL.SDL_Scancode.SDL_SCANCODE_5, Button.NUM_5 },
        { SDL.SDL_Scancode.SDL_SCANCODE_6, Button.NUM_6 },
        { SDL.SDL_Scancode.SDL_SCANCODE_7, Button.NUM_7 },
        { SDL.SDL_Scancode.SDL_SCANCODE_8, Button.NUM_8 },
        { SDL.SDL_Scancode.SDL_SCANCODE_9, Button.NUM_9 },
        { SDL.SDL_Scancode.SDL_SCANCODE_0, Button.NUM_0 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F1, Button.F1 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F2, Button.F2 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F3, Button.F3 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F4, Button.F4 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F5, Button.F5 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F6, Button.F6 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F7, Button.F7 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F8, Button.F8 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F9, Button.F9 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F10, Button.F10 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F11, Button.F11 },
        { SDL.SDL_Scancode.SDL_SCANCODE_F12, Button.F12 },
        { SDL.SDL_Scancode.SDL_SCANCODE_A, Button.A },
        { SDL.SDL_Scancode.SDL_SCANCODE_B, Button.B },
        { SDL.SDL_Scancode.SDL_SCANCODE_C, Button.C },
        { SDL.SDL_Scancode.SDL_SCANCODE_D, Button.D },
        { SDL.SDL_Scancode.SDL_SCANCODE_E, Button.E },
        { SDL.SDL_Scancode.SDL_SCANCODE_F, Button.F },
        { SDL.SDL_Scancode.SDL_SCANCODE_G, Button.G },
        { SDL.SDL_Scancode.SDL_SCANCODE_H, Button.H },
        { SDL.SDL_Scancode.SDL_SCANCODE_I, Button.I },
        { SDL.SDL_Scancode.SDL_SCANCODE_J, Button.J },
        { SDL.SDL_Scancode.SDL_SCANCODE_K, Button.K },
        { SDL.SDL_Scancode.SDL_SCANCODE_L, Button.L },
        { SDL.SDL_Scancode.SDL_SCANCODE_M, Button.M },
        { SDL.SDL_Scancode.SDL_SCANCODE_N, Button.N },
        { SDL.SDL_Scancode.SDL_SCANCODE_O, Button.O },
        { SDL.SDL_Scancode.SDL_SCANCODE_P, Button.P },
        { SDL.SDL_Scancode.SDL_SCANCODE_Q, Button.Q },
        { SDL.SDL_Scancode.SDL_SCANCODE_R, Button.R },
        { SDL.SDL_Scancode.SDL_SCANCODE_S, Button.S },
        { SDL.SDL_Scancode.SDL_SCANCODE_T, Button.T },
        { SDL.SDL_Scancode.SDL_SCANCODE_U, Button.U },
        { SDL.SDL_Scancode.SDL_SCANCODE_V, Button.V },
        { SDL.SDL_Scancode.SDL_SCANCODE_W, Button.W },
        { SDL.SDL_Scancode.SDL_SCANCODE_X, Button.X },
        { SDL.SDL_Scancode.SDL_SCANCODE_Y, Button.Y },
        { SDL.SDL_Scancode.SDL_SCANCODE_Z, Button.Z }
    };

    public static readonly Dictionary<uint, Button> MOUSE_BUTTON_TO_BUTTON = new() {
        { SDL.SDL_BUTTON_LEFT, Button.LEFT_MOUSE },
        { SDL.SDL_BUTTON_RIGHT, Button.RIGHT_MOUSE },
        { SDL.SDL_BUTTON_MIDDLE, Button.MIDDLE_MOUSE }
    };
}
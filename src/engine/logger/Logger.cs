﻿namespace GameEngine.engine.logger; 

internal static class Logger {
    public static void Error(Exception e, string text = "") {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{text}; exception: {e}");
        ResetColor();
    }
    
    public static void Warning(string text, Exception e) {
        Console.BackgroundColor = ConsoleColor.DarkYellow;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{text}; exception: {e}");
        ResetColor();
    }

    private static void ResetColor() {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Gray;
    }
}
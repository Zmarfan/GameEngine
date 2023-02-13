﻿using System.Text;
using Worms.engine.core.renderer.font;

namespace Worms.engine.game_object.components.rendering.text_renderer; 

public static class TextFormatter {
    public static List<string> FormatText(string text, int width, int size, Font font) {
        float sizeModifier = size / (float)Font.FONT_SIZE;
        List<string> allLines = new();
        
        StringBuilder line = new();
        float lineWidth = 0;
        StringBuilder word = new();
        float wordWidth = 0;
        
        for (int i = 0; i < text.Length; i++) {
            char c = font.characters.ContainsKey(text[i]) || text[i] == '\n' ? text[i] : '?';
            
            if (c == '\n') {
                if (lineWidth + wordWidth >= width) {
                    allLines.Add(line.ToString());
                    line.Clear();
                }
                else if (lineWidth != 0) {
                    line.Append(' ');
                }

                line.Append(word);
                
                allLines.Add(line.ToString());

                wordWidth = 0;
                lineWidth = 0;
                line.Clear();
                word.Clear();
                continue;
            }

            CharacterInfo info = font.characters[c];
            if (c != ' ') {
                wordWidth += info.dimension.x * sizeModifier;
            }

            if (wordWidth >= width) {
                if (lineWidth != 0) {
                    allLines.Add(line.ToString());
                    lineWidth = 0;
                    line.Clear();
                }
                
                allLines.Add(word.ToString());

                wordWidth = info.dimension.x * sizeModifier;
                word.Clear();
                word.Append(c);
                continue;
            }

            if (c != ' ') {
                word.Append(c);
            }
            
            if (c == ' ' || i == text.Length - 1) {
                if (lineWidth + wordWidth >= width) {
                    allLines.Add(line.ToString());
                    lineWidth = 0;
                    line.Clear();
                }
                else if (lineWidth != 0) {
                    line.Append(' ');
                    lineWidth += font.characters[' '].dimension.x * sizeModifier;
                }

                line.Append(word);
                lineWidth += wordWidth;
                wordWidth = 0;
                word.Clear();
            }
        }

        allLines.Add(line.ToString());
        return allLines;
    }
}
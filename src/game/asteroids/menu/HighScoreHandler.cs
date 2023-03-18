﻿using Worms.engine.core.saving;

namespace Worms.game.asteroids.menu; 

public static class HighScoreHandler {
    private const string SAVE_NAME = "high_scores";
    
    public static List<HighScoreEntry> GetHighScores() {
        return !SaveManager.HasData<List<HighScoreEntry>>(SAVE_NAME)
            ? new List<HighScoreEntry>()
            : SaveManager.Load<List<HighScoreEntry>>(SAVE_NAME).OrderBy(score => score.score).ToList();
    }

    public static void SaveHighScores(List<HighScoreEntry> highScoreEntries) {
        SaveManager.Save(SAVE_NAME, highScoreEntries);
    }
}
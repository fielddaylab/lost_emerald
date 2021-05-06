using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Shipwreck;

namespace Shipwreck.Level
{
    public abstract class LevelBase
    {
        public abstract bool CheckboxStatus(PlayerProgress progress, string checkboxKey);

        public abstract bool NotificationStatus(PlayerProgress progress, string notificationKey);

        public abstract string PickConversation(PlayerProgress progress, string charName, out string bubble);

        public abstract string CurrentThought(PlayerProgress progress);

        public abstract bool CanShipOut(PlayerProgress progress);

        public abstract bool ChapterComplete(PlayerProgress progress);
    }

    public static class LevelHelper
    {
        private static List<string> levelOrder = new List<string>{
            "loretta",
            "level2",
            "level3"
        };

        public static string CurrentScene() {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        public static bool IsNextLevel(string LevelID) {
            int nextLevelIdx = levelOrder.IndexOf(PlayerProgress.instance.GetCurrentLevel()) + 1;
            return levelOrder[nextLevelIdx] == LevelID;
        }
    }
}


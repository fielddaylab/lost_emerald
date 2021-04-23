using UnityEngine;
using System.Collections;

public class Level2 : LevelBase
{
    override public bool CheckboxStatus(PlayerProgress progress, string checkboxKey)
    {
        bool check = false;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LaSalleTestScene_RealtimeLighting")
        {
            if (checkboxKey == "dive-photo" && progress.IsUnlocked("photo-birds-eye"))
            {
                check = true;
            }
            if (checkboxKey == "photo-ship-name" && progress.IsUnlocked("photo-ship-name"))
            {
                check = true;
            }
        }
        return check;
    } 

    override public bool NotificationStatus(PlayerProgress progress, string notificationKey)
    {
        bool showNotificiation = false;
        return false;
    }

    override public string PickConversation(PlayerProgress progress, string charName, out string bubble)
    {
        bubble = null;
        return null;
    }

    override public string CurrentThought(PlayerProgress progress)
    {
        return "This is Level 2! There's nothing here yet.";
    }

    override public bool CanShipOut(PlayerProgress progress)
    {
        return true;
    }

    override public bool ChapterComplete(PlayerProgress progress)
    {
        return false;
    }
}

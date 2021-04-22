using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class LevelBase
{
    public abstract bool CheckboxStatus(PlayerProgress progress, string checkboxKey);

    public abstract bool NotificationStatus(PlayerProgress progress, string notificationKey);

    public abstract string PickConversation(PlayerProgress progress, string charName, out string bubble);

    public abstract string CurrentThought(PlayerProgress progress);

    public abstract bool CanShipOut(PlayerProgress progress);

    public abstract bool ChapterComplete(PlayerProgress progress);
}

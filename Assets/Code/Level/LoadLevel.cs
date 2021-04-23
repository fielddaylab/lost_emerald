using UnityEngine;
using System.Collections;

public class LoadLevel : SceneSwitch
{
    public string LevelID;

    public void StartLevel()
    {
        PlayerProgress.instance.LoadLevel(LevelID);
        GotoDesk();
    }
}

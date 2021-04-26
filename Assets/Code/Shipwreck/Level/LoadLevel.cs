using UnityEngine;
using System.Collections;
using Shipwreck;

public class LoadLevel : SceneSwitch
{
    public string LevelID;

    public void StartLevel()
    {
        PlayerProgress.instance.LoadLevel(LevelID);
        Logging.instance?.LogMissionStart(LevelID);
        GotoDesk();
    }
}

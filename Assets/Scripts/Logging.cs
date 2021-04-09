using System.Collections.Generic;
using System.Runtime.InteropServices;
using FieldDay;
using UnityEngine;

public class Logging : MonoBehaviour
{
    #region Firebase JS Functions

    [DllImport("__Internal")]
    public static extern void FBMissionStart(string missionId);
    [DllImport("__Internal")]
    public static extern void FBViewTab(string missionId, string tabName);
    [DllImport("__Internal")]
    public static extern void FBViewDesk(string missionId);
    [DllImport("__Internal")]
    public static extern void FBViewChat(string missionId, string chatName);
    [DllImport("__Internal")]
    public static extern void FBOpenMap(string missionId);
    [DllImport("__Internal")]
    public static extern void FBScanStart(string missionId);
    [DllImport("__Internal")]
    public static extern void FBScanComplete(string missionId);
    [DllImport("__Internal")]
    public static extern void FBDiveStart(string missionId);
    [DllImport("__Internal")]
    public static extern void FBPlayerUnlock(string missionId, string unlockKey);
    [DllImport("__Internal")]
    public static extern void FBUpdateShipOverview(string missionId, string targetKey, string infoKey, string infoDisplay, string sourceDisplay);
    [DllImport("__Internal")]
    public static extern void FBMissionComplete(string missionId);

    #endregion // Firebase JS Functions

    public static Logging instance;

    private SimpleLog logger;
    private string appId = "SHIPWRECKS";
    private int appVersion = 1;

    private enum eventCategories
    {
        mission_start,
        view_tab,
        view_desk,
        view_chat,
        open_map,
        scan_start,
        scan_complete,
        dive_start,
        player_unlock,
        new_evidence,
        update_ship_overview,
        mission_complete
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            logger = new SimpleLog(appId, appVersion, null);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void LogMissionStart(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.mission_start));
        FBMissionStart(missionId);
    }

    public void LogViewTab(string missionId, string tabName)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "tab_name", tabName}
        };

        logger.Log(new LogEvent(data, eventCategories.view_tab));
        FBViewTab(missionId, tabName);
    }

    public void LogViewDesk(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.view_desk));
        FBViewDesk(missionId);
    }

    public void LogViewChat(string missionId, string chatName)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "chat_name", chatName }
        };

        logger.Log(new LogEvent(data, eventCategories.mission_start));
        FBViewChat(missionId, chatName);
    }

    public void LogOpenMap(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.open_map));
        FBOpenMap(missionId);
    }

    public void LogScanStart(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.scan_start));
        FBScanStart(missionId);
    }

    public void LogScanComplete(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.scan_complete));
        FBScanComplete(missionId);
    }

    public void LogDiveStart(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.dive_start));
        FBDiveStart(missionId);
    }

    public void LogPlayerUnlock(string missionId, string unlockKey)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "unlock_key", unlockKey }
        };

        logger.Log(new LogEvent(data, eventCategories.player_unlock));
        FBPlayerUnlock(missionId, unlockKey);
    }

    public void LogUpdateShipOverview(string missionId, string targetKey, string infoKey, string infoDisplay, string sourceDisplay)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "targetKey", targetKey },
            { "infoKey", infoKey },
            { "infoDisplay", infoDisplay },
            { "sourceDisplay", sourceDisplay }
        };

        logger.Log(new LogEvent(data, eventCategories.update_ship_overview));
        FBUpdateShipOverview(missionId, targetKey, infoKey, infoDisplay, sourceDisplay);
    }

    public void LogMissionComplete(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.mission_complete));
        FBMissionComplete(missionId);
    }
}

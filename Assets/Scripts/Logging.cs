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

    private int previousPercent = 0;

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
        mission_complete,
        scan_percentage_change
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

        #if !UNITY_EDITOR
        FBMissionStart(missionId);
        #endif
    }

    public void LogViewTab(string missionId, string tabName)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "tab_name", tabName}
        };

        logger.Log(new LogEvent(data, eventCategories.view_tab));

        #if !UNITY_EDITOR
        FBViewTab(missionId, tabName);
        #endif
    }

    public void LogViewDesk(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.view_desk));

        #if !UNITY_EDITOR
        FBViewDesk(missionId);
        #endif
    }

    public void LogViewChat(string missionId, string chatName)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "chat_name", chatName }
        };

        logger.Log(new LogEvent(data, eventCategories.mission_start));

        #if !UNITY_EDITOR
        FBViewChat(missionId, chatName);
        #endif
    }

    public void LogOpenMap(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.open_map));

        #if !UNITY_EDITOR
        FBOpenMap(missionId);
        #endif
    }

    public void LogScanStart(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.scan_start));

        #if !UNITY_EDITOR
        FBScanStart(missionId);
        #endif
    }

    public void LogScanComplete(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.scan_complete));

        #if !UNITY_EDITOR
        FBScanComplete(missionId);
        #endif
    }

    public void LogDiveStart(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.dive_start));

        #if !UNITY_EDITOR
        FBDiveStart(missionId);
        #endif
    }

    public void LogPlayerUnlock(string missionId, string unlockKey)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "unlock_key", unlockKey }
        };

        logger.Log(new LogEvent(data, eventCategories.player_unlock));

        #if !UNITY_EDITOR
        FBPlayerUnlock(missionId, unlockKey);
        #endif
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

        #if !UNITY_EDITOR
        FBUpdateShipOverview(missionId, targetKey, infoKey, infoDisplay, sourceDisplay);
        #endif
    }

    public void LogMissionComplete(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.mission_complete));

        #if !UNITY_EDITOR
        FBMissionComplete(missionId);
        #endif
    }

    public void LogScanPercentageChange(string missionId, int scanPercent)
    {
        if (scanPercent != previousPercent)
        {
            previousPercent = scanPercent;

            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "mission_id", missionId },
                { "scan_percent", scanPercent.ToString() }
            };

            logger.Log(new LogEvent(data, eventCategories.scan_percentage_change), true);
        }
    }
}

using System.Collections.Generic;
using FieldDay;
using UnityEngine;

public class Logging : MonoBehaviour
{
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
    }

    public void LogViewTab(string missionId, string tabName)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "tab_name", tabName}
        };

        logger.Log(new LogEvent(data, eventCategories.view_tab));
    }

    public void LogViewDesk(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.view_desk));
    }

    public void LogViewChat(string missionId, string chatName)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "chat_name", chatName }
        };

        logger.Log(new LogEvent(data, eventCategories.mission_start));
    }

    public void LogOpenMap(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.open_map));
    }

    public void LogScanStart(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.scan_start));
    }

    public void LogScanComplete(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.scan_complete));
    }

    public void LogDiveStart(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.dive_start));
    }

    public void LogPlayerUnlock(string mission_id, string unlock_key)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", mission_id },
            { "unlock_key", unlock_key }
        };

        logger.Log(new LogEvent(data, eventCategories.player_unlock));
    }

    public void LogUpdateShipOverview(string targetKey, string infoKey, string infoDisplay, string sourceDisplay)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "targetKey", targetKey },
            { "infoKey", infoKey },
            { "infoDisplay", infoDisplay },
            { "sourceDisplay", sourceDisplay }
        };

        logger.Log(new LogEvent(data, eventCategories.update_ship_overview));
    }

    public void LogMissionComplete(string missionId)
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.mission_complete));
    }
}

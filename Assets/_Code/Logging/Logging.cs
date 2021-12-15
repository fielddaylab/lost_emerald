#if !UNITY_EDITOR
#define FIREBASE
#endif // !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using BeauUtil;
using FieldDay;
using Shipwreck;
using UnityEngine;

public class Logging : MonoBehaviour
{
    #region Consts

    private static readonly StringHash32 RootChain_Location = "Location";
    private static readonly StringHash32 RootChain_Type = "Type";
    private static readonly StringHash32 RootChain_Name = "Name";
    private static readonly StringHash32 RootChain_Cargo = "Cargo";
    private static readonly StringHash32 RootChain_Cause = "Cause";
    private static readonly StringHash32 RootChain_Artifact = "Artifact";

    #endregion // Consts

    #region Firebase JS Functions

    [DllImport("__Internal")]
    public static extern void FBMissionStart(string missionId);
    [DllImport("__Internal")]
    public static extern void FBMissionComplete(string missionId);
    [DllImport("__Internal")]
    public static extern void FBMissionUnlock(string missionId);
    [DllImport("__Internal")]
    public static extern void FBNewEvidence(string missionId, string evidenceKey);
    [DllImport("__Internal")]
    public static extern void FBOpenEvidenceBoard(string missionId);
    [DllImport("__Internal")]
    public static extern void FBEvidenceChainHint(string missionId, string chainName, string feedbackKey);
    [DllImport("__Internal")]
    public static extern void FBEvidenceChainIncorrect(string missionId, string chainName, string feedbackKey);
    [DllImport("__Internal")]
    public static extern void FBEvidenceChainCorrect(string missionId, string chainName, string feedbackKey);
    [DllImport("__Internal")]
    public static extern void FBUnlockLocation(string missionId);
    [DllImport("__Internal")]
    public static extern void FBEvidenceBoardComplete(string missionId);
    [DllImport("__Internal")]
    public static extern void FBOpenMap(string missionId);
    [DllImport("__Internal")]
    public static extern void FBOpenOffice(string missionId);
    [DllImport("__Internal")]
    public static extern void FBSonarStart(string missionId);
    [DllImport("__Internal")]
    public static extern void FBSonarUpdateProgress(string missionId, string percent);
    [DllImport("__Internal")]
    public static extern void FBSonarComplete(string missionId);
    [DllImport("__Internal")]
    public static extern void FBDiveStart(string missionId);
    [DllImport("__Internal")]
    public static extern void FBDiveExit(string missionId);
    [DllImport("__Internal")]
    public static extern void FBDiveMoveToNode(string missionId, string diveNodeId, string targetNodeId);
    [DllImport("__Internal")]
    public static extern void FBDiveMoveToAscend(string missionId);
    [DllImport("__Internal")]
    public static extern void FBDiveCameraActivate(string missionId, string diveNodeId);
    [DllImport("__Internal")]
    public static extern void FBDiveNewPhoto(string missionId, string diveNodeId);
    [DllImport("__Internal")]
    public static extern void FBDivePhotoFail(string missionId, string diveNodeId);
    [DllImport("__Internal")]
    public static extern void FBDivePhotoAlreadyTaken(string missionId, string diveNodeId);
    [DllImport("__Internal")]
    public static extern void FBDiveNoPhotoAvailable(string missionId, string diveNodeId);
    [DllImport("__Internal")]
    public static extern void FBDiveAllPhotosTaken(string missionId);
    [DllImport("__Internal")]
    public static extern void FBDiveJournalOpen(string missionId);
    [DllImport("__Internal")]
    public static extern void FBViewCutscene(string missionId);
    [DllImport("__Internal")]
    public static extern void FBViewDialog(string missionId, string dialogId);

    #endregion // Firebase JS Functions

    private SimpleLog logger;
    [SerializeField] private string appId = "SHIPWRECKS";
    [SerializeField] private int appVersion = 1;

    [NonSerialized] private string missionId = "";
    [NonSerialized] private string diveNodeId = "";

    private enum eventCategories
    {
        mission_start,
        mission_complete,
        mission_unlock,

        new_evidence,
        open_evidence_board,
        evidence_chain_hint,
        evidence_chain_incorrect,
        evidence_chain_correct,
        unlock_location,
		evidence_board_complete,

        open_office,
        open_map,
        
        sonar_start,
        sonar_percentage_update,
        sonar_complete,

        dive_start,
        dive_exit,
        dive_moveto_location,
        dive_moveto_ascend,
        dive_activate_camera,
        dive_new_photo,
        dive_photo_fail,
        dive_photo_already_taken,
        dive_no_photo_available,
        dive_all_photos_taken,
        dive_journal_open,
        
        view_dialog,
        view_cutscene
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        logger = new SimpleLog(appId, appVersion, null);

        // mission

        GameMgr.Events.Register<int>(GameEvents.LevelStart, LogMissionStart)
            .Register(GameEvents.CaseClosed, LogMissionComplete)
            .Register<int>(GameEvents.LevelUnlocked, LogMissionUnlock);

        // evidence board

        GameMgr.Events.Register<StringHash32>(GameEvents.EvidenceUnlocked, LogEvidenceUnlock)
            .Register<IEvidenceChainState>(GameEvents.ChainHint, LogEvidenceChainHint)
            .Register<IEvidenceChainState>(GameEvents.ChainIncorrect, LogEvidenceChainIncorrect)
            .Register<StringHash32>(GameEvents.ChainSolved, LogEvidenceChainCorrect);
        RegisterGenericLogEvent(GameEvents.BoardOpened, eventCategories.open_evidence_board, FBOpenEvidenceBoard);
        RegisterGenericLogEvent(GameEvents.LocationDiscovered, eventCategories.unlock_location, FBUnlockLocation);
        RegisterGenericLogEvent(GameEvents.BoardComplete, eventCategories.evidence_board_complete, FBEvidenceBoardComplete);

        // map

        RegisterGenericLogEvent(GameEvents.MapOpened, eventCategories.open_map, FBOpenMap);
        RegisterGenericLogEvent(GameEvents.OfficeOpened, eventCategories.open_office, FBOpenOffice);

        // sonar

        RegisterGenericLogEvent(GameEvents.SonarStarted, eventCategories.sonar_start, FBSonarStart);
        RegisterGenericLogEvent(GameEvents.SonarCompleted, eventCategories.sonar_complete, FBSonarComplete);
        GameMgr.Events.Register<float>(GameEvents.SonarProgressUpdated, LogSonarProgress);

        // dive

        RegisterGenericLogEvent(GameEvents.Dive.EnterDive, eventCategories.dive_start, FBDiveStart, () => diveNodeId = string.Empty);
        RegisterGenericLogEvent(GameEvents.Dive.ExitDive, eventCategories.dive_exit, FBDiveExit);
        RegisterDiveArgLogEvent<string>(GameEvents.Dive.NavigateToNode, eventCategories.dive_moveto_location, "next_node_id", FBDiveMoveToNode, (id) => diveNodeId = id);
        RegisterGenericLogEvent(GameEvents.Dive.NavigateToAscendNode, eventCategories.dive_moveto_ascend, FBDiveMoveToAscend, () => diveNodeId = string.Empty);
        RegisterDiveSiteLogEvent(GameEvents.Dive.CameraActivated, eventCategories.dive_activate_camera, FBDiveCameraActivate);
        RegisterDiveSiteLogEvent(GameEvents.Dive.ConfirmPhoto, eventCategories.dive_new_photo, FBDiveNewPhoto);
        RegisterDiveSiteLogEvent(GameEvents.Dive.PhotoFail, eventCategories.dive_photo_fail, FBDivePhotoFail);
        RegisterDiveSiteLogEvent(GameEvents.Dive.PhotoAlreadyTaken, eventCategories.dive_photo_already_taken, FBDivePhotoAlreadyTaken);
        RegisterDiveSiteLogEvent(GameEvents.Dive.NoPhotoAvailable, eventCategories.dive_no_photo_available, FBDiveNoPhotoAvailable);
        RegisterGenericLogEvent(GameEvents.Dive.AllPhotosTaken, eventCategories.dive_all_photos_taken, FBDiveAllPhotosTaken);
        RegisterGenericLogEvent(GameEvents.Dive.RequestPhotoList, eventCategories.dive_journal_open, FBDiveJournalOpen);

        // ui

        GameMgr.Events.Register<ScriptNode>(GameEvents.DialogRun, LogDialog);
        RegisterGenericLogEvent(GameEvents.ViewCutscene, eventCategories.view_cutscene, FBViewCutscene);
    }

    private void Start() {
        LogMissionStart(GameMgr.State.CurrentLevel.Index);
    }

    #region Missions

    private void LogMissionStart(int index) {
        missionId = GameDb.GetLevelData(index).name;

        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.mission_start));

        #if FIREBASE
        FBMissionStart(missionId);
        #endif
    }

    private void LogMissionComplete() {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId }
        };

        logger.Log(new LogEvent(data, eventCategories.mission_complete));

        #if FIREBASE
        FBMissionComplete(missionId);
        #endif

        missionId = "";
    }

    private void LogMissionUnlock(int index) {
        string unlock = GameDb.GetLevelData(index).name;
        
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", unlock }
        };

        logger.Log(new LogEvent(data, eventCategories.mission_unlock));

        #if FIREBASE
        FBMissionUnlock(unlock);
        #endif
    }

    #endregion // Missions

    #region Evidence

    private void LogEvidenceUnlock(StringHash32 id) {
        var evidenceData = GameDb.GetEvidenceData(id);
        string unlock = evidenceData.name;
        
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "evidence_key", unlock }
        };

        logger.Log(new LogEvent(data, eventCategories.new_evidence));

        #if FIREBASE
        FBNewEvidence(missionId, unlock);
        #endif
    }

    private void LogEvidenceChainHint(IEvidenceChainState chainState) {
        string rootName = GetChainRootName(chainState.Root());

        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "chain_id", rootName },
            { "feedback_id", chainState.StickyInfo.Name }
        };

        logger.Log(new LogEvent(data, eventCategories.evidence_chain_hint));

        #if FIREBASE
        FBEvidenceChainHint(missionId, rootName, chainState.StickyInfo.Name);
        #endif
    }

    private void LogEvidenceChainIncorrect(IEvidenceChainState chainState) {
        string rootName = GetChainRootName(chainState.Root());

        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "chain_id", rootName },
            { "feedback_id", chainState.StickyInfo.Name }
        };

        logger.Log(new LogEvent(data, eventCategories.evidence_chain_incorrect));

        #if FIREBASE
        FBEvidenceChainIncorrect(missionId, rootName, chainState.StickyInfo.Name);
        #endif
    }

    private void LogEvidenceChainCorrect(StringHash32 rootId) {
        var chainState = GameMgr.State.CurrentLevel.GetChain(rootId);
        string rootName = GetChainRootName(chainState.Root());

        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "chain_id", rootName },
            { "feedback_id", chainState.StickyInfo.Name }
        };

        logger.Log(new LogEvent(data, eventCategories.evidence_chain_correct));

        #if FIREBASE
        FBEvidenceChainCorrect(missionId, rootName, chainState.StickyInfo.Name);
        #endif
    }

    #endregion // Evidence

    #region Sonar

    private void LogSonarProgress(float updateProgress) {
        string percent = ((int) (updateProgress * 100)).ToStringLookup();
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "progress", percent }
        };

        logger.Log(new LogEvent(data, eventCategories.sonar_percentage_update));

        #if FIREBASE
        FBSonarUpdateProgress(missionId, percent);
        #endif
    }

    #endregion // Sonar

    #region Dialog

    private void LogDialog(ScriptNode node) {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            { "mission_id", missionId },
            { "dialog_id", node.FullName }
        };

        logger.Log(new LogEvent(data, eventCategories.view_dialog));

        #if FIREBASE
        FBViewDialog(missionId, node.FullName);
        #endif
    }

    #endregion // Dialog

    #region Helpers

    private void RegisterGenericLogEvent(StringHash32 eventId, eventCategories category, Action<string> native, Action stateMod = null) {
        GameMgr.Events.Register(eventId, () => {
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "mission_id", missionId }
            };

            logger.Log(new LogEvent(data, category));

            #if FIREBASE
            native(missionId);
            #endif

            if (stateMod != null) {
                stateMod();
            }
        });
    }

    private void RegisterArgLogEvent<T>(StringHash32 eventId, eventCategories category, string fieldName, Action<string, string> native, Action<T> stateMod = null) {
        GameMgr.Events.Register<T>(eventId, (arg) => {
            string argAsString = arg == null ? "" : arg.ToString();

            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "mission_id", missionId },
                { fieldName, argAsString }
            };

            logger.Log(new LogEvent(data, category));

            #if FIREBASE
            native(missionId, argAsString);
            #endif

            if (stateMod != null) {
                stateMod(arg);
            }
        });
    }

    private void RegisterDiveSiteLogEvent(StringHash32 eventId, eventCategories category, Action<string, string> native, Action stateMod = null) {
        GameMgr.Events.Register(eventId, () => {
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "mission_id", missionId },
                { "dive_node_id", diveNodeId }
            };

            logger.Log(new LogEvent(data, category));

            #if FIREBASE
            native(missionId, diveNodeId);
            #endif

            if (stateMod != null) {
                stateMod();
            }
        });
    }

    private void RegisterDiveArgLogEvent<T>(StringHash32 eventId, eventCategories category, string fieldName, Action<string, string, string> native, Action<T> stateMod = null) {
        GameMgr.Events.Register<T>(eventId, (arg) => {
            string argAsString = arg == null ? "" : arg.ToString();

            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "mission_id", missionId },
                { "dive_node_id", diveNodeId },
                { fieldName, argAsString }
            };

            logger.Log(new LogEvent(data, category));

            #if FIREBASE
            native(missionId, diveNodeId, argAsString);
            #endif

            if (stateMod != null) {
                stateMod(arg);
            }
        });
    }

    static private string GetChainRootName(StringHash32 rootId) {
        if (rootId == RootChain_Location) {
            return "Location";
        } else if (rootId == RootChain_Name) {
            return "Name";
        } else if (rootId == RootChain_Artifact) {
            return "Artifact";
        } else if (rootId == RootChain_Cargo) {
            return "Cargo";
        } else if (rootId == RootChain_Cause) {
            return "Cause";
        } else if (rootId == RootChain_Type) {
            return "Type";
        } else {
            return string.Empty;
        }
    }

    #endregion // Helpers
}
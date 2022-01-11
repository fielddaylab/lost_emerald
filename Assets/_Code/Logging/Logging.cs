#if !UNITY_EDITOR
#define FIREBASE
#endif // !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using BeauUtil;
using FieldDay;
using PotatoLocalization;
using Shipwreck;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	public static extern void FBSceneLoad(string missionId, string scene, string timestamp);
	[DllImport("__Internal")]
	public static extern void FBCheckpoint(string missionId, string status);
	[DllImport("__Internal")]
    public static extern void FBNewEvidence(string missionId, string actor, string evidenceId);
    [DllImport("__Internal")]
    public static extern void FBOpenEvidenceBoard(string missionId);
	[DllImport("__Internal")]
	public static extern void FBEvidenceBoardClick(string missionId, string evidenceType, string factOrigin, string factTarget, string accurate);
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
	public static extern void FBDivePhotoClick(string missionId, string diveNodeId, string accurate);
    [DllImport("__Internal")]
    public static extern void FBDiveAllPhotosTaken(string missionId);
	[DllImport("__Internal")]
	public static extern void FBDiveJournalClick(string missionId, string tasks, string clickAction, string actor);
	[DllImport("__Internal")]
	public static extern void FBConversationClick(string missionId, string scene, string clickType, string character, string clickAction);
	[DllImport("__Internal")]
    public static extern void FBViewCutscene(string missionId);
    [DllImport("__Internal")]
    public static extern void FBViewDialog(string missionId, string dialogId);
	[DllImport("__Internal")]
	public static extern void FBCloseInspect(string missionId, string scene, string itemId);

	#endregion // Firebase JS Functions

	private SimpleLog logger;
    [SerializeField] private string appId = "SHIPWRECKS";
    [SerializeField] private int appVersion = 1;

    [NonSerialized] private string missionId = "";
    [NonSerialized] private string diveNodeId = "";
	[NonSerialized] private string diveTasksStr = "";
	[NonSerialized] private string diveJournalActor = "";
	[NonSerialized] private string nodeContact = "";
	[NonSerialized] private string evidenceActor = "";
	[NonSerialized] private DateTime startDT = DateTime.Now;

	private enum eventCategories
    {
		scene_load,

		checkpoint,

        new_evidence,

		open_evidence_board,
		evidence_board_click,
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
		dive_photo_click,
        dive_all_photos_taken,
		dive_journal_click,

		conversation_click,

		view_cutscene,
		view_dialog,

		close_inspect
	}

	#region EventData

	public struct EventData {
		public enum Status {
			BeginMission,
			DiveComplete,
			EvidenceBoardComplete,
			CaseClosed
		}
		public static Dictionary<Status, string> StatusDict = new Dictionary<Status, string> {
			{ Status.BeginMission, "Begin Mission" },
			{ Status.DiveComplete, "Dive Complete" },
			{ Status.EvidenceBoardComplete, "Evidence Board Complete" },
			{ Status.CaseClosed, "Case Closed" }
		};

		public enum Actor {
			Game,
			Player
		}
		public static Dictionary<Actor, string> ActorDict = new Dictionary<Actor, string> {
			{ Actor.Game, "Game" },
			{ Actor.Player, "Player" }
		};

		public enum ClickType {
			Phone,
			Radio,
			Text,
			Evidence,
			Inspect,
			Menu,
			Cutscene,
			Unknown
		}
		public static Dictionary<ClickType, string> ClickTypeDict = new Dictionary<ClickType, string> {
			{ ClickType.Phone, "Phone" },
			{ ClickType.Radio, "Radio" },
			{ ClickType.Text, "Text" },
			{ ClickType.Evidence, "Evidence" },
			{ ClickType.Inspect, "Inspect" },
			{ ClickType.Menu, "Menu" },
			{ ClickType.Cutscene, "Cutscene" },
			{ ClickType.Unknown, "Unknown" }
		};

		public enum ClickAction {
			Continue,
			Open,
			Close
		}
		public static Dictionary<ClickAction, string> ClickActionDict = new Dictionary<ClickAction, string> {
			{ ClickAction.Continue, "Continue" },
			{ ClickAction.Open, "Open" },
			{ ClickAction.Close, "Close" }
		};
	};


	#endregion

	void OnEnable()
    {
        DontDestroyOnLoad(this.gameObject);
        logger = new SimpleLog(appId, appVersion, null);

		// scenes

		GameMgr.Events.Register<int>(GameEvents.SceneLoaded, LogSceneLoad);

		// checkpoint

		GameMgr.Events.Register<int>(GameEvents.LevelStart, LogBeginMission);
		GameMgr.Events.Register(GameEvents.CaseClosed, LogCaseClosed);

		// evidence board

		GameMgr.Events.Register(GameEvents.GameUnlockingEvidence, HandleGameUnlockingEvidence);
		GameMgr.Events.Register<StringHash32>(GameEvents.EvidenceUnlocked, LogEvidenceUnlock)
            .Register<IEvidenceChainState>(GameEvents.ChainHint, LogEvidenceChainHint)
            .Register<IEvidenceChainState>(GameEvents.ChainIncorrect, LogEvidenceChainIncorrect)
            .Register<StringHash32>(GameEvents.ChainSolved, LogEvidenceChainCorrect);
        RegisterGenericLogEvent(GameEvents.BoardOpened, eventCategories.open_evidence_board, FBOpenEvidenceBoard);
        RegisterGenericLogEvent(GameEvents.LocationDiscovered, eventCategories.unlock_location, FBUnlockLocation);
		GameMgr.Events.Register(GameEvents.BoardComplete, LogEvidenceBoardComplete);

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
		GameMgr.Events.Register<StringHash32>(GameEvents.Dive.ConfirmPhoto, LogDiveNewPhoto);
		GameMgr.Events.Register(GameEvents.Dive.PhotoFail, LogDivePhotoFail);
		GameMgr.Events.Register(GameEvents.Dive.PhotoAlreadyTaken, LogDivePhotoAlreadyTaken);
		GameMgr.Events.Register(GameEvents.Dive.NoPhotoAvailable, LogDiveNoPhotoAvailable);
		GameMgr.Events.Register(GameEvents.Dive.AllPhotosTaken, LogDiveComplete);
		GameMgr.Events.Register<EventData.Actor>(GameEvents.Dive.JournalOpened, HandleJournalOpened);
		GameMgr.Events.Register<List<DivePointOfInterest>>(GameEvents.Dive.SendPhotoList, HandlePhotoListSent);
		GameMgr.Events.Register(GameEvents.Dive.PhotoListSent, LogDiveJournalOpen);
		GameMgr.Events.Register(GameEvents.Dive.CloseJournal, LogDiveJournalClose);

		// ui

		GameMgr.Events.Register<ScriptNode>(GameEvents.ConversationOpened, HandleConversationOpen);
		GameMgr.Events.Register<EventData.ClickAction>(GameEvents.ConversationClick, LogConversationClick);
		GameMgr.Events.Register<ScriptNode>(GameEvents.DialogRun, LogDialog);
        RegisterGenericLogEvent(GameEvents.ViewCutscene, eventCategories.view_cutscene, FBViewCutscene);
		GameMgr.Events.Register<string>(GameEvents.CloseInspect, LogCloseInspect);
	}

    private void Start() {
		GameMgr.Events.Dispatch(GameEvents.SceneLoaded, GameMgr.State.CurrentLevel.Index);
	}

	#region Scenes

	// Mission, Scene, Timestamp
	private void LogSceneLoad(int index) {
		missionId = GameDb.GetLevelData(index).name;

		string scene = SceneManager.GetActiveScene().name;

		long elapsedTicks = DateTime.Now.Ticks - startDT.Ticks;
		TimeSpan elapsedTime = new TimeSpan(elapsedTicks);

		string timestamp = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
			elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds,
			elapsedTime.Milliseconds / 10);

		Dictionary<string, string> data = new Dictionary<string, string>() {
			{ "mission_id", missionId },
			{ "scene", scene },
			{ "timestamp", timestamp }
		};

		#if FIREBASE
		FBSceneLoad(missionId, scene, timestamp);
		#endif
	}

	#endregion

	#region Checkpoints

	// Mission, Status
	private void LogCheckpoint(int index, EventData.Status status) {
		missionId = GameDb.GetLevelData(index).name;
		string statusStr = EventData.StatusDict[status];

		Dictionary<string, string> data = new Dictionary<string, string>()
		{
			{ "mission_id", missionId },
			{ "status",  statusStr }
		};

		logger.Log(new LogEvent(data, eventCategories.checkpoint));

#if FIREBASE
        FBCheckpoint(missionId, statusStr);
#endif
	}

	private void LogBeginMission(int missionId) {
		LogCheckpoint(missionId, EventData.Status.BeginMission);
	}

	private void LogDiveComplete() {
		LogCheckpoint(GameMgr.State.CurrentLevel.Index, EventData.Status.DiveComplete);
	}

	private void LogEvidenceBoardComplete() {
		LogCheckpoint(GameMgr.State.CurrentLevel.Index, EventData.Status.EvidenceBoardComplete);
	}

	private void LogCaseClosed() {
		LogCheckpoint(GameMgr.State.CurrentLevel.Index, EventData.Status.CaseClosed);
	}

	#endregion

	#region Evidence

	// Mission, Fact Type, Fact Origin, Fact Target, Accurate
	private void LogEvidenceBoardClick(IEvidenceChainState chainState, bool isAccurate) {
		string factType = GetChainRootName(chainState.Root());

		string factOriginStr = "origins: ";
		string factTargetStr = "targets: ";

		ListSlice<StringHash32> roots = chainState.StickyInfo.RootIds;
		foreach(StringHash32 root in roots) {
			factOriginStr += root.ToDebugString() + " | ";
		}

		ListSlice<StringHash32> nodes = chainState.StickyInfo.NodeIds;
		foreach (StringHash32 node in nodes) {
			factTargetStr += node.ToDebugString() + " | ";
		}

		string accurate = isAccurate.ToString();

		Dictionary<string, string> data = new Dictionary<string, string>()
		{
			{ "mission_id", missionId },
			{ "fact_type", factType },
			{ "fact_origin", factOriginStr },
			{ "fact_target", factTargetStr },
			{ "accurate", accurate }
		};

		logger.Log(new LogEvent(data, eventCategories.evidence_board_click));

#if FIREBASE
        FBEvidenceBoardClick(missionId, factType, factOriginStr, factTargetStr, accurate);
#endif
	}
	
    private void LogEvidenceChainHint(IEvidenceChainState chainState) {
		LogEvidenceBoardClick(chainState, true);
    }
	
    private void LogEvidenceChainIncorrect(IEvidenceChainState chainState) {
		LogEvidenceBoardClick(chainState, false);
    }

    private void LogEvidenceChainCorrect(StringHash32 rootId) {
        var chainState = GameMgr.State.CurrentLevel.GetChain(rootId);
		LogEvidenceBoardClick(chainState, true);
	}

	// Mission, Actor, EvidenceID
	private void LogEvidenceUnlock(StringHash32 id) {
		var evidenceData = GameDb.GetEvidenceData(id);
		string evidenceId = evidenceData.name;

		Dictionary<string, string> data = new Dictionary<string, string>()
		{
			{ "mission_id", missionId },
			{ "actor", evidenceActor },
			{ "evidence_id", evidenceId }
		};

		logger.Log(new LogEvent(data, eventCategories.new_evidence));

#if FIREBASE
        FBNewEvidence(missionId, evidenceActor, evidenceId);
#endif
	}

	#endregion // Evidence

	#region Dive

	// Mission, Location, Accurate
	private void LogDivePhotoClick(bool isAccurate) {
		if (isAccurate) { evidenceActor = EventData.ActorDict[EventData.Actor.Player]; }

		string accurate = isAccurate.ToString();

		string location = diveNodeId;

		Dictionary<string, string> data = new Dictionary<string, string>()
		{
			{ "mission_id", missionId },
			{ "location", location },
			{ "accurate", accurate }
		};

		logger.Log(new LogEvent(data, eventCategories.dive_photo_click));

#if FIREBASE
        FBDivePhotoClick(missionId, location, accurate);
#endif
	}

	private void LogDiveNewPhoto(StringHash32 photoId) {
		LogDivePhotoClick(true);
	}

	private void LogDivePhotoFail() {
		LogDivePhotoClick(false);
	}

	private void LogDivePhotoAlreadyTaken() {
		LogDivePhotoClick(false);
	}

	private void LogDiveNoPhotoAvailable() {
		LogDivePhotoClick(false);
	}

	#endregion // Dive

	#region Journal

	// Mission, Tasks, Action, Actor
	private void LogDiveJournalClick(string clickAction, string actor) {
		Dictionary<string, string> data = new Dictionary<string, string>()
		{
			{ "mission_id", missionId },
			{ "tasks", diveTasksStr },
			{ "click_action", clickAction },
			{ "actor", actor }
		};

		logger.Log(new LogEvent(data, eventCategories.dive_journal_click));

#if FIREBASE
        FBDiveJournalClick(missionId, diveTasksStr, clickAction, actor);
#endif
	}

	private void LogDiveJournalOpen() {
		LogDiveJournalClick(EventData.ClickActionDict[EventData.ClickAction.Open], diveJournalActor);
	}

	private void LogDiveJournalClose() {
		diveJournalActor = EventData.ActorDict[EventData.Actor.Player]; // only player can close the journal
		LogDiveJournalClick(EventData.ClickActionDict[EventData.ClickAction.Close], diveJournalActor);
	}

	#endregion // Journal

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

	// Conversation Click -- Mission, Scene, Click-Type, Character, Click-Action
	private void LogConversationClick(EventData.ClickAction clickAction) {
		EventData.ClickType clickType = DetermineClickType();
		string clickTypeStr = EventData.ClickTypeDict[clickType];

		string clickActionStr = EventData.ClickActionDict[clickAction];

		string scene = SceneManager.GetActiveScene().name;

		Dictionary<string, string> data = new Dictionary<string, string>()
		{
			{ "mission_id", missionId },
			{ "scene", scene },
			{ "click_type", clickTypeStr },
			{ "character", nodeContact },
			{ "click_action", clickActionStr }
		};

		logger.Log(new LogEvent(data, eventCategories.conversation_click));

#if FIREBASE
        FBConversationClick(missionId, scene, clickTypeStr, nodeContact, clickActionStr);
#endif
	}

	private void HandleConversationOpen(ScriptNode node) {
		nodeContact = GameDb.GetCharacterData(node.ContactId).name;
	}

	private void LogDialog(ScriptNode node) {
		string scene = SceneManager.GetActiveScene().name;

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

	#region Close Inspect

	// Mission, Scene, ItemID
	private void LogCloseInspect(string itemId) {
		string scene = SceneManager.GetActiveScene().name;

		Dictionary<string, string> data = new Dictionary<string, string>()
		{
			{ "mission_id", missionId },
			{ "scene", scene },
			{ "item_id", itemId }
		};

		logger.Log(new LogEvent(data, eventCategories.close_inspect));

#if FIREBASE
        FBCloseInspect(missionId, scene, itemId);
#endif
	}

	#endregion // Close Inspect

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

	private struct KeyValuePair {
		public LocalizationKey Key;
		public bool Value;

		public KeyValuePair(LocalizationKey key, bool value) {
			Key = key;
			Value = value;
		}
	}

	private void HandlePhotoListSent(List<DivePointOfInterest> divePoints) {

		List<KeyValuePair> tasks = new List<KeyValuePair>();
		List<LocalizationKey> taskLocalizations = new List<LocalizationKey>();

		foreach (DivePointOfInterest poi in divePoints) {
			if (taskLocalizations.Contains(poi.PhotoName)) { continue; }
			bool isChecked = GameMgr.State.CurrentLevel.IsEvidenceUnlocked(poi.EvidenceUnlock);
			tasks.Add(new KeyValuePair(poi.PhotoName, isChecked));
			taskLocalizations.Add(poi.PhotoName);
		}

		foreach (KeyValuePair pair in tasks) {
			diveTasksStr += pair.Key + ": " + pair.Value + "\n";
		}
	}

	private void HandleJournalOpened(EventData.Actor actor) {
		Debug.Log("handling journal open: " + actor);
		diveJournalActor = EventData.ActorDict[actor];
	}

	private EventData.ClickType DetermineClickType() {
		if (UIMgr.IsOpen<UIDialogScreen>()) {
			return EventData.ClickType.Phone;
		}
		if (UIMgr.IsOpen<UIRadioDialog>()) {
			return EventData.ClickType.Radio;
		}
		if (UIMgr.IsOpen<UITextMessage>()) {
			return EventData.ClickType.Text;
		}
		if (UIMgr.IsOpen<UICloseInspect>()) {
			return EventData.ClickType.Inspect;
		}
		if (UIMgr.IsOpen<UIEvidenceScreen>()) {
			return EventData.ClickType.Evidence;
		}
		if (UIMgr.IsOpen<UITitleScreen>()) {
			return EventData.ClickType.Menu;
		}
		if (CutscenePlayer.IsPlaying) {
			return EventData.ClickType.Cutscene;
		}
		else {
			return EventData.ClickType.Unknown;
		}
	}

	private void HandleGameUnlockingEvidence() {
		evidenceActor = EventData.ActorDict[EventData.Actor.Game];
	}

    #endregion // Helpers
}
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
	public static extern void FBSceneLoad(string appVersion, int logVersion, string userCode, string missionId, string scene, string timestamp);
	[DllImport("__Internal")]
	public static extern void FBCheckpoint(string appVersion, int logVersion, string userCode, string missionId, string status);
	[DllImport("__Internal")]
    public static extern void FBNewEvidence(string appVersion, int logVersion, string userCode, string missionId, string actor, string evidenceId);
    [DllImport("__Internal")]
    public static extern void FBOpenEvidenceBoard(string appVersion, int logVersion, string userCode, string missionId);
	[DllImport("__Internal")]
	public static extern void FBEvidenceBoardClick(string appVersion, int logVersion, string userCode, string missionId, string evidenceType, string factOrigin, string factTarget, string accurate);
	[DllImport("__Internal")]
    public static extern void FBUnlockLocation(string appVersion, int logVersion, string userCode, string missionId);
    [DllImport("__Internal")]
    public static extern void FBEvidenceBoardComplete(string appVersion, int logVersion, string userCode, string missionId);
    [DllImport("__Internal")]
    public static extern void FBOpenMap(string appVersion, int logVersion, string userCode, string missionId);
    [DllImport("__Internal")]
    public static extern void FBOpenOffice(string appVersion, int logVersion, string userCode, string missionId);
    [DllImport("__Internal")]
    public static extern void FBSonarStart(string appVersion, int logVersion, string userCode, string missionId);
    [DllImport("__Internal")]
    public static extern void FBSonarUpdateProgress(string appVersion, int logVersion, string userCode, string missionId, string percent);
    [DllImport("__Internal")]
    public static extern void FBSonarComplete(string appVersion, int logVersion, string userCode, string missionId);
    [DllImport("__Internal")]
    public static extern void FBDiveStart(string appVersion, int logVersion, string userCode, string missionId);
    [DllImport("__Internal")]
    public static extern void FBDiveExit(string appVersion, int logVersion, string userCode, string missionId);
    [DllImport("__Internal")]
    public static extern void FBDiveMoveToNode(string appVersion, int logVersion, string userCode, string missionId, string diveNodeId, string targetNodeId);
    [DllImport("__Internal")]
    public static extern void FBDiveMoveToAscend(string appVersion, int logVersion, string userCode, string missionId);
    [DllImport("__Internal")]
    public static extern void FBDiveCameraActivate(string appVersion, int logVersion, string userCode, string missionId, string diveNodeId);
	[DllImport("__Internal")]
	public static extern void FBDivePhotoClick(string appVersion, int logVersion, string userCode, string missionId, string diveNodeId, string accurate);
    [DllImport("__Internal")]
    public static extern void FBDiveAllPhotosTaken(string appVersion, int logVersion, string userCode, string missionId);
	[DllImport("__Internal")]
	public static extern void FBDiveJournalClick(string appVersion, int logVersion, string userCode, string missionId, string tasks, string clickAction, string actor);
	[DllImport("__Internal")]
	public static extern void FBConversationClick(string appVersion, int logVersion, string userCode, string missionId, string scene, string clickType, string character, string clickAction);
	[DllImport("__Internal")]
    public static extern void FBViewCutscene(string appVersion, int logVersion, string userCode, string missionId);
    [DllImport("__Internal")]
    public static extern void FBViewDialog(string appVersion, int logVersion, string userCode, string missionId, string dialogId);
	[DllImport("__Internal")]
	public static extern void FBCloseInspect(string appVersion, int logVersion, string userCode, string missionId, string scene, string itemId);

	#endregion // Firebase JS Functions

    [SerializeField] private string appId = "SHIPWRECKS";
    [SerializeField] private string appVersion = "1.0";
	[SerializeField] private int logVersion = 1;
	[SerializeField] private string userCode = "default";
	[SerializeField] private LocalizationMap localizationMap = null;

    [NonSerialized] private string missionId = "";
    [NonSerialized] private string diveNodeId = "";
	[NonSerialized] private string diveTasksStr = "";
	[NonSerialized] private string diveJournalActor = "";
	[NonSerialized] private string nodeContact = "";
	[NonSerialized] private string evidenceActor = "";
	[NonSerialized] private DateTime startDT = DateTime.Now;
	[NonSerialized] private EventData.ClickType currClickType = EventData.ClickType.Unknown;

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

		// scenes

		GameMgr.Events.Register<string>(GameEvents.SceneLoaded, LogSceneLoad);

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
		GameMgr.Events.Dispatch(GameEvents.SceneLoaded, "Main");
	}

	#region Scenes

	// Mission, Scene, Timestamp
	private void LogSceneLoad(string sceneId) {
		int index = GameMgr.State.CurrentLevel.Index;
		missionId = GameDb.GetLevelData(index).name;

		string scene = sceneId;

		long elapsedTicks = DateTime.Now.Ticks - startDT.Ticks;
		TimeSpan elapsedTime = new TimeSpan(elapsedTicks);

		string timestamp = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
			elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds,
			elapsedTime.Milliseconds / 10);

		#if FIREBASE
		FBSceneLoad(appVersion, logVersion, userCode, missionId, scene, timestamp);
		#endif
	}

	#endregion

	#region Checkpoints

	// Mission, Status
	private void LogCheckpoint(int index, EventData.Status status) {
		missionId = GameDb.GetLevelData(index).name;
		string statusStr = EventData.StatusDict[status];

		#if FIREBASE
        FBCheckpoint(appVersion, logVersion, userCode, missionId, statusStr);
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

		string factOriginStr = "origin: ";
		string factTargetStr = "targets: ";

		ListSlice<StringHash32> roots = chainState.StickyInfo.RootIds;
		int index = 0;
		foreach(StringHash32 root in roots) {
			if (index > 0) { factOriginStr += "; "; }
			factOriginStr += localizationMap.GetText(GameDb.GetNodeLocalizationKey(root));
			index++;
		}

		index = 0;
		ListSlice<StringHash32> nodes = chainState.Chain();
		foreach (StringHash32 node in nodes) {
			if (index > 0) { factTargetStr += ", "; }
			factTargetStr += node;
			index++;
		}

		string accurate = isAccurate.ToString();

		#if FIREBASE
        FBEvidenceBoardClick(appVersion, logVersion, userCode, missionId, factType, factOriginStr, factTargetStr, accurate);
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

		#if FIREBASE
        FBNewEvidence(appVersion, logVersion, userCode, missionId, evidenceActor, evidenceId);
		#endif
	}

	#endregion // Evidence

	#region Dive

	// Mission, Location, Accurate
	private void LogDivePhotoClick(bool isAccurate) {
		if (isAccurate) { evidenceActor = EventData.ActorDict[EventData.Actor.Player]; }

		string accurate = isAccurate.ToString();
		string location = diveNodeId;

		#if FIREBASE
        FBDivePhotoClick(appVersion, logVersion, userCode, missionId, location, accurate);
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
		#if FIREBASE
        FBDiveJournalClick(appVersion, logVersion, userCode, missionId, diveTasksStr, clickAction, actor);
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

        #if FIREBASE
        FBSonarUpdateProgress(appVersion, logVersion, userCode, missionId, percent);
        #endif
    }

	#endregion // Sonar

	#region Dialog

	// Conversation Click -- Mission, Scene, Click-Type, Character, Click-Action
	private void LogConversationClick(EventData.ClickAction clickAction) {
		EventData.ClickType clickType;
		if (currClickType == EventData.ClickType.Unknown) {
			clickType = DetermineClickType();
		}
		else {
			clickType = currClickType;
		}

		string clickTypeStr = EventData.ClickTypeDict[clickType];

		string clickActionStr = EventData.ClickActionDict[clickAction];
		if (clickAction == EventData.ClickAction.Close) {
			currClickType = EventData.ClickType.Unknown;
		}

		string scene = SceneManager.GetActiveScene().name;

		#if FIREBASE
        FBConversationClick(appVersion, logVersion, userCode, missionId, scene, clickTypeStr, nodeContact, clickActionStr);
		#endif
	}

	private void HandleConversationOpen(ScriptNode node) {
		nodeContact = GameDb.GetCharacterData(node.ContactId).name;
	}

	private void LogDialog(ScriptNode node) {
		string scene = SceneManager.GetActiveScene().name;

        #if FIREBASE
        FBViewDialog(appVersion, logVersion, userCode, missionId, node.FullName);
        #endif
    }

	#endregion // Dialog

	#region Close Inspect

	// Mission, Scene, ItemID
	private void LogCloseInspect(string itemId) {
		string scene = SceneManager.GetActiveScene().name;

		#if FIREBASE
        FBCloseInspect(appVersion, logVersion, userCode, missionId, scene, itemId);
		#endif
	}

	#endregion // Close Inspect

	#region Helpers

	private void RegisterGenericLogEvent(StringHash32 eventId, eventCategories category, Action<string, int, string, string> native, Action stateMod = null) {
        GameMgr.Events.Register(eventId, () => {

            #if FIREBASE
            native(appVersion, logVersion, userCode, missionId);
            #endif

            if (stateMod != null) {
                stateMod();
            }
        });
    }

    private void RegisterArgLogEvent<T>(StringHash32 eventId, eventCategories category, string fieldName, Action<string, int, string, string, string> native, Action<T> stateMod = null) {
        GameMgr.Events.Register<T>(eventId, (arg) => {
            string argAsString = arg == null ? "" : arg.ToString();

            #if FIREBASE
            native(appVersion, logVersion, userCode, missionId, argAsString);
            #endif

            if (stateMod != null) {
                stateMod(arg);
            }
        });
    }

    private void RegisterDiveSiteLogEvent(StringHash32 eventId, eventCategories category, Action<string, int, string, string, string> native, Action stateMod = null) {
        GameMgr.Events.Register(eventId, () => {
            #if FIREBASE
            native(appVersion, logVersion, userCode, missionId, diveNodeId);
            #endif

            if (stateMod != null) {
                stateMod();
            }
        });
    }

    private void RegisterDiveArgLogEvent<T>(StringHash32 eventId, eventCategories category, string fieldName, Action<string, int, string, string, string, string> native, Action<T> stateMod = null) {
        GameMgr.Events.Register<T>(eventId, (arg) => {
            string argAsString = arg == null ? "" : arg.ToString();

            #if FIREBASE
            native(appVersion, logVersion, userCode, missionId, diveNodeId, argAsString);
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

		diveTasksStr = "";
		foreach (KeyValuePair pair in tasks) {
			diveTasksStr += localizationMap.GetText(pair.Key) + ": " + pair.Value + "; ";
		}
	}

	private void HandleJournalOpened(EventData.Actor actor) {
		diveJournalActor = EventData.ActorDict[actor];
	}

	private EventData.ClickType DetermineClickType() {
		EventData.ClickType type = EventData.ClickType.Unknown;

		if (UIMgr.IsOpen<UIDialogScreen>()) {
			type = EventData.ClickType.Phone;
		}
		else if (UIMgr.IsOpen<UIRadioDialog>()) {
			type = EventData.ClickType.Radio;
		}
		else if (UIMgr.IsOpen<UIPhone>()) {
			type = EventData.ClickType.Text;
		}
		else if (UIMgr.IsOpen<UICloseInspect>()) {
			type = EventData.ClickType.Inspect;
		}
		else if (UIMgr.IsOpen<UIEvidenceScreen>()) {
			type = EventData.ClickType.Evidence;
		}
		else if (UIMgr.IsOpen<UITitleScreen>()) {
			type = EventData.ClickType.Menu;
		}
		else if (CutscenePlayer.IsPlaying) {
			type = EventData.ClickType.Cutscene;
		}

		if (type != EventData.ClickType.Unknown) {
			currClickType = type;
		}

		return type;
	}

	private void HandleGameUnlockingEvidence() {
		evidenceActor = EventData.ActorDict[EventData.Actor.Game];
	}

    #endregion // Helpers
}
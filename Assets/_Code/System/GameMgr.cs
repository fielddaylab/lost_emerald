﻿using BeauPools;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using BeauUtil.Variants;
using Leaf;
using Leaf.Runtime;
using System;
using UnityEngine;

namespace Shipwreck
{

	public sealed partial class GameMgr : Singleton<GameMgr> {

		public static IGameState State {
			get { return I.m_state; }
		}

		public static EventService Events {
			get { return I.m_eventService; }
		}

		private ScriptMgr m_scriptMgr;
		private StickyEvaluator m_stickyEvaluator;
		private GameState m_state;
		private EventService m_eventService;

		private int m_selectedLevel = -1;

		[SerializeField]
		private LeafAsset[] m_levelScripts;
		[SerializeField]
		private StickyAsset[] m_levelStickNotes;

		protected override void OnAssigned() {
			Routine.Settings.DebugMode = false;

			m_state = new GameState();

			m_scriptMgr = new ScriptMgr(this);
			m_scriptMgr.LoadGameState(m_state, m_state.VariableTable);
			m_scriptMgr.ConfigureEvents();

			m_eventService = new EventService();
			SetLevelIndex(0);
		}

		public static void MarkTitleScreenComplete() {
			I.m_eventService.Register<ScriptNode>(GameEvents.PhoneNotification, I.HandlePhoneNotification);
			I.m_eventService.Register(GameEvents.ChainSolved, I.HandleChainCompleted);
			I.m_eventService.Register(GameEvents.CaseClosed, I.HandleGameEnding);
			CutscenePlayer.OnVideoComplete += I.HandleCutsceneComplete;

            // added for the Vault Button
            Events.Dispatch(GameEvents.ExitTitleScreen);
		}

		public static void ClearState() {
			I.m_state.Clear();
			I.m_eventService.Deregister<ScriptNode>(GameEvents.PhoneNotification, I.HandlePhoneNotification);
			I.m_eventService.Deregister(GameEvents.ChainSolved, I.HandleChainCompleted);
			I.m_eventService.Deregister(GameEvents.CaseClosed, I.HandleGameEnding);
			CutscenePlayer.OnVideoComplete -= I.HandleCutsceneComplete;
		}

		public static void SetChain(int levelIndex, StringHash32 root, params StringHash32[] chain) {
			EvidenceChainState chainState = (EvidenceChainState)I.m_state.GetChain(levelIndex, root);
			chainState.Lift(0);
			foreach (StringHash32 node in chain) {
				chainState.Drop(node);
			}
		}

		public static void SetLevelIndex(int levelIndex) {
			I.SetLevelIndexInternal(levelIndex);
		}

		private void SetLevelIndexInternal(int levelIndex) {
			if (levelIndex < 0 || levelIndex > m_levelScripts.Length) {
				throw new IndexOutOfRangeException();
			}

			if (m_stickyEvaluator == null) {
				m_stickyEvaluator = new StickyEvaluator();
			}
			if (m_selectedLevel != -1) {
				// unload the previous level
				m_stickyEvaluator.Unload(m_levelStickNotes[m_selectedLevel]);
				UnloadScript(m_levelScripts[m_selectedLevel]);
			}

			m_selectedLevel = levelIndex;
			m_state.SetCurrentLevel(levelIndex);

			m_stickyEvaluator.Load(m_levelStickNotes[m_selectedLevel]);
			LoadScript(m_levelScripts[m_selectedLevel]);

			RunTrigger("Start");
			Events.Dispatch(GameEvents.LevelStart, levelIndex);
		}

		private void HandlePhoneNotification(ScriptNode node) {
			// DIVE SCENE WILL WAIT UNTIL MESSAGE STATE
			// to run its messages
			if (!UIMgr.IsOpen<UIDiveScreen>()) {
				// check to see if the message should be
				// opened automatically
				if (node.OpenAutomatically) {
					TryRunLastNotification(out var _);
				} else {
					UIMgr.Open<UIPhoneNotif>();
					UIMgr.Open<UIModalOverlay>();
				}
			}
		}
		private void HandleChainCompleted() {
			// check to see if all chains are solved
			if (m_state.CurrentLevel.IsBoardComplete()) {
				UIMgr.Open<UIModalBoardComplete>();
			}
		}
		private void HandleCutsceneComplete() {
			UIMgr.Open<UIOfficeScreen>();
			UIMgr.Open<UIModalCaseClosed>();
		}
		private void HandleGameEnding() {
			if (m_state.IsBoardComplete(3)) {
				UIMgr.CloseImmediately<UIOfficeScreen>();
				UIMgr.CloseImmediately<UIEvidenceScreen>();
				UIMgr.Open<UIEndingCredits>();
			}
		}

		public void SetCurrentCustomMessage(StringHash32 messageKey) {
			I.m_state.CurrentLevel.SetCurrentMessage(messageKey);
		}


		#region Scripting

		public static bool TryFindNode(LeafAsset asset, string name, out ScriptNode node) {
			LeafNodePackage<ScriptNode> package = I.m_scriptMgr.RegisterPackage(asset);
			StringHash32 fullId = new StringHash32(package.RootPath()).Concat(".").Concat(name);
			return package.TryGetNode(fullId, out node);
		}

		public static LeafThreadHandle RunScriptNode(ScriptNode node) {
			return I.m_scriptMgr.Run(node);
		}

		public static LeafThreadHandle RunTrigger(StringHash32 triggerId, VariantTable table = null, ILeafActor actor = null, StringHash32 target = default(StringHash32)) {
			LeafThreadHandle responseHandle = default(LeafThreadHandle);

			using (PooledList<ScriptNode> nodes = PooledList<ScriptNode>.Create()) {
				I.m_scriptMgr.GetResponsesForTrigger(triggerId, target, actor, table, nodes);
				foreach (var node in nodes) {
					switch (node.Type) {
						case ScriptNode.NodeType.Function:
							I.m_scriptMgr.Run(node, actor, table);
							break;

						default:
							QueueNotification(node);
							break;
					}
				}
			}

			return responseHandle;
		}

		public static void LoadScript(LeafAsset asset) {
			I.m_scriptMgr.RegisterPackage(asset);
		}

		public static void UnloadScript(LeafAsset asset) {
			I.m_scriptMgr.DeregisterPackage(asset);
		}

		public static void RecordNodeVisited(ScriptNode node) {
			I.m_state.RecordNodeVisit(node);

			I.m_state.ClearNotification(node.ContactId);
			SetVariable(GameVars.LastNotifiedContactId, null);
		}
		public static void RecordNodeVisited(StringHash32 nodeId, StringHash32 contactId) {
			I.m_state.RecordNodeVisit(nodeId, contactId);
			I.m_state.ClearNotification(contactId);
			SetVariable(GameVars.LastNotifiedContactId, null);
		}

		#endregion // Scripting

		#region Notifications

		public static void QueueNotification(ScriptNode node) {
			if (I.m_state.QueueNotification(node.ContactId, node.Id())) {
				SetVariable(GameVars.LastNotifiedContactId, node.ContactId);
				Events.Dispatch(GameEvents.PhoneNotification, node);
			}
		}

		public static bool TryRunLastNotification(out LeafThreadHandle outHandle) {
			StringHash32 contactId = GetVariable(GameVars.LastNotifiedContactId).AsStringHash();
			return TryRunNotification(contactId, out outHandle);
		}

		public static bool TryRunNotification(StringHash32 contactId, out LeafThreadHandle outHandle) {
			if (contactId.IsEmpty) {
				outHandle = default(LeafThreadHandle);
				return false;
			}

			StringHash32 notificationId = I.m_state.GetContactNotificationId(contactId);
			ScriptNode node = I.m_scriptMgr.GetNotification(notificationId);
			if (node != null) {
				outHandle = RunScriptNode(node);
				return true;
			} else {
				outHandle = default(LeafThreadHandle);
				return false;
			}
		}

		#endregion // Notifications

		#region Variables

		public static Variant GetVariable(StringHash32 id) {
			return I.m_state.VariableTable[id];
		}

		public static void SetVariable(StringHash32 id, Variant value) {
			I.m_state.VariableTable[id] = value;
		}

		#endregion // Variables

		#region Leaf

		[LeafMember]
		public static void UnlockContact(StringHash32 contact) {
			if (I.m_state.UnlockContact(contact)) {
				Events.Dispatch(GameEvents.ContactUnlocked, contact);
				using (var table = TempVarTable.Alloc()) {
					table.Set("contactId", contact);
					RunTrigger(GameTriggers.OnContactAdded, table, null, contact);
				}
			}
		}

		[LeafMember]
		public static void UnlockLevel(int levelIndex) {
			if (I.m_state.UnlockLevel(levelIndex - 1)) {
				Events.Dispatch(GameEvents.LevelUnlocked, levelIndex - 1);
				using (var table = TempVarTable.Alloc()) {
					table.Set("levelIndex", levelIndex);
					RunTrigger(GameTriggers.OnLevelUnlock, table);
				}
			}
		}

		[LeafMember]
		public static void DiscoverLocation(int levelIndex) {
			if (I.m_state.DiscoverLocation(levelIndex - 1)) {
				Events.Dispatch(GameEvents.LocationDiscovered);
				// Events.Dispatch(GameEvents.LocationDiscovered, levelIndex - 1);
				/*
				using (var table = TempVarTable.Alloc()) {
					table.Set("levelIndex", levelIndex);
					RunTrigger(GameTriggers.OnLevelUnlock, table);
				}
				*/
			}
		}

		[LeafMember]
		public static void UnlockEvidence(int levelIndex, StringHash32 groupId) {
			if (I.m_state.UnlockEvidence(levelIndex - 1, groupId)) {
				Events.Dispatch(GameEvents.GameUnlockingEvidence); // assists Logging.cs with determining actor
				Events.Dispatch(GameEvents.EvidenceUnlocked, groupId);
				using (var table = TempVarTable.Alloc()) {
					table.Set("levelIndex", levelIndex);
					table.Set("evidence", groupId);
					RunTrigger(GameTriggers.OnEvidenceUnlock, table);
				}
			}
		}

		
		public static void UnlockEvidence(StringHash32 groupId) {
			if (I.m_state.UnlockEvidence(I.m_selectedLevel, groupId)) {
				Events.Dispatch(GameEvents.EvidenceUnlocked, groupId);
				using (var table = TempVarTable.Alloc()) {
					table.Set("levelIndex", I.m_selectedLevel + 1);
					table.Set("evidence", groupId);
					RunTrigger(GameTriggers.OnEvidenceUnlock, table);
				}
			}
		}
		
		[LeafMember]
		public static void RemoveEvidence(int levelIndex, StringHash32 groupId) {
			if (I.m_state.RemoveEvidence(levelIndex-1, groupId)) {
				Events.Dispatch(GameEvents.EvidenceRemoved, groupId);
			}
		}

		[LeafMember]
		private static void TriggerCutscene() {
			I.m_state.SetCutsceneSeen();
			UIMgr.CloseImmediately<UIOfficeScreen>();
			UIMgr.CloseImmediately<UIEvidenceScreen>();
			Routine.Start(Routine.Delay(() => { CutscenePlayer.Play(); }, 1.5f));
		}

		[LeafMember]
		private static bool HasEvidence(StringHash32 evidence) {
			return I.m_state.CurrentLevel.IsEvidenceUnlocked(evidence);
		}

		[LeafMember]
		private static bool HasContact(StringHash32 contact) {
			return I.m_state.IsContactUnlocked(contact);
		}

		[LeafMember]
		private static bool IsChainComplete(StringHash32 root) {
			return I.m_state.CurrentLevel.IsChainComplete(root);
		}

		[LeafMember]
		private static bool Seen(StringHash32 nodeId) {
			return I.m_state.HasVisitedNode(nodeId);
		}

		[LeafMember]
		private static void Trace(string inText) {
			Log.Msg(inText);
		}

		[LeafMember]
		public static void UnlockDive()
		{
			ShipOutMgr.instance.UnlockDive();
		}

		[LeafMember]
		private static void TriggerConvoMusic(string convo_id, string ambiance_id = null)
		{
			AudioSrcMgr.instance.StashAudio();
			AudioSrcMgr.instance.PlayAudio(convo_id, true);
			if (ambiance_id != null) {
				AudioSrcMgr.instance.PlayAmbiance(ambiance_id);
			}
		}

		[LeafMember]
		private static void EndConvoMusic()
		{
			AudioSrcMgr.instance.ResumeStashedAudio();
		}

		[LeafMember]
		private static void PauseSonar() {
			ShipOutMgr.instance.DisableSonar.Invoke();
		}

		[LeafMember]
		private static void ResumeSonar() {
			ShipOutMgr.instance.EnableSonar.Invoke();
		}

		[LeafMember]
		private static void MarkSonarTutorialComplete() {
			UIShipOutScreen.instance.MarkSonarTutorialComplete();
		}

		[LeafMember]
		private static void DropSonarTutorialBuoy() {
			UIShipOutScreen.instance.DropSonarTutorialBuoy();
			ShipOutMgr.instance.UnlockDive();
		}


		[LeafMember]
		private static void BeginLevel(int index) {
			SetLevelIndex(index - 1);
		}

		[LeafMember]
		private static void MapTutorial() {
			Events.Dispatch(GameEvents.MapTutorial);
		}

		[LeafMember]
		private static bool CurrentCustomMessageIs(StringHash32 messageKey) {
			return I.m_state.CurrentLevel.IsCurrentMessage(messageKey);
		}

		[LeafMember]
		private static bool CurrentObservationIs(StringHash32 observationKey) {
			return I.m_state.CurrentLevel.IsCurrentObservation(observationKey);
		}

		#endregion // Leaf

	}

}
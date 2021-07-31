using BeauPools;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using BeauUtil.Variants;
using Leaf;
using Leaf.Runtime;
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
		private PostItEvaluator m_stickyEval;
		private GameState m_state;
		private EventService m_eventService;

		private int m_selectedLevel = 0;

		[SerializeField]
		private PostItAsset m_postitLevel1;

		protected override void OnAssigned() {
			Routine.Settings.DebugMode = false;

			m_state = new GameState();

			m_stickyEval = new PostItEvaluator();
			m_stickyEval.Load(m_postitLevel1);

			m_scriptMgr = new ScriptMgr(this);
			m_scriptMgr.LoadGameState(m_state, m_state.VariableTable);
			m_scriptMgr.ConfigureEvents();

			m_eventService = new EventService();
		}

		public static PostItData EvaluateChain(StringHash32 root) {
			return I.m_stickyEval.Evaluate(root, I.m_state.GetChain(root).Chain());
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

			using(PooledList<ScriptNode> nodes = PooledList<ScriptNode>.Create()) {
				I.m_scriptMgr.GetResponsesForTrigger(triggerId, target, actor, table, nodes);
				foreach(var node in nodes) {
					switch(node.Type) {
						case ScriptNode.NodeType.Function:
							I.m_scriptMgr.Run(node, actor, table);
							break;

						default:
							if (node.IsNotification) {
								QueueNotification(node);
							} else {
								responseHandle = I.m_scriptMgr.Run(node, actor, table);
							}
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

			if (node.IsNotification) {
				I.m_state.ClearNotification(node.ContactId);
				SetVariable(GameVars.LastNotifiedContactId, null);
			}
		}

		#endregion // Scripting

		#region Notifications

		public static void QueueNotification(ScriptNode node) {
			if (I.m_state.QueueNotification(node.ContactId, node.Id())) {
				SetVariable(GameVars.LastNotifiedContactId, node.ContactId);
				Events.Dispatch(GameEvents.PhoneNotification, node.ContactId);
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
				using(var table = TempVarTable.Alloc()) {
					table.Set("contactId", contact);
					RunTrigger(GameTriggers.OnContactAdded, table, null, contact);
				}
			}
		}

		[LeafMember]
		public static void UnlockLevel(int levelIndex) {
			if (I.m_state.UnlockLevel(levelIndex - 1)) {
				Events.Dispatch(GameEvents.LevelUnlocked, levelIndex);
				using (var table = TempVarTable.Alloc()) {
					table.Set("levelIndex", levelIndex);
					RunTrigger(GameTriggers.OnLevelUnlock, table);
				}
			}
		}

		[LeafMember]
		public static void UnlockEvidence(int levelIndex, StringHash32 groupId) {
			if (I.m_state.UnlockEvidence(levelIndex - 1, groupId)) {
				Events.Dispatch(GameEvents.EvidenceUnlocked, groupId);
				using (var table = TempVarTable.Alloc()) {
					table.Set("levelIndex", levelIndex);
					table.Set("evidence", groupId);
					RunTrigger(GameTriggers.OnEvidenceUnlock, table);
				}
			}
		}


		[LeafMember]
		private static bool HasContact(StringHash32 contact) {
			return I.m_state.IsContactUnlocked(contact);
		}

		[LeafMember]
		private static bool Seen(StringHash32 nodeId) {
			return I.m_state.HasVisitedNode(nodeId);
		}

		[LeafMember]
		private static void Trace(string inText) {
			Log.Msg(inText);
		}

		#endregion // Leaf

	}

}
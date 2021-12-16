using BeauData;
using BeauUtil;
using BeauUtil.Variants;
using PotatoLocalization;
using System;
using System.Collections.Generic;

namespace Shipwreck {

	public interface IGameState {

		ILevelState CurrentLevel { get; }

		ILevelState GetLevel(int levelIndex);

		IEnumerable<StringHash32> GetUnlockedContacts();
		IEnumerable<IEvidenceGroupState> GetEvidence(int levelIndex);

		IEvidenceChainState GetChain(int levelIndex, StringHash32 identity);

		IEvidenceChainState GetChain(int levelIndex, int index);
		int GetChainCount(int levelIndex);

		bool IsContactUnlocked(StringHash32 contactId);
		bool IsLevelUnlocked(int levelUnlocked);

		LocalizationKey GetLevelName(int levelIndex);

		bool IsEvidenceUnlocked(int levelIndex, StringHash32 evidenceId);
		bool IsLocationChainComplete(int levelIndex);

		StringHash32 GetContactNotificationId(StringHash32 contactId);
		uint NotificationCount();

		bool HasVisitedNode(ScriptNode node);
		bool HasVisitedNode(StringHash32 nodeId);

		bool HasTakenTopDownPhoto(int levelIndex);

		bool IsDiveUnlocked(int shipOutIndex);
		bool UnlockDive(int shipOutIndex);

		int GetCurrShipOutIndex();
		void SetCurrShipOutIndex(int index);

		bool HasTutorialSonarDisplayed();
		void SetTutorialSonarDisplayed(bool hasDisplayed);

		bool HasTutorialBuoyDropped();
		void SetTutorialBuoyDropped(bool isDropped);

		bool HasTutorialPinDisplayed();
	}


	public sealed partial class GameMgr { // GameState.cs

		private struct QueuedNotification : ISerializedObject, ISerializedVersion {
			public StringHash32 ContactId;
			public StringHash32 NodeId;

			public ushort Version { get { return 1; } }

			public void Serialize(Serializer ioSerializer) {
				ioSerializer.UInt32Proxy("contactId", ref ContactId);
				ioSerializer.UInt32Proxy("nodeId", ref NodeId);
			}
		}

		private sealed partial class GameState : IGameState, ISerializedObject, ISerializedVersion {

			public ushort Version {
				get { return 1; }
			}
			public VariantTable VariableTable { 
				get { return m_variableTable; } 
			}
			public ILevelState CurrentLevel {
				get { return m_levelStates[m_currentLevel]; }
			}

			// non-serialized
			private int m_currentLevel;

			// serialized
			private VariantTable m_variableTable;
			private HashSet<StringHash32> m_visitedNodes;
			private HashSet<StringHash32> m_unlockedContacts;
			private List<QueuedNotification> m_queuedNotifications;
			private LevelState[] m_levelStates;
			private ShipOutState[] m_shipOutStates;
			private int m_currShipOutIndex; // does this need to be serialized?
			private bool m_tutorialBuoyDropped;
			private bool m_tutorialSonarDisplayed;

			public GameState() {
				m_variableTable = new VariantTable();
				m_visitedNodes = new HashSet<StringHash32>();
				m_unlockedContacts = new HashSet<StringHash32>() { "you", "mom", "amy" };
				m_queuedNotifications = new List<QueuedNotification>();
				m_levelStates = new LevelState[4] {
					new LevelState(), new LevelState(),
					new LevelState(), new LevelState()
				};
				for (int index = 0; index < m_levelStates.Length; index++) {
					m_levelStates[index].AssignLevelData(GameDb.GetLevelData(index));
				}
				m_shipOutStates = new ShipOutState[4] {
					new ShipOutState(),
					new ShipOutState(),
					new ShipOutState(),
					new ShipOutState()
				};
				for (int index = 0; index < m_shipOutStates.Length; index++)
				{
					m_shipOutStates[index].AssignShipOutData(GameDb.GetShipOutData(index));
				}
				m_currShipOutIndex = 0;
				m_tutorialBuoyDropped = false;
				m_tutorialSonarDisplayed = false;
			}

			public void Clear() {
				m_variableTable.Clear();
				m_visitedNodes.Clear();
				m_unlockedContacts = new HashSet<StringHash32>() { "you", "mom", "amy" };
				m_queuedNotifications = new List<QueuedNotification>();
				m_levelStates = new LevelState[4] {
					new LevelState(), new LevelState(),
					new LevelState(), new LevelState()
				};
				for (int index = 0; index < m_levelStates.Length; index++) {
					m_levelStates[index].AssignLevelData(GameDb.GetLevelData(index));
				}
				m_shipOutStates = new ShipOutState[4] {
					new ShipOutState(),
					new ShipOutState(),
					new ShipOutState(),
					new ShipOutState()
				};
				for (int index = 0; index < m_shipOutStates.Length; index++) {
					m_shipOutStates[index].AssignShipOutData(GameDb.GetShipOutData(index));
				}
				m_currShipOutIndex = 0;
				m_tutorialBuoyDropped = false;
				m_tutorialSonarDisplayed = false;
			}


			public ILevelState GetLevel(int levelIndex) {
				return m_levelStates[levelIndex];
			}
			public void SetCurrentLevel(int levelIndex) {
				m_currentLevel = levelIndex;
			}

			public int GetChainCount(int levelIndex) {
				return m_levelStates[levelIndex].ChainCount;
			}
			public IEnumerable<IEvidenceGroupState> GetEvidence(int levelIndex) {
				return m_levelStates[levelIndex].Evidence;
			}
			public IEvidenceChainState GetChain(int levelIndex, int index) {
				return m_levelStates[levelIndex].GetChain(index);
			}
			public IEvidenceChainState GetChain(int levelIndex, StringHash32 chainId) {
				return m_levelStates[levelIndex].GetChain(chainId);
			}
			public IEnumerable<StringHash32> GetUnlockedContacts() {
				foreach (StringHash32 hash in m_unlockedContacts) {
					yield return hash;
				}
			}
			public bool IsContactUnlocked(StringHash32 contactId) {
				return m_unlockedContacts.Contains(contactId);
			}
			public bool IsLevelUnlocked(int levelIndex) {
				if (levelIndex < 0 || levelIndex >= m_levelStates.Length) {
					throw new IndexOutOfRangeException();
				}
				return m_levelStates[levelIndex].IsUnlocked;
			}
			public LocalizationKey GetLevelName(int levelIndex) {
				if (levelIndex < 0 || levelIndex >= m_levelStates.Length) {
					throw new IndexOutOfRangeException();
				}
				return m_levelStates[levelIndex].Name;
			}

			public bool IsEvidenceUnlocked(int levelIndex, StringHash32 evidenceId) {
				return m_levelStates[levelIndex].IsEvidenceUnlocked(evidenceId);
			}
			public bool IsLocationChainComplete(int levelIndex) {
				return m_levelStates[levelIndex].IsLocationChainComplete();
			}

			public bool IsChainComplete(int levelIndex, StringHash32 root) {
				return m_levelStates[levelIndex].IsChainComplete(root);
			}
			public bool IsBoardComplete(int levelIndex) {
				for (int index = 0; index < m_levelStates[levelIndex].ChainCount; index++) {
					if (!m_levelStates[levelIndex].GetChain(index).IsCorrect) {
						return false;
					}
				}
				return true;
			}

			public bool UnlockContact(StringHash32 contact) {
				return m_unlockedContacts.Add(contact);
			}
			public bool UnlockLevel(int levelIndex) {
				if (levelIndex < 0 || levelIndex >= m_levelStates.Length) {
					throw new IndexOutOfRangeException();
				}
				return m_levelStates[levelIndex].Unlock();
			}
			public bool UnlockEvidence(int levelIndex, StringHash32 groupID) {
				if (levelIndex < 0 || levelIndex >= m_levelStates.Length) {
					throw new IndexOutOfRangeException();
				}
				return m_levelStates[levelIndex].UnlockEvidence(GameDb.GetEvidenceData(groupID));
			}
			public bool RemoveEvidence(int levelIndex, StringHash32 groupId) {
				if (levelIndex < 0 || levelIndex >= m_levelStates.Length) {
					throw new IndexOutOfRangeException();
				}
				return m_levelStates[levelIndex].RemoveEvidence(groupId);
			}
			public bool DiscoverLocation(int levelIndex) {
				if (levelIndex < 0 || levelIndex >= m_levelStates.Length) {
					throw new IndexOutOfRangeException();
				}
				return m_levelStates[levelIndex].DiscoverLocation();
			}

			public StringHash32 GetContactNotificationId(StringHash32 contactId) {
				for(int i = 0, len = m_queuedNotifications.Count; i < len; i++) {
					if (m_queuedNotifications[i].ContactId == contactId)
						return m_queuedNotifications[i].NodeId;
				}

				return StringHash32.Null;
			}
			public uint NotificationCount() {
				return (uint) m_queuedNotifications.Count;
			}

			public bool QueueNotification(StringHash32 contactId, StringHash32 nodeId) {
				QueuedNotification notification;
				for(int i = 0, len = m_queuedNotifications.Count; i < len; i++) {
					notification = m_queuedNotifications[i];
					if (notification.ContactId == contactId) {
						if (notification.NodeId != nodeId) {
							notification.NodeId = nodeId;
							m_queuedNotifications[i] = notification;
							return true;
						}
						return false;
					}
				}

				notification.ContactId = contactId;
				notification.NodeId = nodeId;
				m_queuedNotifications.Add(notification);
				return true;
			}

			public void ClearNotification(StringHash32 contactId) {
				for(int i = 0, len = m_queuedNotifications.Count; i < len; i++) {
					if (m_queuedNotifications[i].ContactId == contactId) {
						m_queuedNotifications.FastRemoveAt(i);
						return;
					}
				}
			}

			public bool HasVisitedNode(ScriptNode node) {
				return m_visitedNodes.Contains(node.Id());
			}
			public bool HasVisitedNode(StringHash32 nodeId) {
				return m_visitedNodes.Contains(nodeId);
			}
			public bool HasTakenTopDownPhoto(int levelIndex) {
				return m_levelStates[levelIndex].HasTakenTopDownPhoto();
			}
			public void RecordNodeVisit(ScriptNode node) {
				m_visitedNodes.Add(node.Id());
				ClearNotification(node.ContactId);
			}

			public void RecordNodeVisit(StringHash32 nodeId, StringHash32 contactId) {
				m_visitedNodes.Add(nodeId);
				ClearNotification(contactId);
			}

			public bool IsDiveUnlocked(int shipOutIndex)
			{
				if (shipOutIndex < 0 || shipOutIndex >= m_shipOutStates.Length)
				{
					throw new IndexOutOfRangeException();
				}
				return m_shipOutStates[shipOutIndex].IsDiveUnlocked();
			}

			public bool UnlockDive(int shipOutIndex)
			{
				if (shipOutIndex < 0 || shipOutIndex >= m_shipOutStates.Length)
				{
					throw new IndexOutOfRangeException();
				}
				return m_shipOutStates[shipOutIndex].UnlockDive();
			}

			public int GetCurrShipOutIndex(){
				return m_currShipOutIndex;
			}
			public void SetCurrShipOutIndex(int index){
				m_currShipOutIndex = index;
			}

			public bool HasTutorialSonarDisplayed()
			{
				return m_tutorialSonarDisplayed;
			}
			public void SetTutorialSonarDisplayed(bool hasDisplayed)
			{
				m_tutorialSonarDisplayed = hasDisplayed;
			}
			public bool HasTutorialBuoyDropped()
			{
				return m_tutorialBuoyDropped;
			}
			public void SetTutorialBuoyDropped(bool isDropped) 
			{
				m_tutorialBuoyDropped = isDropped;
			}
			public bool HasTutorialPinDisplayed() {
				return m_levelStates[0].IsLocationChainComplete();
			}
			public void SetCutsceneSeen() {
				m_levelStates[m_currentLevel].SetCutsceneSeen();
			}

			public void Serialize(Serializer ioSerializer) {
				ioSerializer.Object("variantTable", ref m_variableTable);
				ioSerializer.UInt32ProxySet("nodeVisits", ref m_visitedNodes);
				ioSerializer.UInt32ProxySet("unlockedContacts", ref m_unlockedContacts);
				ioSerializer.ObjectArray("queuedNotifications", ref m_queuedNotifications);
				ioSerializer.ObjectArray("levelStates", ref m_levelStates);
				ioSerializer.ObjectArray("shipOutStates", ref m_shipOutStates);

				ioSerializer.Serialize("tutorialBuoy", ref m_tutorialBuoyDropped);
				ioSerializer.Serialize("tutorialSonar", ref m_tutorialSonarDisplayed);

				if (ioSerializer.IsReading) {
					for (int index = 0; index < m_levelStates.Length; index++) {
						m_levelStates[index].AssignLevelData(GameDb.GetLevelData(index));
					}
				}
			}
		}

	}

}
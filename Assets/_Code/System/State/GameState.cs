using BeauData;
using BeauUtil;
using BeauUtil.Variants;
using System;
using System.Collections.Generic;

namespace Shipwreck {

	public interface IGameState {
		IEnumerable<StringHash32> GetUnlockedContacts();
		IEnumerable<IEvidenceGroupState> GetEvidence();

		IEvidenceChainState GetChain(StringHash32 identity);

		IEvidenceChainState GetChain(int index);
		int ChainCount { get; }

		bool IsContactUnlocked(StringHash32 contactId);
		bool IsLevelUnlocked(int levelUnlocked);

		bool IsEvidenceUnlocked(StringHash32 evidenceId);

		bool IsLocationChainComplete();

		StringHash32 GetContactNotificationId(StringHash32 contactId);
		uint NotificationCount();

		bool HasVisitedNode(ScriptNode node);
		bool HasVisitedNode(StringHash32 nodeId);

		bool HasTakenTopDownPhoto();

		bool IsDiveUnlocked(int shipOutIndex);
		bool UnlockDive(int shipOutIndex);

		int GetCurrShipOutIndex();
		void SetCurrShipOutIndex(int index);
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

			public int ChainCount {
				get { return m_levelStates[m_levelIndex].ChainCount; }
			}

			private int m_levelIndex = 0;

			// serialized
			private VariantTable m_variableTable;
			private HashSet<StringHash32> m_visitedNodes;
			private HashSet<StringHash32> m_unlockedContacts;
			private List<QueuedNotification> m_queuedNotifications;
			private LevelState[] m_levelStates;
			private ShipOutState[] m_shipOutStates;
			private int m_currShipOutIndex;

			public GameState() {
				m_variableTable = new VariantTable();
				m_visitedNodes = new HashSet<StringHash32>();
				m_unlockedContacts = new HashSet<StringHash32>() { "mom" };
				m_queuedNotifications = new List<QueuedNotification>();
				m_levelStates = new LevelState[4] {
					new LevelState(), new LevelState(),
					new LevelState(), new LevelState()
				};
				for (int index = 0; index < m_levelStates.Length; index++) {
					m_levelStates[index].AssignLevelData(GameDb.GetLevelData(index));
				}
				m_shipOutStates = new ShipOutState[1] {
					new ShipOutState()
				};
				for (int index = 0; index < m_shipOutStates.Length; index++)
				{
					m_shipOutStates[index].AssignShipOutData(GameDb.GetShipOutData(index));
				}
				m_currShipOutIndex = 0;
			}

			public IEnumerable<IEvidenceGroupState> GetEvidence() {
				return m_levelStates[m_levelIndex].Evidence;
			}
			public IEvidenceChainState GetChain(int index) {
				return m_levelStates[m_levelIndex].GetChain(index);
			}
			public IEvidenceChainState GetChain(StringHash32 chainId) {
				return m_levelStates[m_levelIndex].GetChain(chainId);
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
			public bool IsEvidenceUnlocked(StringHash32 evidenceId) {
				return m_levelStates[m_levelIndex].IsEvidenceUnlocked(evidenceId);
			}
			public bool IsLocationChainComplete() {
				return m_levelStates[m_levelIndex].IsLocationChainComplete();
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
			public bool HasTakenTopDownPhoto() {
				return m_levelStates[m_levelIndex].HasTakenTopDownPhoto();
			}
			public void RecordNodeVisit(ScriptNode node) {
				m_visitedNodes.Add(node.Id());
				if (node.IsNotification) {
					ClearNotification(node.ContactId);
				}
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

			public void Serialize(Serializer ioSerializer) {
				ioSerializer.Object("variantTable", ref m_variableTable);
				ioSerializer.UInt32ProxySet("nodeVisits", ref m_visitedNodes);
				ioSerializer.UInt32ProxySet("unlockedContacts", ref m_unlockedContacts);
				ioSerializer.ObjectArray("queuedNotifications", ref m_queuedNotifications);
				ioSerializer.ObjectArray("levelStates", ref m_levelStates);

				if (ioSerializer.IsReading) {
					for (int index = 0; index < m_levelStates.Length; index++) {
						m_levelStates[index].AssignLevelData(GameDb.GetLevelData(index));
					}
				}
			}
		}

	}

}
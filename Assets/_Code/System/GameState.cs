using BeauData;
using BeauUtil;
using BeauUtil.Variants;
using System.Collections.Generic;

namespace Shipwreck {

	public interface IGameState {
		IEnumerable<StringHash32> GetUnlockedContacts();
		bool HasVisitedNode(ScriptNode node);
	}


	public sealed partial class GameMgr : Singleton<GameMgr> { // GameState.cs

		private sealed partial class GameState : IGameState, ISerializedObject, ISerializedVersion {

			public ushort Version {
				get { return 1; }
			}
			public VariantTable VariableTable { 
				get { return m_variableTable; } 
			}

			// serialized
			private VariantTable m_variableTable;
			private HashSet<StringHash32> m_visitedNodes;
			private HashSet<StringHash32> m_unlockedContacts;
			private LevelState m_level1;
			private LevelState m_level2;
			private LevelState m_level3;
			private LevelState m_level4;
			private LevelState m_level5;
			private LevelState m_levelGrandpa;

			// non-serialized
			private CustomVariantResolver m_customResolver;


			public GameState() {
				m_variableTable = new VariantTable();
				m_visitedNodes = new HashSet<StringHash32>();
				m_unlockedContacts = new HashSet<StringHash32>() {"dad"};
				m_customResolver = new CustomVariantResolver();
				InitializeLevels();
			}

			public IEnumerable<StringHash32> GetUnlockedContacts() {
				foreach (StringHash32 hash in m_unlockedContacts) {
					yield return hash;
				}
			}
			public void UnlockContact(StringHash32 contact) {
				m_unlockedContacts.Add(contact);
			}

			public bool HasVisitedNode(ScriptNode node) {
				return m_visitedNodes.Contains(node.Id());
			}
			public void RecordNodeVisit(ScriptNode node) {
				m_visitedNodes.Add(node.Id());
			}


			public void Serialize(Serializer ioSerializer) {
				ioSerializer.Object("variantTable", ref m_variableTable);
				ioSerializer.UInt32ProxySet("nodeVisits", ref m_visitedNodes);
				ioSerializer.UInt32ProxySet("unlockedContacts", ref m_unlockedContacts);
			}

			private void InitializeLevels() {
				m_level1 = new LevelState();
				m_level2 = new LevelState();
				m_level3 = new LevelState();
				m_level4 = new LevelState();
				m_level5 = new LevelState();
				m_level1.Unlock();
			}

		}

	}

}
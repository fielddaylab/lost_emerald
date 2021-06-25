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

		private class GameState : IGameState, ISerializedObject, ISerializedVersion {
			public ushort Version {
				get { return 1; }
			}
			public VariantTable VariableTable { 
				get { return m_variableTable; } 
			}

			// serialized
			private VariantTable m_variableTable;
			private Dictionary<string, int> m_nodeVisits;
			private HashSet<StringHash32> m_unlockedContacts;

			// non-serialized
			private CustomVariantResolver m_customResolver;

			public GameState() {
				m_variableTable = new VariantTable();
				m_nodeVisits = new Dictionary<string, int>();
				m_unlockedContacts = new HashSet<StringHash32>() {"dad"};
				m_customResolver = new CustomVariantResolver();
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
				if (m_nodeVisits.TryGetValue(node.FullName, out int visits)) {
					return visits > 0;
				} else {
					return false;
				}
			}
			public void RecordNodeVisit(ScriptNode node) {
				if (m_nodeVisits.ContainsKey(node.FullName)) {
					m_nodeVisits[node.FullName]++;
				} else {
					m_nodeVisits.Add(node.FullName, 1);
				}
			}


			public void Serialize(Serializer ioSerializer) {
				ioSerializer.Object("variantTable", ref m_variableTable);
				ioSerializer.Map("nodeVisits", ref m_nodeVisits);
				ioSerializer.UInt32ProxySet("unlockedContacts", ref m_unlockedContacts);
			}

		}

	}

}
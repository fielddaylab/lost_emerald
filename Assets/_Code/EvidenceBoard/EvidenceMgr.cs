using BeauUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {

	public sealed partial class EvidenceMgr : UIBase {

		private class Connection {

			private StringHash32 m_startNode;
			private StringHash32 m_endNode;

		}

		private List<EvidenceNode> m_nodes;
		private List<Connection> m_connections;
		private Dictionary<StringHash32, List<EvidenceNode>> m_groups;

		private void Awake() {
			m_nodes = new List<EvidenceNode>();
			m_connections = new List<Connection>();
			m_groups = new Dictionary<StringHash32, List<EvidenceNode>>();
		}

		public void AddEvidence(EvidenceGroup group) {
			foreach (EvidenceNode node in group.Nodes) {
				AddNode(group.GroupID, node);
			}
		}
		private void AddNode(StringHash32 group, EvidenceNode node) {
			if (!m_groups.TryGetValue(group,out List<EvidenceNode> list)) {
				list = new List<EvidenceNode>();
				m_groups.Add(group, list);
			}
			list.Add(node);
			m_nodes.Add(node);
		}
		private void RemoveNode(StringHash32 group, EvidenceNode node) {
			if (m_groups.TryGetValue(group,out List<EvidenceNode> list)) {
				list.Remove(node);
				m_nodes.Remove(node);
			}
		}

		protected override IEnumerator ShowRoutine() {
			throw new System.NotImplementedException();
		}

		protected override IEnumerator HideRoutine() {
			throw new System.NotImplementedException();
		}
	}

}
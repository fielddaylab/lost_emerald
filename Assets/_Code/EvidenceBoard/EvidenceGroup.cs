using BeauUtil;
using UnityEngine;

namespace Shipwreck {

	public class EvidenceGroup : MonoBehaviour {

		public StringHash32 GroupID {
			get {
				return m_groupID;
			}
		}
		public EvidenceNode[] Nodes {
			get {
				return m_nodes;
			}
		}

		[SerializeField]
		private SerializedHash32 m_groupID;
		[SerializeField]
		private EvidenceNode[] m_nodes;



	}

}
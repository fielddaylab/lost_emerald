using BeauUtil;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shipwreck {

	public class EvidenceGroup : MonoBehaviour, IPointerDownHandler {

		public StringHash32 GroupID {
			get { return m_groupID; }
		}
		public EvidenceNode[] Nodes {
			get { return m_nodes; }
		}

		[SerializeField]
		private SerializedHash32 m_groupID;
		[SerializeField]
		private EvidenceNode[] m_nodes;

		public void OnPointerDown(PointerEventData eventData) {
			throw new System.NotImplementedException();
		}
	}

}
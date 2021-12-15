using UnityEngine;
using UnityEngine.EventSystems;

namespace Shipwreck {

	[RequireComponent(typeof(RectTransform))]
	public class EvidenceGroup : MonoBehaviour /*, IPointerDownHandler, IPointerUpHandler*/ {

		public RectTransform RectTransform {
			get {
				if (m_rectTransform == null) {
					m_rectTransform = GetComponent<RectTransform>();
				}
				return m_rectTransform;
			}
		}
		public EvidenceNode[] Nodes {
			get { return m_nodes; }
		}
		public float PopupScale {
			get { return m_popupScale; }
		}

		[SerializeField]
		private EvidenceNode[] m_nodes;
		[SerializeField,Tooltip("Scale to display the evidence at when shown in dialogue.")]
		private float m_popupScale = 1f;

		private RectTransform m_rectTransform;
		private Vector2 m_offset;
		private bool m_selected = false;

		private void Update() {
			if (m_selected) {
				RectTransformUtility.ScreenPointToLocalPointInRectangle(
					(RectTransform)RectTransform.parent, InputMgr.Position, Camera.main, out Vector2 point
				);
				RectTransform.localPosition = point - m_offset;
			}
		}

		public void RemoveNodes() {
			foreach (EvidenceNode node in m_nodes) {
				Destroy(node.gameObject);
			}
			m_nodes = null;
		}

	}

}
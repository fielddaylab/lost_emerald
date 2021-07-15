using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Shipwreck {

	[RequireComponent(typeof(RectTransform))]
	public class EvidenceGroup : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

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

		[SerializeField]
		private EvidenceNode[] m_nodes;

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

		public void OnPointerDown(PointerEventData eventData) {
			m_selected = true;
			RectTransform.SetAsLastSibling();
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				RectTransform, InputMgr.Position, Camera.main, out m_offset
			);
		}
		public void OnPointerUp(PointerEventData eventData) {
			if (m_selected) {
				m_selected = false;
			}
		}
	}

}
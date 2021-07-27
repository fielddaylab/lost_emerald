using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shipwreck {

	public class EvidencePin : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

		public event Action<EvidencePin> OnPointerDown;
		public event Action<EvidencePin> OnPointerUp;

		public event Action<EvidencePin> OnPositionSet;

		public RectTransform RectTransform {
			get {
				if (m_rectTransform == null) {
					m_rectTransform = GetComponent<RectTransform>();
				}
				return m_rectTransform;
			}
		}
		public EvidenceNode Link {
			get { return m_link; }
		}

		private RectTransform m_rectTransform;
		private EvidenceNode m_link;

		public void SetPosition(Vector2 screenPos) {
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				(RectTransform)RectTransform.parent, screenPos, Camera.main, out Vector2 point
			);
			RectTransform.localPosition = point;
			OnPositionSet?.Invoke(this);
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
			OnPointerDown?.Invoke(this);
		}
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
			OnPointerUp?.Invoke(this);
		}

	}

}
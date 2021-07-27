using BeauRoutine;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

		[SerializeField]
		private Image m_image = null;

		private RectTransform m_rectTransform;
		private Routine m_routine;

		public void SetPosition(Vector2 screenPos) {
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				(RectTransform)RectTransform.parent, screenPos, Camera.main, out Vector2 point
			);
			RectTransform.localPosition = point;
			OnPositionSet?.Invoke(this);
		}

		public void SetColor(Color color) {
			m_routine.Replace(this, m_image.ColorTo(color, 0.2f));
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
			OnPointerDown?.Invoke(this);
		}
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
			OnPointerUp?.Invoke(this);
		}

	}

}
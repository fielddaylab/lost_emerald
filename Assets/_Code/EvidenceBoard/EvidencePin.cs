using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Shipwreck {

	public class EvidencePin : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

		public Action<EvidencePin> OnPointerDown;
		public Action<EvidencePin> OnPointerUp;

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

		public void SetLink(EvidenceNode node) {
			m_link = node;
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
			OnPointerDown?.Invoke(this);

		}
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
			OnPointerUp?.Invoke(this);
		}

	}

}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Shipwreck {

	public class EvidencePin : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

		public Action<EvidencePin> OnPressed;

		public RectTransform RectTransform {
			get {
				if (m_rectTransform == null) {
					m_rectTransform = GetComponent<RectTransform>();
				}
				return m_rectTransform;
			}
		}

		private RectTransform m_rectTransform;
		private Vector2 m_offset;
		private bool m_selected = false;

		private Transform m_layerParent;

		public void SetLayerParent(Transform transform) {
			m_layerParent = transform;
		}

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
			//GROOSSSSSSS
			GraphicRaycaster caster = GetComponentInParent<GraphicRaycaster>();
			List<RaycastResult> results = new List<RaycastResult>();
			caster.Raycast(eventData, results);
			bool foundNode = false;
			foreach (RaycastResult result in results) {
				EvidenceNode node = result.gameObject.GetComponent<EvidenceNode>();
				if (node != null) {
					transform.SetParent(node.transform);
					foundNode = true;
					break;
				}
			}
			if (!foundNode) {
				transform.SetParent(m_layerParent);
			}
		}

	}

}
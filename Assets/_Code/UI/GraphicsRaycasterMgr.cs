using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Shipwreck {
	public class GraphicsRaycasterMgr : MonoBehaviour {
		public static GraphicsRaycasterMgr instance;

		private GraphicRaycaster m_raycaster;
		private List<RaycastResult> m_raycastResults;

		public GraphicRaycaster Raycaster {
			get { return m_raycaster; }
		}
		public List<RaycastResult> RaycastResults {
			get { return m_raycastResults; }
		}

		private void Awake() {
			if (instance == null) {
				instance = this;
			}
			else if (this != instance) {
				Destroy(this.gameObject);
			}

			m_raycaster = GetComponentInParent<GraphicRaycaster>();
			m_raycastResults = new List<RaycastResult>();
		}

		public bool RaycastForNode(Vector2 screenPos, out EvidenceNode node) {
			instance.RaycastResults.Clear();
			PointerEventData eventData = new PointerEventData(EventSystem.current);
			eventData.position = screenPos;
			instance.Raycaster.Raycast(eventData, instance.RaycastResults);
			// we only care about the first node result
			foreach (RaycastResult result in instance.RaycastResults) {
				node = result.gameObject.GetComponent<EvidenceNode>();
				if (node != null) {
					return true;
				}
			}
			node = null;
			return false;
		}
	}
}
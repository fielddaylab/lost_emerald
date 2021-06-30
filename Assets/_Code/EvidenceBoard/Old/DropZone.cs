using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {

	public class DropZone : MonoBehaviour {

		[SerializeField]
		private Transform[] m_dropPoints = null;
		[SerializeField]
		private YarnNode m_outNode = null;

		private YarnNode m_spawnedNode = null;

		private List<Transform> m_attached = new List<Transform>();

		public Vector3 Attach(Transform toDrop) {
			Transform closest = GetClosestDropPoint(toDrop.position);
			toDrop.SetParent(closest);

			if (m_attached.Contains(toDrop)) {
				return closest.position;
			}
			m_attached.Add(toDrop);
			// spawn a node if we don't have one
			if (m_spawnedNode == null && m_outNode != null) {
				m_spawnedNode = Instantiate(EvidenceBoard.DroppableNodePrefab, null);
				m_spawnedNode.transform.position = m_outNode.transform.position + Vector3.down;
				m_spawnedNode.transform.localScale = Vector3.one;
				m_outNode.SetChildNode(m_spawnedNode);
			}
			return closest.position;
		}
		public void Remove(Transform toRemove) {
			m_attached.Remove(toRemove);
			if (m_attached.Count <= 0 && m_spawnedNode != null) {
				Destroy(m_spawnedNode.gameObject);
				m_outNode.SetChildNode(null);
			}
		}

		public Transform GetClosestDropPoint(Vector3 point) {
			float distance = float.MaxValue;
			Transform result = null;
			for (int ix = 0; ix < m_dropPoints.Length; ix++) {
				float newDist = Vector3.Distance(point, m_dropPoints[ix].position);
				if (newDist < distance) {
					distance = newDist;
					result = m_dropPoints[ix];
				}
			}
			return result;
		}


	}

}



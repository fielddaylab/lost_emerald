using UnityEngine;

namespace Shipwreck {

	public class DropZone : MonoBehaviour {

		[SerializeField]
		private Transform[] m_dropPoints;

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



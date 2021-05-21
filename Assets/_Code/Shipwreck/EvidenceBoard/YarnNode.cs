using UnityEngine;

namespace Shipwreck {


	public class YarnNode : MonoBehaviour {

		[SerializeField]
		private YarnNode m_child;
		[SerializeField]
		private LineRenderer m_lineRenderer;
		[SerializeField]
		private Transform m_label;
		[SerializeField, Range(0f, 1f)]
		private float m_labelDistance = 0.5f;

		private bool m_updateLine = false;
		private bool m_updateLabel = false;


		private void OnEnable() {
			if (m_child != null && m_lineRenderer != null) {
				m_lineRenderer.enabled = true;
				m_updateLine = true;
				if (m_label != null) {
					m_updateLabel = true;
				}
			}
		}

		private void Update() {
			if (m_updateLine) {
				Vector3[] points = new Vector3[2] {
					transform.position,
					m_child.transform.position
				};
				m_lineRenderer.SetPositions(points);
				if (m_updateLabel) {
					m_label.transform.position = Vector3.Lerp(points[0], points[1], m_labelDistance);
				}
			}
		}

	}

}
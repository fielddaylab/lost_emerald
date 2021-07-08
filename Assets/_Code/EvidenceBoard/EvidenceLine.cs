using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Shipwreck {

	public class EvidenceLine : MonoBehaviour {

		[SerializeField]
		private UILineRenderer m_lineRenderer = null;
		[SerializeField]
		private float m_labelYOffset = 20f;

		private Vector2[] m_points = new Vector2[2];
		private RectTransform m_a;
		private RectTransform m_b;
		private RectTransform m_label;

		private void Awake() {
			m_lineRenderer.Points = m_points;
		}

		public void Setup(RectTransform a, RectTransform b, RectTransform label = null) {
			m_a = a;
			m_b = b;
			m_label = label;
			Refresh();
		}
		private void Update() {
			Refresh();//hack
		}

		public void Refresh() {
			m_points[0] = m_a.position;
			m_points[1] = m_b.position;
			if (m_label != null) {
				m_label.anchoredPosition = (m_points[1] - m_points[0]) * 0.5f + Vector2.up * m_labelYOffset;
			}
			m_lineRenderer.SetAllDirty();
		}

	}

}



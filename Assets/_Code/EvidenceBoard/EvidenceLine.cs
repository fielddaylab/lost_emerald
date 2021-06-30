using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Shipwreck {

	public class EvidenceLine : MonoBehaviour {

		[SerializeField]
		private UILineRenderer m_lineRenderer = null;
		[SerializeField]
		private RectTransform m_labelGroup = null;
		[SerializeField]
		private float m_yOffset = 20f;
		[SerializeField]
		private RectTransform m_pin = null;

		private Vector2[] m_points = new Vector2[2];

		private void Awake() {
			m_points[0] = m_lineRenderer.Points[0];
			m_points[1] = m_pin.anchoredPosition;
			m_lineRenderer.Points = m_points;
		}

		private void Update() {
			// hack
			Refresh();
		}

		public void Refresh() {
			m_points[1] = m_pin.anchoredPosition;
			m_labelGroup.anchoredPosition = (m_points[1] - m_points[0]) * 0.5f + Vector2.up * m_yOffset;
			m_lineRenderer.SetAllDirty();
		}

	}

}



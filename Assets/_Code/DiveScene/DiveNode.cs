using BeauRoutine;
using Cinemachine;
using UnityEngine;

namespace Shipwreck {

	public class DiveNode : MonoBehaviour {

		[SerializeField]
		private CinemachineVirtualCamera m_camera = null;
		[SerializeField]
		private Collider m_collider = null;
		[SerializeField]
		private MeshRenderer m_circleRenderer = null;
		//[SerializeField]
		//private string m_colorProperty = "_BaseColor";

		private DivePointOfInterest m_pointOfInterest;
		//private Routine m_colorRoutine;

		public void Prioritize() {
			m_camera.Priority = 10;
			m_collider.enabled = false;
			m_circleRenderer.enabled = false;
		}
		public void Deprioritize() {
			m_camera.Priority = 0;
			m_collider.enabled = true;
			m_circleRenderer.enabled = true;
		}
		public bool MatchesCollider(Collider collider) {
			return m_collider == collider;
		}
		public DivePointOfInterest GetPointOfInterest() {
			if (m_pointOfInterest == null) {
				m_pointOfInterest = GetComponent<DivePointOfInterest>();
			}
			// it is acceptable that this can return null
			return m_pointOfInterest;
		}
		/*
		public void SetColor(Color color) {
			m_colorRoutine.Replace(this, m_circleRenderer.material.ColorTo(m_colorProperty, color, 0.25f, ColorUpdate.FullColor).Ease(Curve.QuadInOut));
		}
		*/
	}

}
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
		private Vector3 m_startPosition;
		private Routine m_zoomRoutine;
		//private Routine m_colorRoutine;

		private void Awake() {
			m_startPosition = m_camera.transform.position;
		}

		public void Prioritize() {
			m_camera.Priority = 10;
		}
		public void Deprioritize() {
			m_camera.Priority = 0;
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

		public void SetActive(bool isActive) {
			m_collider.enabled = isActive;
			m_circleRenderer.enabled = isActive;
		}

		public void SetZoom(float percent) {
			m_zoomRoutine.Replace(this, Tween.Float(
				m_camera.m_Lens.FieldOfView,
				60f - 30f * percent,
				FieldOfViewSetter, 0.1f
			).Ease(Curve.QuadOut));
			//m_startPosition + m_camera.transform.forward * 5f * percent, 
		}
		private void FieldOfViewSetter(float value) {
			m_camera.m_Lens.FieldOfView = value;
		}

		/*
		public void SetColor(Color color) {
			m_colorRoutine.Replace(this, m_circleRenderer.material.ColorTo(m_colorProperty, color, 0.25f, ColorUpdate.FullColor).Ease(Curve.QuadInOut));
		}
		*/
	}

}
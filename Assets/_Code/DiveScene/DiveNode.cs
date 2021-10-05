using BeauRoutine;
using Cinemachine;
using System.Collections;
using UnityEngine;

namespace Shipwreck {

	public class DiveNode : MonoBehaviour {

		[SerializeField]
		private CinemachineVirtualCamera m_camera = null;
		[SerializeField]
		private Collider m_collider = null;
		[SerializeField]
		private MeshRenderer m_circleRenderer = null;

		private DivePointOfInterest m_pointOfInterest;
		private Vector3 m_startPosition;
		private Routine m_zoomRoutine;
		private Routine m_pulseRoutine;
		private float m_startScale = 1f;

		private void Awake() {
			m_startPosition = m_camera.transform.position;
			m_startScale = m_circleRenderer.transform.localScale.x;
			m_pulseRoutine.Replace(this, PulseRoutine()).ExecuteWhileDisabled();
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
		}
		private void FieldOfViewSetter(float value) {
			m_camera.m_Lens.FieldOfView = value;
		}
		private IEnumerator PulseRoutine() {
			while (true) {
				yield return Routine.Combine(
					m_circleRenderer.transform.ScaleTo(m_startScale - m_startScale * 0.1f, 1f, Axis.XY).Ease(Curve.QuadIn),
					m_circleRenderer.material.FadeTo("_BaseColor", 0.3f, 1f)
				);
				yield return Routine.Combine(
					m_circleRenderer.transform.ScaleTo(m_startScale + m_startScale * 0.1f, 1f, Axis.XY).Ease(Curve.QuadOut),
					m_circleRenderer.material.FadeTo("_BaseColor", 1f, 1f)
				);
			}
		}

	}

}
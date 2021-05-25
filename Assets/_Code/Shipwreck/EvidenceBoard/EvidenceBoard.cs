using BeauRoutine;
using UnityEngine;

namespace Shipwreck {


	public class EvidenceBoard : MonoBehaviour {

		[SerializeField]
		private float m_raycastDistance = 50f;
		[SerializeField]
		private LayerMask m_dragLayer = 0;
		[SerializeField]
		private float m_dragIncreaseZ = 4;
		[SerializeField]
		private TweenSettings m_dragTweenSettings = new TweenSettings(0.2f, Curve.QuadOut);

		private Draggable m_selected;
		private Vector3 m_selectionOffset;
		private Vector3 m_originalPosition;

		private Routine m_routine;


		private void OnEnable() {
			Routine.Settings.DebugMode = false;
			InputMgr.Register(InputMgr.OnInteractPressed, HandleInteractPressed);
			InputMgr.Register(InputMgr.OnInteractReleased, HandleInteractReleased);
		}
		private void OnDisable() {
			InputMgr.Deregister(InputMgr.OnInteractPressed, HandleInteractPressed);
			InputMgr.Deregister(InputMgr.OnInteractReleased, HandleInteractReleased);
		}


		private void HandleInteractPressed() {
			Ray ray = Camera.main.ScreenPointToRay(InputMgr.Position);
			if (Physics.Raycast(ray, out RaycastHit hitInfo, m_raycastDistance, m_dragLayer)) {
				Draggable draggable = hitInfo.collider.GetComponent<Draggable>();
				if (draggable != null) {
					m_selected = draggable;
					m_originalPosition = m_selected.transform.position;
					m_selectionOffset = hitInfo.point - m_selected.transform.position;
					m_routine.Replace(this, Tween.ZeroToOne(SetDragPosition, m_dragTweenSettings));
					draggable.OnPickup();
				}
			}
		}
		private void HandleInteractReleased() {
			if (m_selected == null) {
				return;
			}
			if (m_selected.IsDroppable) {

			} else {
				m_routine.Replace(this, Tween.OneToZero(SetDragPosition, m_dragTweenSettings))
					.OnComplete(OnSetDropComplete).OnStop(OnSetDropComplete);
				// need to place object based on offset and mouse position
				// to avoid distortion based on camera
				Vector3 world = MouseToWorldPos(InputMgr.Position, -Camera.main.transform.position.z) - m_selectionOffset;
				world.z = m_originalPosition.z;
				m_originalPosition = world;
			}
		}
		private void Update() {
			if (m_selected == null || m_routine) {
				return; // nothing to do right now
			}
			float distance = -Camera.main.transform.position.z - m_dragIncreaseZ;
			m_selected.transform.position = MouseToWorldPos(InputMgr.Position, distance) - m_selectionOffset;
		}

		private void SetDragPosition(float value) {
			float distance = -Camera.main.transform.position.z - m_dragIncreaseZ;
			m_selected.transform.position = Vector3.Lerp(m_originalPosition, MouseToWorldPos(InputMgr.Position, distance) - m_selectionOffset, value);
		}
		private void OnSetDropComplete() {
			m_selected.transform.position = m_originalPosition;
			m_selected = null;
			m_selectionOffset = Vector2.zero;
		}

		private Vector3 MouseToWorldPos(Vector2 mouse, float distance) {
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(
				mouse.x, mouse.y, distance
			));
			
			return worldPos;
		}

	}


}



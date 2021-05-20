using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shipwreck {


	public class EvidenceBoard : MonoBehaviour {

		[SerializeField]
		private float m_raycastDistance = 50f;
		[SerializeField]
		private LayerMask m_dragLayer = 0;
		[SerializeField]
		private float m_dragCameraDistance = 20;
		[SerializeField]
		private TweenSettings m_dragTweenSettings = new TweenSettings(0.2f, Curve.QuadOut);

		private Draggable m_selected;
		private Vector3 m_selectionOffset;
		private Vector3 m_originalPosition;

		private Routine m_routine;


		private void OnEnable() {
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

				Vector3 pos = m_selected.transform.position;
				pos.z = m_originalPosition.z;
				m_originalPosition = pos;
			}
		}
		private void Update() {
			if (m_selected == null || m_routine) {
				return; // nothing to do right now
			}
			Vector3 screenPos = new Vector3(
				InputMgr.Position.x,
				InputMgr.Position.y,
				m_dragCameraDistance
			);
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
			m_selected.transform.position = worldPos - m_selectionOffset;
		}

		private void SetDragPosition(float value) {
			Vector3 screenPos = new Vector3(
				InputMgr.Position.x,
				InputMgr.Position.y,
				m_dragCameraDistance
			);
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos) - m_selectionOffset;
			m_selected.transform.position = Vector3.Lerp(m_originalPosition, worldPos, value);
		}
		private void OnSetDropComplete() {
			m_selected.transform.position = m_originalPosition;
			m_selected = null;
			m_selectionOffset = Vector2.zero;
		}

	}


}



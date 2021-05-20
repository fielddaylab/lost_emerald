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

		private Draggable m_selected;
		private Vector3 m_selectionOffset;
		private float m_originalZ;


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
					m_originalZ = m_selected.transform.position.z;
					m_selectionOffset = hitInfo.point - m_selected.transform.position;
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
				Vector3 position = m_selected.transform.position;
				position.z = m_originalZ;
				m_selected.transform.position = position;
				m_selected = null;
				m_selectionOffset = Vector2.zero;
			}
		}
		private void Update() {
			if (m_selected == null) {
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

	}


}



﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shipwreck {


	public class EvidenceBoard : MonoBehaviour {

		[SerializeField]
		private float m_raycastDistance = 50f;
		[SerializeField]
		private LayerMask m_dragLayer = 0;

		private Draggable m_selected;
		private Vector3 m_selectionOffset;


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
			if (Physics.Raycast(ray,out RaycastHit hitInfo,m_raycastDistance,m_dragLayer)) {
				Draggable draggable = hitInfo.collider.GetComponent<Draggable>();
				if (draggable != null) {
					//m_selectionOffset = draggable.transform.InverseTransformPoint(hitInfo.point);
					m_selected = draggable;
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
				Mathf.Abs(m_selected.transform.position.z - Camera.main.transform.position.z) - 3f
			);
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
			m_selected.transform.position = worldPos;
		}

	}


}



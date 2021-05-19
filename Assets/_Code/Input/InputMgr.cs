using BeauUtil;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shipwreck {


	public sealed class InputMgr : Singleton<InputMgr> {

		public static readonly StringHash32 OnInteractPressed = new StringHash32("OnInteractPressed");

		public static readonly StringHash32 OnInteractReleased = new StringHash32("OnInteractReleased");

		/// <summary>
		/// Returns current position of the Mouse/Touch in Screen Space
		/// </summary>
		public static Vector2 Position { 
			get { return I.m_controls.Default.Position.ReadValue<Vector2>(); } 
		}

		private EventService m_eventService;
		private Controls m_controls;

		public static void Register(StringHash32 id, Action handler) {
			I.m_eventService.Register(id, handler);
		}
		public static void Deregister(StringHash32 id, Action handler) {
			I?.m_eventService?.Deregister(id, handler);
		}


		private void OnEnable() {
			if (m_eventService == null) {
				m_eventService = new EventService();
			}
			if (m_controls == null) {
				m_controls = new Controls();
			}
			m_controls.Default.Enable();
			m_controls.Default.Interact.performed += HandleInteractPressed;
			m_controls.Default.Interact.canceled += HandleInteractReleased;
		}
		private void OnDisable() {
			if (m_controls != null) {
				m_controls.Default.Interact.performed -= HandleInteractPressed;
				m_controls.Default.Interact.canceled -= HandleInteractReleased;
			}
		}


		private void HandleInteractPressed(InputAction.CallbackContext context) {
			m_eventService.Dispatch(OnInteractPressed);
		}
		private void HandleInteractReleased(InputAction.CallbackContext context) {
			m_eventService.Dispatch(OnInteractReleased);
		}


	}

}
/*
 * Author(s): Levi Huillet
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck
{
	/// <summary>
	/// Enables the ship to move with mouse click controls
	/// </summary>
	public class ShipController : MonoBehaviour
	{
		public Camera shipOutCamera; // the main camera for this scene
		public float shipSpeed; // how quickly the ship moves across the scene
		public float sceneMargin; // the spacing between the ship and the canvas edges
		public float rotationSpeed; // the speed at which the ship rotates

		private bool m_mouseIsDown; // whether the InputMgr has had an Interact press but not yet a release

		#region Actions

		private System.Action shipInteractPressed;
		private System.Action shipInteractReleased;

		/// <summary>
		/// Tracks when interaction has started
		/// </summary>
		private void TrackInteractPressed()
		{
			m_mouseIsDown = true;
		}

		/// <summary>
		/// Tracks when interaction has ended
		/// </summary>
		private void TrackInteractReleased()
		{
			m_mouseIsDown = false;
		}

		/// <summary>
		/// Registers what the ship does/tracks when InputMgr interactions occur
		/// </summary>
		private void RegisterActions()
		{
			shipInteractPressed += TrackInteractPressed;
			shipInteractReleased += TrackInteractReleased;
			InputMgr.Register(InputMgr.OnInteractPressed, shipInteractPressed);
			InputMgr.Register(InputMgr.OnInteractReleased, shipInteractReleased);
		}

		#endregion

		#region Unity Callbacks

		// Awake is called when the script instance is being loaded (before Start)
		private void Awake()
		{
			// input does not start pressed down
			m_mouseIsDown = false;
		}

		// Start is called before the first frame update
		private void Start()
		{
			RegisterActions();
		}

		// Update is called once per frame
		private void Update()
		{
			if (m_mouseIsDown && InteractionIsInBounds())
			{
				// Ship moves when mouse is down
				MoveShip();
			}
		}

		#endregion

		#region Member Functions

		/// <summary>
		/// Moves the ship toward the mouse position
		/// </summary>
		public void MoveShip()
		{
			// find the mouse position relative to the canvas
			Vector2 mousePosRaw = shipOutCamera.ScreenToViewportPoint(InputMgr.Position);

			// extrapolate the mouse position into screen space
			Vector2 mouseScreenPos = new Vector2(
				mousePosRaw.x * Screen.width,
				mousePosRaw.y * Screen.height
				);

			// enforce scene margins
			if (mouseScreenPos.x < sceneMargin) { mouseScreenPos.x = sceneMargin; }
			if (mouseScreenPos.x > Screen.width - sceneMargin) { mouseScreenPos.x = Screen.width - sceneMargin; }

			if (mouseScreenPos.y < sceneMargin) { mouseScreenPos.y = sceneMargin; }
			if (mouseScreenPos.y > Screen.height - sceneMargin) { mouseScreenPos.y = Screen.height - sceneMargin; }

			// calculate the new location
			Vector2 newPos = Vector2.MoveTowards(this.transform.position, mouseScreenPos, shipSpeed * Time.deltaTime);

			// rotate the ship toward the new location
			// (implementation helped by the video Rotating in the Direction of Movement 2D, by Ketra Games,
			// found here: https://www.youtube.com/watch?v=gs7y2b0xthU)
			Vector2 moveDirection = newPos - (Vector2)(this.transform.position);
			if (moveDirection != Vector2.zero)
			{
				Quaternion toDirection = Quaternion.LookRotation(Vector3.forward, moveDirection);
				this.transform.rotation = Quaternion.RotateTowards(
					this.transform.rotation,
					toDirection,
					rotationSpeed * Time.deltaTime
					);
			}

			// move the ship toward the new location
			this.transform.position = newPos;
		}

		#endregion

		#region BoundsChecking

		/// <summary>
		/// Checks whether an interaction is within screen space
		/// </summary>
		/// <returns>true if an interaction is within screen space, false otherwise</returns>
		private bool InteractionIsInBounds()
		{
			// Horizontal
			if (InputMgr.Position.x < 0 || InputMgr.Position.x > Screen.width)
			{
				return false;
			}

			// Vertical
			if (InputMgr.Position.y < 0 || InputMgr.Position.y > Screen.height)
			{
				return false;
			}

			return true;
		}

		#endregion
	}

}
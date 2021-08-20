/*
 * Organization: Field Day Lab
 * Author(s): Levi Huillet
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck
{
	/// <summary>
	/// Enables the ship to move with interact (mouse, touch) controls
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class ShipController : MonoBehaviour
	{
		public Camera shipOutCamera; // the main camera for this scene

		[SerializeField]
		private float m_shipBaseSpeed; // how quickly the ship moves across the scene
		[SerializeField]
		private float m_distanceModifier; // how much interact distance from ship affects ship speed
		[SerializeField]
		private float m_maxSpeed; // the fastest this ship can move
		[SerializeField]
		private float m_rotationSpeed; // the speed at which the ship rotates
		[SerializeField]
		private float m_sceneMargin; // the spacing between the ship and the scene edges
		private float m_currSpeed; // how quickly the ship is moving this frame
		private bool m_interactIsActive; // whether the InputMgr has had an Interact press but not yet a release
		private Vector2 m_sceneDimensions; // the dimensions of the scene the ship finds itself in

		#region Actions

		private System.Action shipInteractPressed;
		private System.Action shipInteractReleased;

		/// <summary>
		/// Tracks when interaction has started
		/// </summary>
		private void TrackInteractPressed()
		{
			m_interactIsActive = true;
		}

		/// <summary>
		/// Tracks when interaction has ended
		/// </summary>
		private void TrackInteractReleased()
		{
			m_interactIsActive = false;
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
			m_interactIsActive = false;
		}

		// Start is called before the first frame update
		private void Start()
		{
			RegisterActions();
			m_sceneDimensions = ShipOutMgr.instance.GetTargetDimensions();
		}

		// Update is called once per frame
		private void Update()
		{
			if (m_interactIsActive && InteractionIsInBounds())
			{
				// Ship moves when input interaction is active
				MoveShip();
			}
		}

		#endregion

		#region Member Functions

		/// <summary>
		/// Moves the ship toward the interact position
		/// </summary>
		public void MoveShip()
		{
			Vector2 interactScreenPos = shipOutCamera.ScreenToWorldPoint(InputMgr.Position);

			// enforce scene margins
			EnforceMargins(ref interactScreenPos);

			// apply distance modifier (ship travels faster when the interact position is farther)
			float interactDistance = Vector2.Distance(interactScreenPos, this.transform.position);

			float rawSpeed = m_shipBaseSpeed + (interactDistance * m_distanceModifier);
			float correctedSpeed;

			if (rawSpeed > m_maxSpeed) { correctedSpeed = m_maxSpeed; }
			else { correctedSpeed = rawSpeed; }

			// calculate the new location
			Vector2 newPos = Vector2.MoveTowards(this.transform.position, interactScreenPos, correctedSpeed * Time.deltaTime);

			// save the current speed so the sonar can use it for randomization
			m_currSpeed = correctedSpeed;

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
					 m_rotationSpeed * Time.deltaTime
					);
			}

			// move the ship toward the new location
			this.transform.position = newPos;
		}

		/// <summary>
		/// Brings the ship's target position to the margin if it is beyond it
		/// </summary>
		/// <param name="interactScreenPos">the location of the interact position</param>
		private void EnforceMargins(ref Vector2 interactScreenPos)
		{
			// Horizontal
			if (interactScreenPos.x < m_sceneMargin)
			{
				// too far left
				interactScreenPos.x = m_sceneMargin;
			}
			else if (interactScreenPos.x > m_sceneDimensions.x - m_sceneMargin)
			{
				// too far right
				interactScreenPos.x = m_sceneDimensions.x - m_sceneMargin;
			}

			// Vertical
			if (interactScreenPos.y < m_sceneMargin)
			{
				// too far down
				interactScreenPos.y = m_sceneMargin;
			}
			else if (interactScreenPos.y > m_sceneDimensions.y - m_sceneMargin)
			{
				// too far up
				interactScreenPos.y = m_sceneDimensions.y - m_sceneMargin;
			}
		}

		/// <summary>
		/// Returns this ship's current speed
		/// </summary>
		/// <returns>this ship's current speed</returns>
		public float GetCurrSpeed()
		{
			return m_currSpeed;
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
			if (InputMgr.Position.x < 0 || InputMgr.Position.x > Screen.width) { return false; }

			// Vertical
			if (InputMgr.Position.y < 0 || InputMgr.Position.y > Screen.height) { return false; }

			return true;
		}

		/// <summary>
		/// Checks whether interaction is over a buoy
		/// </summary>
		/// <returns></returns>
		private bool InteractionIsOverBuoy()
		{
			return false;
		}

		/// <summary>
		/// Checks whether interaction is over a button or other UI
		/// </summary>
		/// <returns></returns>
		private bool InteractionIsOverUI()
		{
			return false;
		}

		#endregion
	}

}
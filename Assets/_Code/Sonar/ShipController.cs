﻿/*
 * Organization: Field Day Lab
 * Author(s): Levi Huillet
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shipwreck
{
	/// <summary>
	/// Enables the ship to move with interact (mouse, touch) controls
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
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
		[SerializeField]
		private float m_obstacleMargin; // the spacing between the ship and the buoy
		private float m_currSpeed; // how quickly the ship is moving this frame
		private bool m_interactIsActive; // whether the InputMgr has had an Interact press but not yet a release
		private Vector2 m_sceneDimensions; // the dimensions of the scene the ship finds itself in

		private AudioSource m_audioSrc; // for making ship movement sounds

		[SerializeField]
		private Transform m_shipShadow; // need to manually move and spin the shadow
		[SerializeField]
		private AudioData m_engineAudioData; // the sound to make
		[SerializeField]
		private float m_engineRevRate;
		[SerializeField]
		private float m_engineDieRate;

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

			m_audioSrc = GetComponent<AudioSource>();
			AudioSrcMgr.LoadAudio(m_audioSrc, m_engineAudioData);
			m_audioSrc.Play();
		}

		// Start is called before the first frame update
		private void Start()
		{
			RegisterActions();
			m_sceneDimensions = ShipOutMgr.instance.GetTargetDimensions();

			m_shipShadow.transform.position = new Vector3(transform.position.x - 0.2f, transform.position.y, m_shipShadow.transform.position.z);
			m_shipShadow.transform.rotation = transform.rotation;

		}

		// Update is called once per frame
		private void Update()
		{
			if (m_interactIsActive
				&& !UIMgr.IsOpen<UIPhoneNotif>()
				&& !UIMgr.IsOpen<UIRadioDialog>()
				//&& InteractionIsInBounds()
				&& !InteractionIsOverUI()
				&& !ShipOutMgr.instance.IsMessageShowing())
			{
				// Ship moves when input interaction is active
				MoveShip();
			}
			else
			{
				m_currSpeed = 0;
			}

			// make boat louder the faster it travels
			/*
			float volumeChange = ((m_currSpeed / (m_maxSpeed - 1f)) - m_audioSrc.volume);
			if (volumeChange < -.5 || m_currSpeed == 0) { volumeChange *= m_engineDieRate; } // shutting off engine is quicker than revving up
			m_audioSrc.volume += volumeChange * m_engineRevRate * Time.deltaTime;
			*/

			// TODO: smooth out engine sounds; break into rev, sustain, and die
			if (m_currSpeed > 0)
			{
				m_audioSrc.volume = 1;
			}
			else
			{
				m_audioSrc.volume = 0;
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

			// correct for buoy
			Vector2 obstacleCorrection = CorrectForObstacle(interactScreenPos);

			// apply distance modifier (ship travels faster when the interact position is farther)
			float interactDistance = Vector2.Distance(obstacleCorrection, this.transform.position);

			float rawSpeed = m_shipBaseSpeed + (interactDistance * m_distanceModifier);
			float correctedSpeed;

			if (rawSpeed > m_maxSpeed) { correctedSpeed = m_maxSpeed; }
			else { correctedSpeed = rawSpeed; }

			// calculate the new location
			Vector2 newPos = Vector2.MoveTowards(this.transform.position, obstacleCorrection, correctedSpeed * Time.deltaTime);

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
				m_shipShadow.rotation = transform.rotation;
			}

			// move the ship toward the new location
			this.transform.position = newPos;
			m_shipShadow.transform.position = newPos + new Vector2(-0.2f,0f);

			// adjust audio position
			m_audioSrc.panStereo = (this.transform.position.x * 100) / Screen.width;
		}

		private Vector2 CorrectForObstacle(Vector2 interactScreenPos)
		{
			// cast a ray from ship to position
			RaycastHit2D hit = Physics2D.Linecast(
				this.transform.position,
				interactScreenPos,
				1 << LayerMask.NameToLayer("Obstacle")
				);

			// if ray hits buoy (or other obstacle), find nearest point and stop there
			if (hit.collider != null)
			{
				Vector2 closestPoint = hit.collider.ClosestPoint(this.transform.position);
				// push the ship just beyond the bounds to orbit the obstacle(-ish)
				interactScreenPos = closestPoint
					//+ ((Vector2)this.transform.position - closestPoint).normalized
					- (closestPoint - (Vector2)this.transform.position).normalized
					* m_obstacleMargin;
			}
			
			return interactScreenPos;
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
			Vector2 interactScreenPos = shipOutCamera.ScreenToWorldPoint(InputMgr.Position);

			Collider2D collider = Physics2D.OverlapPoint(interactScreenPos);

			if (collider != null && collider.tag == "Buoy")
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Checks whether interaction is over a button or other UI
		/// </summary>
		/// <returns></returns>
		private bool InteractionIsOverUI()
		{
			return ShipOutMgr.instance.GetInteractIsOverUI();
		}

		#endregion
	}

}
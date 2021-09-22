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
	/// "Casts" the sonar by registering when this field collides with a sonar dot
	/// </summary>
	[RequireComponent(typeof(CapsuleCollider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(AudioSource))]
	public class SonarField : MonoBehaviour
	{
		public ShipController ship; // the ship casting this sonar field

		private AudioSource m_audioSrc; // for making sonar sounds
		[SerializeField]
		private AudioData m_sonarAudioData; // the sound to make

		#region Randomization

		/// <summary>
		/// To adjust the rate at which sonar dots appear, mess with the following fields.
		/// TODO: create a neater solution, where programmer can plug in
		/// something like: "1 in 2 at base speed, 1 in 40 at maxSpeed",
		/// and the variables will adjust themselves.
		/// </summary>
		[SerializeField]
		private float m_speedModifier;
		private static float RAND_START = 0.15f;
		private static float RAND_THRESHOLD = 1.5f;

		#endregion

		#region Unity Callbacks

		private void Awake()
		{
			m_audioSrc = GetComponent<AudioSource>();
			AudioSrcMgr.instance.InitializeAudio(m_audioSrc, m_sonarAudioData);
		}

		private void Start()
		{
			ShipOutMgr.instance.EnableSonar.AddListener(PlaySonar);
			ShipOutMgr.instance.DisableSonar.AddListener(StopSonar);
		}

		private void PlaySonar()
		{
			m_audioSrc.Play();
		}

		private void StopSonar()
		{
			m_audioSrc.Stop();
		}

		// Called when collides with another object
		void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.tag != "SonarDot")
			{
				return;
			}

			// generate random number modified by ship velocity
			float shipSpeed = ship.GetCurrSpeed();

			float modifiedSpeed = shipSpeed * m_speedModifier;

			// modifiedSpeed below threshold automatically detects sonar
			if (modifiedSpeed >= RAND_THRESHOLD)
			{
				float randNum = Random.Range(RAND_START, modifiedSpeed);

				if (randNum > RAND_THRESHOLD) { return; }
			}

			// if the number is low enough, ensure the collision is a sonar dot
			SonarDot sonarDot = other.gameObject.GetComponent<SonarDot>();
			if (sonarDot == null) { return; }

			// reveal the sonar dot
			sonarDot.Reveal();
		}

		#endregion
	}

}
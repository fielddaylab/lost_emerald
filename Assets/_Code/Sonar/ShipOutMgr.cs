/*
 * Organization: Field Day Lab
 * Author(s): Levi Huillet
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shipwreck
{
	/// <summary>
	/// Manages the ShipOut scene
	/// </summary>
	public class ShipOutMgr : MonoBehaviour
	{
		public static ShipOutMgr instance; // there can only be one
		public SonarDotMgr m_sdmgr; // the scene's SonarDotMgr

		[SerializeField]
		private Vector2 m_targetDimensions; // the dimensions of the scene
		[SerializeField]
		private float m_completionPercent; // the percent of solar dots a player must reveal before diving
		private int m_sonarProgress; // the number of dots that have been revealed
		private bool m_diveUnlocked; // whether the dive has been unlocked

		private static int DIM_TO_WORLD_PROP = 100; // the proportion of scene dimensions to world space is 100 pixels per unit

		private void Start()
		{
			// ensure there is only one ShipOutMgr at any given time
			if (ShipOutMgr.instance == null)
			{
				ShipOutMgr.instance = this;
			}
			else if (ShipOutMgr.instance != this)
			{
				Destroy(this.gameObject);
			}

			// convert dimensions to world space
			m_targetDimensions /= DIM_TO_WORLD_PROP;

			// sonar progress starts at 0
			m_sonarProgress = 0;

			// dive starts locked
			m_diveUnlocked = false;
		}

		/// <summary>
		/// Returns the scene's target dimensions
		/// </summary>
		/// <returns></returns>
		public Vector2 GetTargetDimensions()
		{
			return m_targetDimensions;
		}

		/// <summary>
		/// Increments the number of sonar dots that have been revealed, 
		/// then checks if the dive should be unlocked
		/// </summary>
		public void IncrementRevealCount()
		{
			m_sonarProgress = m_sonarProgress + 1;

			if (!m_diveUnlocked)
			{
				float percentComplete = ((float)m_sonarProgress / m_sdmgr.GetTargetNumDots()) * 100;

				if (percentComplete >= m_completionPercent)
				{
					Debug.Log("Dive unlocked!");
					m_diveUnlocked = true;
				}
			}
		}

		/// <summary>
		/// Loads the relevant scene
		/// </summary>
		/// <param name="sceneName"></param>
		public void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}

		/// <summary>
		/// Loads the given dive scene
		/// </summary>
		/// <param name="sceneName"></param>
		public void Dive(string sceneName)
		{
			if (m_diveUnlocked)
			{
				Debug.Log("Diving!");
			}
		}
	}
}
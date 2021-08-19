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
	/// 
	/// </summary>
	public class ShipOutMgr : MonoBehaviour
	{
		public static ShipOutMgr instance;
		public SonarDotMgr m_sdmgr;

		[SerializeField]
		private Vector2 m_targetDimensions; // the dimensions of the scene
		[SerializeField]
		private float m_completionPercent; // the percent of solar dots a player must reveal before diving
		private int m_sonarProgress; // the number of dots that have been revealed
		private bool m_diveUnlocked;

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

			m_sonarProgress = 0;

			m_diveUnlocked = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Vector2 GetTargetDimensions()
		{
			return m_targetDimensions;
		}

		/// <summary>
		/// 
		/// </summary>
		public void IncrementRevealCount()
		{
			m_sonarProgress = m_sonarProgress + 1;

			float percentComplete = ((float)m_sonarProgress / m_sdmgr.GetTargetNumDots()) * 100;

			if (percentComplete >= m_completionPercent)
			{
				Debug.Log("Dive unlocked!");
				m_diveUnlocked = true;
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
		/// 
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
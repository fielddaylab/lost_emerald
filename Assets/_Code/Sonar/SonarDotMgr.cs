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
	/// Given a ship's collider, generates sonar dots in the Unity editor
	/// </summary>
	public class SonarDotMgr : MonoBehaviour
	{
		public PolygonCollider2D shipCollider; // the collider of the ship to fill in with sonar dots
		[SerializeField]
		private GameObject m_sonarDotPrefab; // the prefab for a sonar dot
		[SerializeField]
		private GameObject m_sonarDotParentPrefab; // prefab for object that groups the generated dots
		private GameObject m_sonarDotParent; // // an empty object that groups the generated dots
		[SerializeField]
		private int m_targetNumDots;
		private List<GameObject> m_sonarDots; // all the sonar dots in the scene
		private List<Vector2> m_polygonPoints; // the coordinates of all sonar dots in the scene

		/// <summary>
		/// Generates SonarDots within the given PolygonCollider2D
		/// NOTE: currently, attempting to regenerate points after running the game creates a new set of points.
		/// The old sonarDots must be deleted manually.
		/// </summary>
		[ContextMenu("Regenerate Points")]
		private void RegeneratePoints()
		{
			if (m_sonarDots == null) { m_sonarDots = new List<GameObject>(); }

			if (m_polygonPoints == null) { m_polygonPoints = new List<Vector2>(); }

			DestroyImmediate(m_sonarDotParent);
			m_sonarDotParent = Instantiate(m_sonarDotParentPrefab, this.transform);

			m_sonarDots.Clear();
			m_polygonPoints.Clear();

			// Generate new dots

			// Define area in which random points will be generated
			Bounds shipBounds = shipCollider.bounds;

			float minX = shipBounds.center.x - shipBounds.extents.x;
			float maxX = shipBounds.center.x + shipBounds.extents.x;

			float minY = shipBounds.center.y - shipBounds.extents.y;
			float maxY = shipBounds.center.y + shipBounds.extents.y;

			// Randomly generate list of points inside polygon

			int numFoundDots = 0;
			float randomX;
			float randomY;
			Vector2 randomPoint;
			int numAttempts = 0;

			while (numFoundDots < m_targetNumDots)
			{
				randomX = Random.Range(minX, maxX);
				randomY = Random.Range(minY, maxY);
				randomPoint = new Vector2(randomX, randomY);
				if (shipCollider.OverlapPoint(randomPoint))
				{
					m_polygonPoints.Add(randomPoint);
					++numFoundDots;
				}
				++numAttempts;
				if (numAttempts > 1000)
				{
					Debug.Log("Failed to generate all dots. Number generated: " + numFoundDots);
					break;
				}
			}

			// Instantiate SonarDot Prefabs at each point

			foreach (Vector2 point in m_polygonPoints)
			{
				GameObject newDot = Instantiate(m_sonarDotPrefab, m_sonarDotParent.transform);
				newDot.transform.position = point;
				m_sonarDots.Add(newDot);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public int GetTargetNumDots()
		{
			return m_targetNumDots;
		}
	}
}

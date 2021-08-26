/*
 * Organization: Field Day Lab
 * Author(s): Levi Huillet
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Shipwreck
{
	/// <summary>
	/// Given a ship's collider, generates sonar dots in the Unity editor
	/// </summary>
	public class SonarDotGenerator : MonoBehaviour
	{
		public PolygonCollider2D shipCollider; // the collider of the ship to fill in with sonar dots
		[SerializeField]
		private int m_targetNumDots;
		private List<GameObject> m_sonarDots; // all the sonar dots in the scene
		private List<Vector2> m_polygonPoints; // the coordinates of all sonar dots in the scene

		[SerializeField]
		private ShipOutData m_dataToGenerateFor; // the ShipOutData that will store this new data

		/// <summary>
		/// Generates SonarDots within the given PolygonCollider2D
		/// </summary>
		[ContextMenu("Regenerate Points")]
		private void RegeneratePoints()
		{
			if (m_sonarDots == null) { m_sonarDots = new List<GameObject>(); }

			if (m_polygonPoints == null) { m_polygonPoints = new List<Vector2>(); }

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
			// int numAttempts = 0;

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
				// ++numAttempts;
			}

			m_dataToGenerateFor.SetSonarDots(m_polygonPoints);

#if UNITY_EDITOR
			EditorUtility.SetDirty(m_dataToGenerateFor);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
#endif

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

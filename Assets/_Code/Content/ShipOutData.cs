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
	/// variable state related to an individual ShipOut scene
	/// </summary>
	[CreateAssetMenu(fileName = "NewShipOutData", menuName = "Shipwrecks/ShipOut")]
	public class ShipOutData : ScriptableObject
	{
		public int ShipOutIndex
		{
			get { return m_shipOutIndex; }
		}

		public Vector2 WreckLocation
		{
			get { return m_wreckLocation; }
		}

		public Vector2 BuoyLocation
		{
			get { return m_buoyLocation; }
		}

		public string DiveDest
		{
			get { return m_diveDest; }
		}

		public List<Vector2> SonarPoints
		{
			get { return m_sonarPoints; }
		}

		public bool SonarImmediate {
			get { return m_sonarImmediate; }
		}

		public void SetSonarDots(List<Vector2> points)
		{
			m_sonarPoints = points;
		}

		[SerializeField]
		private int m_shipOutIndex;
		[SerializeField]
		private Vector2 m_wreckLocation;
		[SerializeField]
		private Vector2 m_buoyLocation;
		[SerializeField]
		private string m_diveDest;
		[SerializeField]
		private List<Vector2> m_sonarPoints;
		[SerializeField]
		private bool m_sonarImmediate = true;
	}
}

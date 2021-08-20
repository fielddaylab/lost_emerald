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

		public List<Vector2> SonarPoints
		{
			get { return m_sonarPoints; }
		}

		public void SetSonarDots(List<Vector2> points)
		{
			m_sonarPoints = points;
		}

		[SerializeField]
		private int m_shipOutIndex;
		[SerializeField]
		private List<Vector2> m_sonarPoints;
	}
}

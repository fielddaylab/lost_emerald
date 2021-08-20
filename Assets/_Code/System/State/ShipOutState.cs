using BeauData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck
{
	/// <summary>
	/// Tracks the state of a single ShipOut scene
	/// </summary>
	public class ShipOutState : ISerializedObject, ISerializedVersion
	{
		private ShipOutData m_shipOutData;

		private bool m_diveUnlocked; // whether this dive has been unlocked

		/// <summary>
		/// Initializes this state
		/// </summary>
		public ShipOutState()
		{
			// no dives are unlocked before completing sonar
			m_diveUnlocked = false;
		}

		public ushort Version
		{
			get { return 1; }
		}

		public void AssignShipOutData(ShipOutData data)
		{
			m_shipOutData = data;
		}

		public bool IsDiveUnlocked()
		{
			return m_diveUnlocked;
		}

		public void Serialize(Serializer ioSerializer)
		{
			ioSerializer.Serialize("diveUnlocked", ref m_diveUnlocked);
		}

		public bool UnlockDive()
		{
			m_diveUnlocked = true;
			if (m_diveUnlocked)
			{
				return false;
			}
			else
			{
				m_diveUnlocked = true;
				return true;
			}
		}
	}
}
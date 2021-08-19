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
	/// A dot that is sometimes detected when the ship passes over it
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class SonarDot : MonoBehaviour
	{
		private SpriteRenderer m_sr;

		#region Unity Callbacks

		// Awake is called when the script instance is being loaded (before Start)
		private void Awake()
		{
			m_sr = this.GetComponent<SpriteRenderer>();
		}

		#endregion

		#region Member Functions

		/// <summary>
		/// Reveals this dot and prevents the ship from detecting it again
		/// </summary>
		public void Reveal()
		{
			m_sr.enabled = true;

			// prevent future OnCollisionEnter2D's from occuring
			this.GetComponent<BoxCollider2D>().enabled = false;
		}

		#endregion
	}
}

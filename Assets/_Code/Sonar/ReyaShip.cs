using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class ReyaShip : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_shipBubble; // the bubble indicating reya has something to say

		void OnCollisionEnter2D(Collision2D other)
		{
			// player ship enters bounds
			GameMgr.RunTrigger(GameTriggers.OnFindReya);

			// remove the ship speech bubble
			m_shipBubble.SetActive(false);
		}

		public void ActivateBubble()
		{
			m_shipBubble.SetActive(true);
		}
	}

}
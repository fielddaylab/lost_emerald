using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class ReyaShip : MonoBehaviour
	{
		void OnCollisionEnter2D(Collision2D other)
		{
			// player ship enters bounds
			GameMgr.RunTrigger(GameTriggers.OnFindReya);
		}
	}

}
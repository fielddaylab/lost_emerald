using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shipwreck
{
	/// <summary>
	/// Tracks when interaction hovers over UI, which prevents the ship from moving
	/// </summary>
	public class ShipOutUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
	{
		public void OnPointerEnter(PointerEventData eventData)
		{
			ShipOutMgr.instance.SetInteractIsOverUI(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			ShipOutMgr.instance.SetInteractIsOverUI(false);
		}
	}


}


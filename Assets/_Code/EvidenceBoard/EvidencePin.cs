using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shipwreck {

	public class EvidencePin : MonoBehaviour, IPointerDownHandler {

		public Action<EvidencePin> OnPressed;
	
		public void OnPointerDown(PointerEventData eventData) {
			OnPressed?.Invoke(this);
		}



	}

}
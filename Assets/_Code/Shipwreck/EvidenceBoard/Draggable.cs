using UnityEngine;

namespace Shipwreck {

	public class Draggable : MonoBehaviour {

		public bool IsDroppable { 
			get { return m_isDroppable; } 
		}

		[SerializeField]
		private bool m_isDroppable = false;


		public void OnPickup() {

		}
		public void OnRelease() {

		}


	}

}
using BeauRoutine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {


	public class StickyNote : MonoBehaviour {

		[SerializeField]
		private Image m_image = null;
		[SerializeField]
		private TextMeshProUGUI m_text = null;

		private Routine m_routine;

		public void SetColor(Color imageColor) {
			m_routine.Replace(this, m_image.ColorTo(imageColor,0.2f));
		}

	}

}



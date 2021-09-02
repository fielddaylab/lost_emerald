using BeauRoutine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {


	public class StickyNote : MonoBehaviour {

		public RectTransform RectTransform {
			get { 
				if (m_rectTransform == null) {
					m_rectTransform = (RectTransform)transform;
				}
				return m_rectTransform; 
			}
		}

		[SerializeField]
		private Image m_image = null;
		[SerializeField]
		private TextMeshProUGUI m_text = null;

		private Routine m_routine;
		private RectTransform m_rectTransform;

		public void SetText(string text) {
			m_text.text = text;
		}
		public void SetColor(Color imageColor) {
			m_routine.Replace(this, m_image.ColorTo(imageColor,0.2f));
		}

	}

}



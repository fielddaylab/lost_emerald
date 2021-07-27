using BeauRoutine;
using PotatoLocalization;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {


	[RequireComponent(typeof(RectTransform))]
	public class EvidenceLabel : MonoBehaviour {

		public LocalizationKey Key { 
			get { return m_text.Key; } 
			set { m_text.Key = value; }
		}
		public RectTransform RectTransform {
			get {
				if (m_rectTransform == null) {
					m_rectTransform = GetComponent<RectTransform>();
				}
				return m_rectTransform;
			}
		}

		[SerializeField]
		private LocalizedTextUGUI m_text;
		[SerializeField]
		private Image m_image;

		private RectTransform m_rectTransform;
		private Routine m_routine;

		public void SetColor(Color color) {
			m_routine.Replace(this, m_image.ColorTo(color, 0.2f));
		}
	}

}
using PotatoLocalization;
using UnityEngine;

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

		private RectTransform m_rectTransform;
	}

}
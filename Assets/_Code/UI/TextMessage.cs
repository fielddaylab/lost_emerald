using BeauRoutine;
using PotatoLocalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class TextMessage : MonoBehaviour {

		public Sprite Icon {
			get {
				if (m_icon == null) {
					return null;
				} else {
					return m_icon.sprite;
				}
			}
			set {
				if (m_icon != null) {
					m_icon.sprite = value;
				}
			}
		}
		public string Text {
			get { return m_bodyText.text; }
			set { m_bodyText.text = value; }
		}

		[SerializeField]
		private Image m_icon = null;
		[SerializeField]
		private TextMeshProUGUI m_bodyText = null;


		public void OnAwake() {
			transform.localScale = Vector3.zero;
			Routine.Start(this, transform.ScaleTo(1f, 0.25f).Ease(Curve.QuadInOut));
		}


	}


}
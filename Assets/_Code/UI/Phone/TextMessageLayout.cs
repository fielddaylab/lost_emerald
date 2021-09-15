using BeauRoutine;
using PotatoLocalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class TextMessageLayout : MonoBehaviour {
		[SerializeField]
		private Image m_icon = null;
		[SerializeField]
		private Graphic m_iconRoot = null;
		[SerializeField]
		private Graphic m_iconBack = null;
		[SerializeField]
		private HorizontalLayoutGroup m_layout = null;
		[SerializeField]
		private Graphic m_tail = null;
		[SerializeField]
		private Graphic m_contentOutline = null;
		[SerializeField]
		private Graphic m_contentBackground = null;

		public void Awake() {
			transform.localScale = Vector3.zero;
			Routine.Start(this, transform.ScaleTo(1f, 0.25f).Ease(Curve.QuadInOut));
		}

		public void Populate(CharacterData character) {
			Sprite icon = character.TextingIcon;

			m_contentOutline.color = character.TextingColor;
			m_contentBackground.color = character.TextingBackgroundColor;

			if (icon == null) {
				m_iconRoot.gameObject.SetActive(false);
				m_tail.gameObject.SetActive(false);
				m_layout.childAlignment = TextAnchor.UpperRight;
			} else {
				m_iconRoot.color = character.TextingColor;
				m_iconBack.color = character.TextingBackgroundColor;
				m_icon.sprite = icon;
				m_tail.color = character.TextingBackgroundColor;
				m_iconRoot.gameObject.SetActive(true);
				m_tail.gameObject.SetActive(true);
				m_layout.childAlignment = TextAnchor.UpperLeft;
			}
		}
	}

}
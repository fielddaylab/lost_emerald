using BeauRoutine;
using PotatoLocalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class TextMessageText : MonoBehaviour {

		[SerializeField]
		private TextMessageLayout m_layout = null;
		[SerializeField]
		private TextMeshProUGUI m_bodyText = null;

		public void Populate(CharacterData character, string text) {
			m_layout.Populate(character);
			m_bodyText.SetText(text);
			m_bodyText.color = character.TextingColor;
		}
	}


}
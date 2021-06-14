using PotatoLocalization;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Shipwreck {

	public class TextMessagePanel : MonoBehaviour, IMessagePanel {

		[SerializeField]
		private TextMessage m_theirPrefab = null;
		[SerializeField]
		private TextMessage m_yourPrefab = null;
		[SerializeField]
		private RectTransform m_content = null;
		[SerializeField]
		private LocalizedTextUGUI m_conversationPartner = null;
		[SerializeField]
		private ScrollRect m_scrollRect = null;
		[SerializeField]
		private Button m_continueButton = null;

		private Sprite m_textingIcon = null;

		public void SetConversationPartner(CharacterData partner) {
			m_conversationPartner.Key = partner.DisplayName;
		}
		public void SetSpeaker(CharacterData speaker) {
			m_textingIcon = speaker.TextingIcon;
		}

		public void Show() {
			gameObject.SetActive(true);
		}
		public void Hide() {
			gameObject.SetActive(false);
			for (int ix = m_content.childCount - 1; ix >= 0; ix--) {
				Destroy(m_content.GetChild(ix).gameObject);
			}
		}

		public void PrepareLine(string text) {
			TextMessage prefab = m_theirPrefab;
			if (m_textingIcon == null) {
				prefab = m_yourPrefab;
			}
			TextMessage obj = Instantiate(prefab, m_content);
			obj.Icon = m_textingIcon;
			obj.Text = text;
		}

		public IEnumerator TypeLine(uint visibleCharacterCount) {
			yield return null;
		}

		public IEnumerator CompleteLine() {
			bool isComplete = false;
			UnityAction handler = () => {
				isComplete = true;
			};
			m_continueButton.onClick.AddListener(handler);
			while (!isComplete) {
				yield return null;
			}
			m_continueButton.onClick.RemoveListener(handler);
		}
	}

}
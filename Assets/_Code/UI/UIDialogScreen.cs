using BeauUtil;
using BeauUtil.Tags;
using Leaf.Defaults;
using PotatoLocalization;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIDialogScreen : UIDialogueBase {


		[SerializeField]
		private float m_typeSpeed = 10f;
		[SerializeField]
		private LocalizedTextUGUI m_speakerName = null;
		[SerializeField]
		private TextMeshProUGUI m_textBox = null; //todo: switch to localized text
		[SerializeField]
		private Image m_portrait = null;
		[SerializeField]
		private Button m_continueButton = null;

		

		public void SetConversationPartner(CharacterData partner) {
			m_portrait.sprite = partner.Portrait;
		}
		public void SetSpeaker(CharacterData speaker) {
			m_speakerName.Key = speaker.DisplayName;
		}


		public void Show() {
			gameObject.SetActive(true);
		}
		public void Hide() {
			gameObject.SetActive(false);
		}


		protected override IEnumerator ShowRoutine() {
			throw new System.NotImplementedException();
		}

		protected override IEnumerator HideRoutine() {
			throw new System.NotImplementedException();
		}
		protected override void OnPrepareLine(TagString inString) {
			m_textBox.text = inString.RichText.ToString();
			m_textBox.maxVisibleCharacters = 0;
		}

		public override IEnumerator TypeLine(TagString inString, TagTextData inType) {
			uint visibleCharacterCount = inType.VisibleCharacterCount;
			float timeWaited = 0f;
			bool skipped = false;
			UnityAction action = () => {
				skipped = true;
			};
			m_continueButton.onClick.AddListener(action);
			while (visibleCharacterCount > 0 && skipped == false) {
				if (timeWaited >= 1f) {
					m_textBox.maxVisibleCharacters += (int)timeWaited;
					visibleCharacterCount -= (uint)timeWaited;
					timeWaited -= timeWaited - (timeWaited % 1f);
				}
				yield return null;
				timeWaited += Time.deltaTime * m_typeSpeed;
			}
			m_continueButton.onClick.RemoveAllListeners();
		}

		public override IEnumerator CompleteLine() {
			bool completed = false;
			UnityAction action = () => {
				completed = true;
			};
			m_continueButton.onClick.AddListener(action);
			while (!completed) {
				yield return null;
			}
			m_continueButton.onClick.RemoveAllListeners();
		}

		protected override void OnSetSpeaker(CharacterData speaker) {
			//throw new System.NotImplementedException();
		}
	}

}
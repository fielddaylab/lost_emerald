using BeauRoutine;
using BeauUtil;
using BeauUtil.Tags;
using Leaf.Defaults;
using PotatoLocalization;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIRadioDialog : UIDialogueBase {

		#region Inspector

		[SerializeField]
		private float m_typeSpeed = 10f;
		[SerializeField]
		private LocalizedTextUGUI m_speakerName = null;
		[SerializeField]
		private Graphic m_speakerNameBackground = null;
		[SerializeField]
		private TextMeshProUGUI m_textBox = null;
		[SerializeField]
		private Graphic m_textBoxBackground = null;
		[SerializeField]
		private Graphic m_textBoxOutline = null;
		[SerializeField]
		private LayoutGroup m_layout = null;
		[SerializeField]
		private Image m_icon = null;
		[SerializeField]
		private Graphic m_iconOutline = null;
		[SerializeField]
		private Button m_continueButton = null;

		#endregion // Inspector

		#region UIBase

		protected override void OnShowStart() {
			base.OnShowStart();
			CanvasGroup.alpha = 0;
			m_textBox.text = string.Empty;

			GameMgr.Events.Dispatch(GameEvents.DialogOpened);
			UIMgr.Close<UIPhone>();
			UIMgr.Close<UIModalOverlay>();
		}

		protected override void OnHideCompleted() {
			base.OnHideCompleted();

			GameMgr.Events.Dispatch(GameEvents.DialogClosed);
			GameMgr.RunTrigger(GameTriggers.OnDialogClosed);
		}

		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1, 0.2f);
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0, 0.2f);
		}

		#endregion // UIBase

		#region Dialog

		public override void PrepareNode(ScriptNode node) {
			AssignPartner(GameDb.GetCharacterData(node.ContactId));
		}

		protected override void AssignPartner(CharacterData character) {
			m_icon.sprite = character.TextingIcon;
			m_iconOutline.color = character.DialogTextColor;
		}

		protected override void OnPrepareLine(TagString inString) {
			if (inString.RichText.Length > 0) {
				m_textBox.text = inString.RichText.ToString();
				m_textBox.maxVisibleCharacters = 0;
				m_layout.ForceRebuild();
			}
		}

		public override IEnumerator TypeLine(TagString inString, TagTextData inType) {
			uint visibleCharacterCount = inType.VisibleCharacterCount;
			float timeToWait = 0f;
			bool skipped = false;

			UnityAction action = () => {
				skipped = true;
			};
			m_continueButton.onClick.AddListener(action);

			while (visibleCharacterCount > 0 && skipped == false) {
				if (timeToWait >= 0) {
					timeToWait -= Time.deltaTime * m_typeSpeed;
				}
				while(timeToWait < 0 && visibleCharacterCount > 0) {
					m_textBox.maxVisibleCharacters++;
					visibleCharacterCount--;
					char c = inString.VisibleText[m_textBox.maxVisibleCharacters - 1];
					switch(c) {
						case ' ':
							timeToWait += 0.05f;
							break;
						case '.':
						case '!':
						case '?':
							timeToWait += 0.2f;
							break;
						case ',':
							timeToWait += 0.08f;
							break;
						default:
							timeToWait += 0.03f;
							break;
					}
				}
				yield return null;
			}

			if (skipped) {
				m_textBox.maxVisibleCharacters += (int) visibleCharacterCount;
			}

			m_continueButton.onClick.RemoveAllListeners();
		}

		public override IEnumerator CompleteLine() {
			yield return m_continueButton.onClick.WaitForInvoke();
		}

		protected override void OnSetSpeaker(CharacterData speaker) {
			m_speakerName.Key = speaker.DisplayName;
			m_textBox.color = speaker.DialogTextColor;
			m_textBoxBackground.color = speaker.DialogBackgroundColor;
			m_textBoxOutline.color = speaker.DialogTextColor;
			m_speakerName.GetComponent<Graphic>().color = speaker.DialogBackgroundColor;
			m_speakerNameBackground.color = speaker.DialogTextColor;
		}

		protected override IEnumerator OnShowImage(Sprite image) {
			throw new NotSupportedException();
		}

		protected override IEnumerator OnHideImage() {
			throw new NotSupportedException();
		}

		protected override void AssignBackground(Sprite background) {
			throw new NotSupportedException();
		}

		#endregion // Dialog
	}

}
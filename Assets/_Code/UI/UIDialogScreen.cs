using BeauRoutine;
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
		private Image m_portrait = null;
		[SerializeField]
		private Image m_background = null;
		[SerializeField]
		private Image m_image = null;
		[SerializeField]
		private Button m_continueButton = null;

		#endregion // Inspector

		#region UIBase

		protected override void OnShowStart() {
			base.OnShowStart();
			CanvasGroup.alpha = 0;
			m_textBox.text = string.Empty;
			m_image.gameObject.SetActive(false);

			GameMgr.Events.Dispatch(GameEvents.DialogOpened);
			UIMgr.Close<UIPhone>();
			UIMgr.Close<UIPhoneNotif>();
		}

		protected override void OnHideCompleted() {
			base.OnHideCompleted();

			GameMgr.Events.Dispatch(GameEvents.DialogClosed);
			using (var table = TempVarTable.Alloc()) {
				GameMgr.RunTrigger(GameTriggers.OnDialogClosed, table);
			}

			//UIPhoneNotif.AttemptReopen();
		}

		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1, 0.2f);
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0, 0.2f);
		}

		#endregion // UIBase

		#region Dialog

		protected override void AssignPartner(CharacterData character) {
			AssignSpritePreserveAspect(m_portrait, character.Portrait, Axis.Y);
			AssignSpritePreserveAspect(m_background, character.Background, Axis.Y);
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
			m_image.gameObject.SetActive(true);
			m_image.SetAlpha(0);
			AssignSpritePreserveAspect(m_image, image, Axis.Y);
			yield return m_image.FadeTo(1, 0.1f);
		}

		protected override IEnumerator OnHideImage() {
			yield return m_image.FadeTo(0, 0.1f);
			m_image.gameObject.SetActive(false);
		}

		#endregion // Dialog
	}

}
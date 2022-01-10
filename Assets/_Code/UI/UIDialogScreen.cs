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
		private Image m_icon = null;
		[SerializeField]
		private GameObject m_iconGroup = null;
		[SerializeField]
		private Graphic m_iconOutline = null;
		[SerializeField]
		private Graphic m_iconBackground = null;

		[SerializeField]
		private LayoutGroup m_layout = null;
		//[SerializeField]
		//private Image m_portrait = null;
		[SerializeField]
		private Image m_background = null;
		[SerializeField]
		private Image m_image = null;
		[SerializeField]
		private Button m_continueButton = null;
		[SerializeField]
		private RectTransform m_displayContainer = null;

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
			UIMgr.Close<UIModalOverlay>();
		}

		protected override void OnHideCompleted() {
			base.OnHideCompleted();

			GameMgr.Events.Dispatch(GameEvents.DialogClosed);
			GameMgr.RunTrigger(GameTriggers.OnDialogClosed);

			//UIPhoneNotif.AttemptReopen();
		}

		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1, 0.2f);
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0, 0.2f);
		}

		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}

		#endregion // UIBase

		#region Dialog

		protected override void AssignPartner(CharacterData character) {
			//AssignSpritePreserveAspect(m_portrait, character.Portrait, Axis.Y);
		}
		protected override void AssignBackground(Sprite background) {
			AssignSpritePreserveAspect(m_background, background, Axis.Y);
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
			m_continueButton.onClick.RemoveListener(ClickSound);
			m_continueButton.onClick.RemoveListener(LogConversationClick);
			m_continueButton.onClick.AddListener(SkipSound);
			m_continueButton.onClick.AddListener(action);

			//AudioSrcMgr.instance.StartLineAudio(DialogAudioMgr.Type.phone);

			while (visibleCharacterCount > 0 && !skipped) {
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

			//AudioSrcMgr.instance.EndLineAudio();

			m_continueButton.onClick.RemoveAllListeners();
			m_continueButton.onClick.AddListener(ClickSound);
			m_continueButton.onClick.AddListener(LogConversationClick);
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

			if (speaker.TextingIcon != null) {
				m_icon.sprite = speaker.TextingIcon;
				m_iconOutline.color = speaker.DialogTextColor;
				m_iconBackground.color = speaker.DialogBackgroundColor;
				m_iconGroup.SetActive(true);
			} else {
				m_iconGroup.SetActive(false);
			}
		}

		protected override IEnumerator OnShowImage(Sprite image) {
			//m_image.sprite = image;
			m_image.gameObject.SetActive(true);
			//m_image.SetAlpha(0);
			AssignSpritePreserveAspect(m_image, image, Axis.Y);
			m_image.transform.localScale = Vector3.zero;
			m_image.transform.localPosition = Vector3.zero;
			yield return m_image.transform.ScaleTo(1f, 0.35f, Axis.XY).Ease(Curve.BackOut);
			//yield return m_image.FadeTo(1, 0.1f);
		}

		protected override IEnumerator OnHideImage() {
			//yield return m_image.FadeTo(0, 0.1f);
			yield return m_image.transform.ScaleTo(0f, 0.35f, Axis.XY).Ease(Curve.BackIn);
			m_image.gameObject.SetActive(false);
		}

		protected override IEnumerator OnShowObject(GameObject prefab) {
			GameObject obj = Instantiate(prefab, m_displayContainer);
			obj.transform.localScale = Vector3.zero;
			obj.transform.localPosition = Vector3.zero;
			yield return obj.transform.ScaleTo(1f, 0.35f, Axis.XY).Ease(Curve.BackOut);
		}
		protected override IEnumerator OnHideObject() {
			for (int index = 0; index < m_displayContainer.childCount; index++) {
				yield return m_displayContainer.GetChild(index).transform.ScaleTo(0f, 0.35f, Axis.XY).Ease(Curve.BackIn);
			}
			for (int index = 0; index < m_displayContainer.childCount; index++) {
				Destroy(m_displayContainer.GetChild(index).gameObject);
			}
		}

		protected override IEnumerator OnShowEvidence(EvidenceGroup prefab) {
			EvidenceGroup group = Instantiate(prefab, m_displayContainer);
			group.transform.localScale = new Vector3(0f,0f,1f);
			group.transform.localPosition = Vector3.zero;
			group.RemoveNodes();
			yield return group.transform.ScaleTo(prefab.PopupScale, 0.35f, Axis.XY).Ease(Curve.BackOut);
		}
		protected override IEnumerator OnHideEvidence() {
			for (int index = 0; index < m_displayContainer.childCount; index++) {
				yield return m_displayContainer.GetChild(index).transform.ScaleTo(0f, 0.35f, Axis.XY).Ease(Curve.BackIn);
			}
			for (int index = 0; index < m_displayContainer.childCount; index++) {
				Destroy(m_displayContainer.GetChild(index).gameObject);
			}
		}


		private void ClickSound()
		{
			AudioSrcMgr.instance.PlayOneShot("click_dialog_continue");
		}

		private void SkipSound()
		{
			AudioSrcMgr.instance.PlayOneShot("click_dialog_skip");
		}

		private void LogConversationClick() {
			GameMgr.Events.Dispatch(GameEvents.ConversationClick, Logging.EventData.ClickAction.Continue);
		}


		#endregion // Dialog
	}

}
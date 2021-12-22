using BeauRoutine;
using PotatoLocalization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UITitleOptions : UIBase {

		[SerializeField]
		private RectTransform m_grouper = null;
		[SerializeField]
		private Image m_overlay = null;
		[SerializeField]
		private Button m_buttonClose = null;
		[SerializeField]
		private Toggle m_toggleSound = null;
		[SerializeField]
		private Toggle m_toggleFullscreen = null;
		[SerializeField]
		private Button m_buttonEnglish = null;
		[SerializeField]
		private Button m_buttonEspanol = null;

		protected override void OnShowStart() {
			base.OnShowStart();
			m_toggleSound.isOn = !AudioSrcMgr.instance.IsMute();
			m_toggleFullscreen.isOn = Screen.fullScreen;

			m_buttonClose.onClick.AddListener(HandleClose);
			m_toggleSound.onValueChanged.AddListener(HandleSound);
			m_toggleFullscreen.onValueChanged.AddListener(HandleFullscreen);
			m_buttonEnglish.onClick.AddListener(HandleEnglish);
			m_buttonEspanol.onClick.AddListener(HandleEspanol);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_buttonClose.onClick.RemoveListener(HandleClose);
			m_toggleSound.onValueChanged.RemoveListener(HandleSound);
			m_toggleFullscreen.onValueChanged.RemoveListener(HandleFullscreen);
			m_buttonEnglish.onClick.RemoveListener(HandleEnglish);
			m_buttonEspanol.onClick.RemoveListener(HandleEspanol);
		}


		private void HandleClose() {
			AudioSrcMgr.instance.PlayOneShot("click_map_close");
			UIMgr.Close(this);
		}
		private void HandleSound(bool value) {
			AudioSrcMgr.instance.MuteAudio(!value);
			AudioSrcMgr.instance.PlayOneShot("click_contact");
		}
		private void HandleFullscreen(bool value) {
			Screen.fullScreen = value;
			AudioSrcMgr.instance.PlayOneShot("click_contact");
		}
		private void HandleEnglish() {
			LocalizationMgr.SetLanguage(new LanguageCode("en"));
			AudioSrcMgr.instance.PlayOneShot("click_contact");
		}
		private void HandleEspanol() {
			LocalizationMgr.SetLanguage(new LanguageCode("es"));
			AudioSrcMgr.instance.PlayOneShot("click_contact");
		}


		protected override IEnumerator HideRoutine() {
			yield return Routine.Combine(
				m_overlay.FadeTo(0f, 0.25f),
				m_grouper.AnchorPosTo(620f, 0.2f, Axis.Y)
			);
		}

		protected override IEnumerator ShowRoutine() {
			m_overlay.color = new Color(0f, 0f, 0f, 0f);
			m_grouper.anchoredPosition = new Vector2(0f, 620f);
			yield return Routine.Combine(
				m_overlay.FadeTo(0.6f, 0.25f),
				m_grouper.AnchorPosTo(0f, 0.2f, Axis.Y)
			);
		}

		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}

	}

}
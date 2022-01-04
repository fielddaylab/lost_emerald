using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UITitleScreen : UIBase {

		[SerializeField]
		private RectTransform m_grouper = null;
		[SerializeField]
		private Image m_overlay = null;
		[SerializeField]
		private CanvasGroup m_buttonGroup = null;
		[SerializeField]
		private Button m_buttonNewGame = null;
		[SerializeField]
		private Button m_buttonLoadLevel = null;
		[SerializeField]
		private Button m_buttonCredits = null;
		[SerializeField]
		private Button m_buttonOptions = null;

		private Routine m_introPan;

		private void OnEnable() {
			m_buttonNewGame.onClick.AddListener(HandleNewGameButton);
			m_buttonCredits.onClick.AddListener(HandleCredits);
			m_buttonLoadLevel.onClick.AddListener(HandleLoadLevel);
			m_buttonOptions.onClick.AddListener(HandleOptions);
		}
		private void OnDisable() {
			m_buttonNewGame.onClick.RemoveListener(HandleNewGameButton);
			m_buttonCredits.onClick.RemoveListener(HandleCredits);
			m_buttonLoadLevel.onClick.RemoveListener(HandleLoadLevel);
			m_buttonOptions.onClick.RemoveListener(HandleOptions);
		}

		protected override void OnShowStart() {
			base.OnShowStart();
			m_overlay.color = Color.black;
			m_buttonGroup.interactable = false;
			m_buttonGroup.alpha = 0f;
			m_grouper.anchoredPosition = new Vector2(0f, 880f);
			m_introPan.Replace(this, IntroPanRoutine());
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_buttonGroup.interactable = false;
		}

		private IEnumerator IntroPanRoutine() {
			yield return m_grouper.AnchorPosTo(0f, 8f, Axis.Y).Ease(Curve.QuadInOut);
			yield return m_buttonGroup.FadeTo(1f, 0.3f);
			m_buttonGroup.interactable = true;
			CanvasGroup.interactable = true;
		}

		protected override IEnumerator HideRoutine() {
			yield return m_overlay.FadeTo(1f, 0.3f);
		}

		protected override IEnumerator ShowRoutine() {
			yield return m_overlay.FadeTo(0f, 1f);
		}

		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}

		

		private void StartGame() {
			AudioSrcMgr.instance.PlayOneShot("click_new_game");
			GameMgr.UnlockContact("dad");
			UIMgr.CloseThenCall<UITitleScreen>(() => {
				GameMgr.MarkTitleScreenComplete();
				UIMgr.Open<UIOfficeScreen>();
				AudioSrcMgr.instance.PlayAudio("office_music", true);
			});
		}

		private void HandleNewGameButton() {
			GameMgr.ClearState();
			StartGame();
		}

		private void HandleCredits() {
			AudioSrcMgr.instance.PlayOneShot("click_contact");
			UIMgr.CloseThenOpen<UITitleScreen, UITitleCredits>();
		}

		private void HandleLoadLevel() {
			AudioSrcMgr.instance.PlayOneShot("click_contact");
			UIMgr.Open<UITitleUnlocks>();
		}
		private void HandleOptions() {
			AudioSrcMgr.instance.PlayOneShot("click_contact");
			UIMgr.Open<UITitleOptions>();
		}
		
	}

}


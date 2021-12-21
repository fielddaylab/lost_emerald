using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UITitleScreen : UIBase {

		[SerializeField]
		private RectTransform m_grouper = null;
		[SerializeField]
		private Button m_buttonNewGame = null;
		[SerializeField]
		private Button m_buttonLoadLevel = null;
		[SerializeField]
		private Button m_buttonCredits = null;

		private void OnEnable() {
			m_buttonNewGame.onClick.AddListener(HandleNewGameButton);
			m_buttonCredits.onClick.AddListener(HandleCredits);
			m_buttonLoadLevel.onClick.AddListener(HandleLoadLevel);
		}
		private void OnDisable() {
			m_buttonNewGame.onClick.RemoveListener(HandleNewGameButton);
			m_buttonCredits.onClick.RemoveListener(HandleCredits);
			m_buttonLoadLevel.onClick.RemoveListener(HandleLoadLevel);
		}

		protected override void OnShowStart() {
			base.OnShowStart();
			CanvasGroup.alpha = 0;
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}

		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
			CanvasGroup.interactable = true;
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
			UIMgr.CloseThenOpen<UITitleScreen, UITitleCredits>();
		}

		private void HandleLoadLevel() {
			UIMgr.Open<UITitleUnlocks>();
		}

		
	}

}


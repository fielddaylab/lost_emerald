using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UITitleScreen : UIBase {

		[SerializeField]
		private Button m_newGameButton = null;
		[SerializeField]
		private Button m_unlockButton = null;

		private void OnEnable() {
			m_newGameButton.onClick.AddListener(HandleNewGame);
			m_unlockButton.onClick.AddListener(HandleUnlock);
		}
		private void OnDisable() {
			m_newGameButton.onClick.RemoveListener(HandleNewGame);
			m_unlockButton.onClick.RemoveListener(HandleUnlock);
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

		private void HandleNewGame() {
			UIMgr.CloseThenCall<UITitleScreen>(() => {
				UIMgr.Open<UIOfficeScreen>();
				UIPhoneNotif.AttemptReopen();
			});
		}
		private void HandleUnlock() {
			GameMgr.UnlockLevel(1);
			GameMgr.UnlockEvidence("LV1-Root");
			GameMgr.UnlockEvidence("LV1-Transcript-Lou");
		}


	}

}


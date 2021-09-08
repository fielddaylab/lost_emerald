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
				GameMgr.MarkTitleScreenComplete();
				UIMgr.Open<UIOfficeScreen>();
			});
		}
		private void HandleUnlock() {
			GameMgr.UnlockLevel(1);
			GameMgr.UnlockEvidence(1,"LV1-Root");
			GameMgr.UnlockEvidence(1,"LV1-Transcript-Lou");
			GameMgr.UnlockEvidence(1,"LV1-Photo-Above");
			GameMgr.UnlockEvidence(1,"LV1-Photo-Name");
			GameMgr.UnlockEvidence(1,"LV1-Photo-Artifact");
			GameMgr.UnlockEvidence(1,"LV1-Card-Types");
			GameMgr.UnlockEvidence(1,"LV1-Table-Wrecks");
			GameMgr.UnlockEvidence(1,"LV1-Article-Sinking");
			GameMgr.UnlockContact("dad");
			GameMgr.UnlockContact("lou");
			GameMgr.UnlockContact("amy");
			GameMgr.RecordNodeVisited("level01.mom-meet", "mom");
			GameMgr.RecordNodeVisited("level01.dad-meet", "dad");
			GameMgr.RecordNodeVisited("level01.lou-meet", "lou");
			GameMgr.RecordNodeVisited("level01.amy-meet", "amy");
			GameMgr.RecordNodeVisited("level01.dad-urgent-text", "dad");
			GameMgr.RecordNodeVisited("level01.amy-article", "amy");
		}


	}

}


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
		private Button m_level1Button = null;
		[SerializeField]
		private Button m_level2Button = null;
		[SerializeField]
		private Button m_level3Button = null;
		[SerializeField]
		private Button m_level4Button = null;

		private void OnEnable() {
			m_newGameButton.onClick.AddListener(HandleNewGame);
			m_level1Button.onClick.AddListener(HandleUnlock1);
			m_level2Button.onClick.AddListener(HandleUnlock2);
			m_level3Button.onClick.AddListener(HandleUnlock3);
			m_level4Button.onClick.AddListener(HandleUnlock4);
		}
		private void OnDisable() {
			m_newGameButton.onClick.RemoveListener(HandleNewGame);
			m_level1Button.onClick.RemoveListener(HandleUnlock1);
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
			AudioSrcMgr.instance.PlayOneShot("click_new_game");
			UIMgr.CloseThenCall<UITitleScreen>(() => {
				GameMgr.MarkTitleScreenComplete();
				UIMgr.Open<UIOfficeScreen>();
				AudioSrcMgr.instance.PlayAudio("office_ambiance", true);
			});
		}
		private void HandleUnlock1() {
			AudioSrcMgr.instance.PlayOneShot("click_unlock");
			UnlockLevel1();
			HandleNewGame();
		}
		private void HandleUnlock2() {
			AudioSrcMgr.instance.PlayOneShot("click_unlock");
			UnlockLevel2();
			HandleNewGame();
		}
		private void HandleUnlock3() {
			AudioSrcMgr.instance.PlayOneShot("click_unlock");
			UnlockLevel3();
			HandleNewGame();
		}
		private void HandleUnlock4() {
			AudioSrcMgr.instance.PlayOneShot("click_unlock");
			UnlockLevel4();
			HandleNewGame();
		}


		private void UnlockLevel1() {
			GameMgr.UnlockLevel(1);
			GameMgr.UnlockEvidence(1, "LV1-Root");
			GameMgr.UnlockEvidence(1, "LV1-Transcript-Lou");
			GameMgr.UnlockEvidence(1, "LV1-Photo-Above");
			GameMgr.UnlockEvidence(1, "LV1-Photo-Name");
			GameMgr.UnlockEvidence(1, "LV1-Photo-Artifact");
			GameMgr.UnlockEvidence(1, "LV1-Card-Types");
			GameMgr.UnlockEvidence(1, "LV1-Table-Wrecks");
			GameMgr.UnlockEvidence(1, "LV1-Article-Sinking");
			GameMgr.UnlockContact("dad");
			GameMgr.UnlockContact("lou");
			GameMgr.UnlockContact("amy");
			GameMgr.RecordNodeVisited("level01.mom-meet", "mom");
			GameMgr.RecordNodeVisited("level01.dad-meet", "dad");
			GameMgr.RecordNodeVisited("level01.lou-meet", "lou");
			GameMgr.RecordNodeVisited("level01.amy-meet", "amy");
			GameMgr.RecordNodeVisited("level01.dad-urgent-text", "dad");
			GameMgr.RecordNodeVisited("level01.amy-article", "amy");

			GameMgr.SetLevelIndex(0);
			GameMgr.SetChain(0, "Location", "location-coordinates");
		}
		private void UnlockLevel2() {
			UnlockLevel1();

			GameMgr.SetLevelIndex(0);
			GameMgr.SetChain(0, "Type", "card-canaller", "photo-above", "type-canaller");
			GameMgr.SetChain(0, "Name", "photo-name", "name-loretta");
			GameMgr.SetChain(0, "Cause", "cause-sandbar");
			GameMgr.SetChain(0, "Cargo", "cargo-cargo", "cargo-corn");
			GameMgr.SetChain(0, "Artifact", "photo-artifact", "artifact-trunk");

			GameMgr.UnlockLevel(2);
			GameMgr.UnlockLevel(4);
			GameMgr.UnlockEvidence(4, "LV4-Letter-Treasure");
			//GameMgr.UnlockEvidence(2, "LV2-Transcript-Reya");
			GameMgr.UnlockContact("reya");
			GameMgr.RecordNodeVisited("level01.lou-complete", "lou");
			GameMgr.RecordNodeVisited("level01.amy-level-end", "amy");
			GameMgr.RecordNodeVisited("level01.dad-level-end", "dad");
			GameMgr.RecordNodeVisited("level04.level2-starter", "dad");
			GameMgr.RecordNodeVisited("level04.level2-meet-reya", "reya");
		}

		private void UnlockLevel3() {
			UnlockLevel2();
		}

		private void UnlockLevel4() {
			UnlockLevel3();
		}
	}

}


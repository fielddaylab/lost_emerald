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
		private Button m_level2_50Button = null;
		[SerializeField]
		private Button m_level3Button = null;
		[SerializeField]
		private Button m_level3_50Button = null;
		[SerializeField]
		private Button m_level4Button = null;

		private void OnEnable() {
			m_newGameButton.onClick.AddListener(HandleNewGame);
			m_level1Button.onClick.AddListener(HandleUnlock1);
			m_level2Button.onClick.AddListener(HandleUnlock2);
			m_level2_50Button.onClick.AddListener(HandleUnlock2_50);
			m_level3Button.onClick.AddListener(HandleUnlock3);
			m_level3_50Button.onClick.AddListener(HandleUnlock3_50);
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
			GameMgr.UnlockContact("dad");
			UIMgr.CloseThenCall<UITitleScreen>(() => {
				GameMgr.MarkTitleScreenComplete();
				UIMgr.Open<UIOfficeScreen>();
				AudioSrcMgr.instance.PlayAudio("office_music", true);
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
		private void HandleUnlock2_50()
		{
			AudioSrcMgr.instance.PlayOneShot("click_unlock");
			UnlockLevel2_50();
			HandleNewGame();
		}
		private void HandleUnlock3() {
			AudioSrcMgr.instance.PlayOneShot("click_unlock");
			UnlockLevel3();
			HandleNewGame();
		}
		private void HandleUnlock3_50() {
			AudioSrcMgr.instance.PlayOneShot("click_unlock");
			UnlockLevel3_50();
			HandleNewGame();
		}
		private void HandleUnlock4() {
			AudioSrcMgr.instance.PlayOneShot("click_unlock");
			UnlockLevel4();
			HandleNewGame();
		}


		private void UnlockLevel1() {
			GameMgr.UnlockLevel(1);
			GameMgr.UnlockEvidence(1, "LV1-Transcript-Lou");
			GameMgr.UnlockEvidence(1, "LV1-Photo-Above");
			GameMgr.UnlockEvidence(1, "LV1-Photo-Name");
			GameMgr.UnlockEvidence(1, "LV1-Photo-Artifact");
			GameMgr.UnlockEvidence(1, "LV1-Card-Types");
			GameMgr.UnlockEvidence(1, "LV1-Table-Wrecks");
			GameMgr.UnlockEvidence(1, "LV1-Article-Sinking");
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
			GameMgr.SetChain(0, "Type", "card-canaller", "photo-above", "type-canaller");
			GameMgr.SetChain(0, "Name", "photo-name", "name-loretta");
			GameMgr.SetChain(0, "Cause", "cause-sandbar");
			GameMgr.SetChain(0, "Cargo", "cargo-cargo", "cargo-corn");
		}
		private void UnlockLevel2() {
			UnlockLevel1();

			GameMgr.SetLevelIndex(0);

			GameMgr.SetChain(0, "Artifact", "photo-artifact", "artifact-trunk");

			GameMgr.State.SetTutorialBuoyDropped(true);
			GameMgr.State.SetTutorialSonarDisplayed(true);
			GameMgr.State.UnlockDive(0);

			// GameMgr.UnlockLevel(2);
			GameMgr.UnlockLevel(4);
			GameMgr.UnlockEvidence(4, "LV4-Letter-Treasure");
			GameMgr.RecordNodeVisited("level01.lou-complete", "lou");
			GameMgr.RecordNodeVisited("level01.amy-level-end", "amy");
			GameMgr.RecordNodeVisited("level01.dad-level-end", "dad");
			GameMgr.SetLevelIndex(1);
		}

		private void UnlockLevel2_50()
		{
			UnlockLevel2();

			GameMgr.State.UnlockDive(1);
			GameMgr.UnlockContact("cooper");

			GameMgr.UnlockContact("reya");
			GameMgr.RecordNodeVisited("level04.level2-starter", "dad");
			GameMgr.RecordNodeVisited("level04.level2-meet-reya", "reya");
			GameMgr.UnlockLevel(2);
			GameMgr.UnlockEvidence(2, "LV2-Transcript-Reya");
			GameMgr.UnlockEvidence(2, "LV2-Card-Types");

			GameMgr.UnlockEvidence(2, "LV2-Photo-Above");
			GameMgr.UnlockEvidence(2, "LV2-Photo-Cargo");
			GameMgr.UnlockEvidence(2, "LV2-Photo-Gash");
			GameMgr.UnlockEvidence(2, "LV2-Table-Wrecks");
			GameMgr.UnlockEvidence(2, "LV2-Photo-Safe");
			GameMgr.UnlockEvidence(2, "LV2-Images-Car");
			GameMgr.UnlockEvidence(4, "LV4-Investigation-Report");
			GameMgr.RecordNodeVisited("level02.reya-boat", "reya");
			GameMgr.RecordNodeVisited("level02.reya-dive", "reya");
			GameMgr.RecordNodeVisited("level02.dive-gash", "reya");
			GameMgr.RecordNodeVisited("level02.dive-cargo", "reya");
			GameMgr.RecordNodeVisited("level02.dive-safe", "reya");
			GameMgr.RecordNodeVisited("level02.amy-match", "amy");
			GameMgr.RecordNodeVisited("level02.cooper-meet", "cooper");

			GameMgr.SetLevelIndex(1);
			GameMgr.SetChain(1, "Location", "location-coordinates");
			GameMgr.SetChain(1, "Type", "photo-above", "card-freighter");
		}

		private void UnlockLevel3() {
			UnlockLevel2_50();
			GameMgr.UnlockLevel(3);
			GameMgr.State.UnlockDive(1);

			GameMgr.UnlockEvidence(2, "LV2-Transcript-Reya");
			GameMgr.UnlockEvidence(2, "LV2-Card-Types");
			GameMgr.UnlockEvidence(2, "LV2-List-Cargo");
			GameMgr.UnlockEvidence(2, "LV2-Distress-Transcript");
			GameMgr.RecordNodeVisited("level02.amy-match", "amy");
			GameMgr.RecordNodeVisited("level02.cooper-meet", "cooper");
			GameMgr.RecordNodeVisited("level02.amy-distress", "amy");
			GameMgr.RecordNodeVisited("level02.reya-safe", "reya");

			GameMgr.SetChain(1, "Cargo", "photo-cargo", "cargo-nash", "image-nash");
			GameMgr.SetChain(1, "Cause", "photo-gash", "cause-rammed");
			GameMgr.SetChain(1, "Name", "name-madison");
			GameMgr.SetChain(1, "Artifact", "photo-safe", "artifact-safe");

			GameMgr.SetLevelIndex(2);
		}

		private void UnlockLevel3_50() {
			UnlockLevel3();
			GameMgr.SetChain(2, "Location", "location-coordinates");

			GameMgr.UnlockEvidence(3, "LV3-Transcript-Dad");

			GameMgr.UnlockEvidence(3, "LV3-Photo-Above");
			GameMgr.UnlockEvidence(3, "LV3-Photo-Anchor");
			GameMgr.UnlockEvidence(3, "LV3-Photo-Gold");
			GameMgr.UnlockEvidence(3, "LV3-Photo-Cargo");
			GameMgr.UnlockEvidence(3, "LV3-Ship-Chart");
			GameMgr.UnlockEvidence(3, "LV3-Card-Types");
			GameMgr.UnlockEvidence(3, "LV3-Accounts-Survivor");
			GameMgr.UnlockContact("cami");

			GameMgr.RecordNodeVisited("level03.mom-photos", "mom");
			GameMgr.RecordNodeVisited("level03.dad-superior", "dad");
			GameMgr.RecordNodeVisited("level03.amy-paradise", "amy");
			GameMgr.RecordNodeVisited("level03.cami-meet", "cami");

			GameMgr.State.UnlockDive(2);

			GameMgr.RecordNodeVisited("level03.cami-ship", "cami");
			GameMgr.RecordNodeVisited("level03.cami-anchor", "cami");
			GameMgr.RecordNodeVisited("level03.cami-gold", "cami");
			GameMgr.RecordNodeVisited("level03.cami-cargo", "cami");
			GameMgr.RecordNodeVisited("level03.cami-regroup", "cami");
		}

		private void UnlockLevel4() {
			UnlockLevel3_50();
			GameMgr.State.UnlockDive(2);

			GameMgr.SetChain(2, "Type", "photo-above", "ship-chart-type", "card-side-steamer");
			GameMgr.SetChain(2, "Cargo", "photo-cargo", "ship-chart-cargo");
			GameMgr.SetChain(2, "Cause", "survivor-accounts-cause");
			GameMgr.SetChain(2, "Name", "photo-anchor", "ship-chart-name");
			GameMgr.SetChain(2, "Artifact", "survivor-accounts-artifact", "photo-gold");

			GameMgr.RecordNodeVisited("level03.dad-steamer", "dad");

			GameMgr.SetLevelIndex(3);

			// GameMgr.UnlockEvidence(4, "LV4-Photo-Cargo"); // hack until cargo is added to the dive scene
			GameMgr.UnlockEvidence(4, "LV4-Ship-Chart"); // hack until cargo is added to the dive scene
		}
	}

}


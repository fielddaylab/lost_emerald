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
		[SerializeField]
		private Button m_level4_50Button = null;
		[SerializeField]
		private Button m_levelAllButton = null;
		[SerializeField]
		private Button m_credits = null;

		private void OnEnable() {
			m_newGameButton.onClick.AddListener(HandleNewGame);
			m_level1Button.onClick.AddListener(HandleUnlock1);
			m_level2Button.onClick.AddListener(HandleUnlock2);
			m_level2_50Button.onClick.AddListener(HandleUnlock2_50);
			m_level3Button.onClick.AddListener(HandleUnlock3);
			m_level3_50Button.onClick.AddListener(HandleUnlock3_50);
			m_level4Button.onClick.AddListener(HandleUnlock4);
			m_level4_50Button.onClick.AddListener(HandleUnlock4_50);
			m_levelAllButton.onClick.AddListener(HandleUnlockAll);
			m_credits.onClick.AddListener(HandleCredits);
		}
		private void OnDisable() {
			m_newGameButton.onClick.RemoveListener(HandleNewGame);
			m_level1Button.onClick.RemoveListener(HandleUnlock1);
			m_level2Button.onClick.RemoveListener(HandleUnlock2);
			m_level2_50Button.onClick.RemoveListener(HandleUnlock2_50);
			m_level3Button.onClick.RemoveListener(HandleUnlock3);
			m_level3_50Button.onClick.RemoveListener(HandleUnlock3_50);
			m_level4Button.onClick.RemoveListener(HandleUnlock4);
			m_level4_50Button.onClick.RemoveListener(HandleUnlock4_50);
			m_levelAllButton.onClick.RemoveListener(HandleUnlockAll);
			m_credits.onClick.RemoveListener(HandleCredits);
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

		private void HandleNewGame() {
			AudioSrcMgr.instance.PlayOneShot("click_new_game");
			GameMgr.UnlockContact("dad");
			UIMgr.CloseThenCall<UITitleScreen>(() => {
				GameMgr.MarkTitleScreenComplete();
				UIMgr.Open<UIOfficeScreen>();
				AudioSrcMgr.instance.PlayAudio("office_music", true);
			});
		}
		private void HandleCredits() {
			UIMgr.CloseThenOpen<UITitleScreen, UITitleCredits>();
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
			UnlockLevel3(true);
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
		private void HandleUnlock4_50() {
			AudioSrcMgr.instance.PlayOneShot("click_unlock");
			UnlockLevel4_50();
			HandleNewGame();
		}
		private void HandleUnlockAll() {
			AudioSrcMgr.instance.PlayOneShot("click_unlock");
			UnlockAll();
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
			GameMgr.RecordNodeVisited("level01.dive-start", "you");
			GameMgr.RecordNodeVisited("level01.dive-photo-above", "you");
			GameMgr.RecordNodeVisited("level01.dive-photo-name", "you");
			GameMgr.RecordNodeVisited("level01.dive-photo-artifact", "you");

			GameMgr.SetLevelIndex(0);
			GameMgr.SetChain(0, "Location", "location-coordinates");
		}
		private void UnlockLevel2() {
			UnlockLevel1();

			GameMgr.SetLevelIndex(0);

			GameMgr.SetChain(0, "Artifact", "photo-artifact", "artifact-trunk");
			GameMgr.SetChain(0, "Type", "card-canaller", "photo-above", "type-canaller");
			GameMgr.SetChain(0, "Name", "photo-name", "name-loretta");
			GameMgr.SetChain(0, "Cause", "cause-sandbar");
			GameMgr.SetChain(0, "Cargo", "cargo-cargo", "cargo-corn");

			GameMgr.State.SetTutorialBuoyDropped(true);
			GameMgr.State.SetTutorialSonarDisplayed(true);
			GameMgr.State.UnlockDive(0);

			// GameMgr.UnlockLevel(2);
			GameMgr.UnlockLevel(4);
			GameMgr.UnlockEvidence(4, "LV4-Do-Later-Note");
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
			//GameMgr.UnlockEvidence(4, "LV4-Investigation-Report");
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

		private void UnlockLevel3(bool furthestUnlock) {
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

			
			if (furthestUnlock) {
				// this will cause the initial phone conversation
				// to run instead of needing to show the case
				// closed modal
				Routine.Start(this, Routine.Delay(() => {
					GameMgr.SetLevelIndex(1); // must be at level 2
					GameMgr.Events.Dispatch(GameEvents.CaseClosed);
					GameMgr.RunTrigger(GameTriggers.OnCaseClosed);
				}, 0.5f)).ExecuteWhileDisabled();
			}
		}

		private void UnlockLevel3_50() {
			UnlockLevel3(false);

			GameMgr.SetLevelIndex(2);
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

			//GameMgr.RecordNodeVisited("level03.mom-photos", "mom");
			GameMgr.RecordNodeVisited("level02.dad-superior", "dad");
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
		}

		private void UnlockLevel4_50() {
			UnlockLevel4();

			GameMgr.RemoveEvidence(4, "LV4-Do-Later-Note");

			GameMgr.State.UnlockDive(3);

			GameMgr.RecordNodeVisited("level04.reya-steel", "reya");
			GameMgr.RecordNodeVisited("level04.amy-lead", "amy");
			GameMgr.RecordNodeVisited("level04.reya-rushing", "reya");
			GameMgr.RecordNodeVisited("level04.reya-dad-sonar", "reya");
			GameMgr.RecordNodeVisited("level04.reya-mow", "reya");
			GameMgr.RecordNodeVisited("level04.dive-start", "reya");
			GameMgr.RecordNodeVisited("level04.dive-name", "reya");
			GameMgr.RecordNodeVisited("level04.dive-locket", "reya");
			GameMgr.RecordNodeVisited("level04.amy-strange", "amy");
			GameMgr.RecordNodeVisited("level04.ed-survivor", "ed");
			GameMgr.RecordNodeVisited("level04.reya-mow", "reya");

			GameMgr.UnlockEvidence(4, "LV4-Location-Coordinates");
			GameMgr.UnlockEvidence(4, "LV4-Card-Types");
			GameMgr.UnlockEvidence(4, "LV4-Ship-Chart");
			GameMgr.UnlockEvidence(4, "LV4-Photo-Above");
			GameMgr.UnlockEvidence(4, "LV4-Photo-Cause");
			GameMgr.UnlockEvidence(4, "LV4-Photo-Name");
			GameMgr.UnlockEvidence(4, "LV4-Photo-Locket");
			GameMgr.UnlockEvidence(4, "LV4-Transcript-Survivor");

			GameMgr.DiscoverLocation(4);

			GameMgr.SetChain(3, "Location", "location-coordinates");
		}

		private void UnlockAll() {
			UnlockLevel4_50();

			GameMgr.RecordNodeVisited("level04.mom-weak", "mom");
			GameMgr.RecordNodeVisited("level04.reya-end", "reya");

			GameMgr.UnlockEvidence(4, "LV4-Transcript-Mom");

			GameMgr.SetChain(3, "Type", "ship-chart-type", "photo-above", "card-freighter");
			GameMgr.SetChain(3, "Cargo", "ship-chart-cargo");
			GameMgr.SetChain(3, "Cause", "photo-cause", "survivor-waves", "mom-brittle");
			GameMgr.SetChain(3, "Name", "ship-chart-name", "photo-name");
			GameMgr.SetChain(3, "Artifact", "letter-locket", "photo-locket");

		}
	}

}


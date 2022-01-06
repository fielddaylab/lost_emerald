using BeauRoutine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UITitleUnlocks : UIBase {

		[SerializeField]
		private RectTransform m_grouper = null;
		[SerializeField]
		private Image m_overlay = null;
		[SerializeField]
		private Button m_buttonClose = null;
		[SerializeField]
		private Button m_buttonReview = null;
		[SerializeField]
		private Button m_buttonLevel1Part1 = null;
		[SerializeField]
		private Button m_buttonLevel1Part2 = null;
		[SerializeField]
		private Button m_buttonLevel2Part1 = null;
		[SerializeField]
		private Button m_buttonLevel2Part2 = null;
		[SerializeField]
		private Button m_buttonLevel3Part1 = null;
		[SerializeField]
		private Button m_buttonLevel3Part2 = null;
		[SerializeField]
		private Button m_buttonLevel4Part1 = null;
		[SerializeField]
		private Button m_buttonLevel4Part2 = null;

		protected override void OnShowCompleted() {
			base.OnShowCompleted();
			m_buttonClose.onClick.AddListener(HandleCloseButton);
			m_buttonLevel1Part1.onClick.AddListener(HandleUnlockLv1Part1);
			m_buttonLevel1Part2.onClick.AddListener(HandleUnlockLv1Part2);
			m_buttonLevel2Part1.onClick.AddListener(HandleUnlockLv2Part1);
			m_buttonLevel2Part2.onClick.AddListener(HandleUnlockLv2Part2);
			m_buttonLevel3Part1.onClick.AddListener(HandleUnlockLv3Part1);
			m_buttonLevel3Part2.onClick.AddListener(HandleUnlockLv3Part2);
			m_buttonLevel4Part1.onClick.AddListener(HandleUnlockLv4Part1);
			m_buttonLevel4Part2.onClick.AddListener(HandleUnlockLv4Part2);
			m_buttonReview.onClick.AddListener(HandleUnlockAll);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_buttonClose.onClick.RemoveListener(HandleCloseButton);
			m_buttonLevel1Part1.onClick.RemoveListener(HandleUnlockLv1Part1);
			m_buttonLevel1Part2.onClick.RemoveListener(HandleUnlockLv1Part2);
			m_buttonLevel2Part1.onClick.RemoveListener(HandleUnlockLv2Part1);
			m_buttonLevel2Part2.onClick.RemoveListener(HandleUnlockLv2Part2);
			m_buttonLevel3Part1.onClick.RemoveListener(HandleUnlockLv3Part1);
			m_buttonLevel3Part2.onClick.RemoveListener(HandleUnlockLv3Part2);
			m_buttonLevel4Part1.onClick.RemoveListener(HandleUnlockLv4Part1);
			m_buttonLevel4Part2.onClick.RemoveListener(HandleUnlockLv4Part2);
			m_buttonReview.onClick.RemoveListener(HandleUnlockAll);
		}


		protected override IEnumerator HideImmediateRoutine() {
			throw new NotImplementedException();
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

		private void StartGame() {
			AudioSrcMgr.instance.PlayOneShot("click_new_game");
			GameMgr.UnlockContact("dad");
			UIMgr.Close(this);
			UIMgr.CloseThenCall<UITitleScreen>(() => {
				GameMgr.MarkTitleScreenComplete();
				UIMgr.Open<UIOfficeScreen>();
				AudioSrcMgr.instance.PlayAudio("office_music", true);
			});
		}

		private void HandleCloseButton() {
			AudioSrcMgr.instance.PlayOneShot("click_map_close");
			UIMgr.Close(this);
		}

		private void HandleUnlockLv1Part1() {
			GameMgr.ClearState();
			StartGame();
		}

		private void HandleUnlockLv1Part2() {
			GameMgr.ClearState();
			UnlockLevel1Part2();
			StartGame();
		}
		private void HandleUnlockLv2Part1() {
			GameMgr.ClearState();
			UnlockLevel2Part1();
			StartGame();
		}
		private void HandleUnlockLv2Part2() {
			GameMgr.ClearState();
			UnlockLevel2Part2();
			StartGame();
		}
		private void HandleUnlockLv3Part1() {
			GameMgr.ClearState();
			UnlockLevel3Part1(true);
			StartGame();
		}
		private void HandleUnlockLv3Part2() {
			GameMgr.ClearState();
			UnlockLevel3Part2();
			StartGame();
		}
		private void HandleUnlockLv4Part1() {
			GameMgr.ClearState();
			UnlockLevel4Part1();
			StartGame();
		}
		private void HandleUnlockLv4Part2() {
			GameMgr.ClearState();
			UnlockLevel4Part2();
			StartGame();
		}
		private void HandleUnlockAll() {
			GameMgr.ClearState();
			UnlockAll();
			StartGame();
		}

		private void UnlockLevel1Part2() {
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

			GameMgr.RecordNodeVisited("level01.sonar-tutorial", "you");
			GameMgr.State.SetTutorialBuoyDropped(true);
			GameMgr.State.SetTutorialSonarDisplayed(true);
			GameMgr.State.UnlockDive(0);

			GameMgr.SetLevelIndex(0);
			GameMgr.SetChain(0, "Location", "location-coordinates");
		}
		private void UnlockLevel2Part1() {
			UnlockLevel1Part2();

			GameMgr.SetLevelIndex(0);

			GameMgr.SetChain(0, "Artifact", "photo-artifact", "artifact-trunk");
			GameMgr.SetChain(0, "Type", "card-canaller", "photo-above", "type-canaller");
			GameMgr.SetChain(0, "Name", "photo-name", "name-loretta");
			GameMgr.SetChain(0, "Cause", "cause-sandbar");
			GameMgr.SetChain(0, "Cargo", "cargo-cargo", "cargo-corn");

			GameMgr.UnlockLevel(4);
			GameMgr.UnlockEvidence(4, "LV4-Do-Later-Note");
			GameMgr.UnlockEvidence(4, "LV4-Letter-Treasure");
			GameMgr.RecordNodeVisited("level01.lou-complete", "lou");
			GameMgr.RecordNodeVisited("level01.amy-level-end", "amy");
			GameMgr.RecordNodeVisited("level01.dad-level-end", "dad");
			GameMgr.SetLevelIndex(1);
		}

		private void UnlockLevel2Part2() {
			UnlockLevel2Part1();

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
		}

		private void UnlockLevel3Part1(bool furthestUnlock) {
			UnlockLevel2Part2();
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

			GameMgr.SetChain(1, "Type", "photo-above", "card-freighter");
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

		private void UnlockLevel3Part2() {
			UnlockLevel3Part1(false);

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

		private void UnlockLevel4Part1() {
			UnlockLevel3Part2();
			GameMgr.State.UnlockDive(2);

			GameMgr.SetChain(2, "Type", "photo-above", "ship-chart-type", "card-side-steamer");
			GameMgr.SetChain(2, "Cargo", "photo-cargo", "ship-chart-cargo");
			GameMgr.SetChain(2, "Cause", "survivor-accounts-cause");
			GameMgr.SetChain(2, "Name", "photo-anchor", "ship-chart-name");
			GameMgr.SetChain(2, "Artifact", "survivor-accounts-artifact", "photo-gold");

			GameMgr.RecordNodeVisited("level03.dad-steamer", "dad");

			GameMgr.SetLevelIndex(3);
		}

		private void UnlockLevel4Part2() {
			UnlockLevel4Part1();

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
			UnlockLevel4Part2();

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
using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIOfficeScreen : UIBase {

		[SerializeField]
		private Button[] m_levelButtons = null;

		protected override void OnShowStart() {
			base.OnShowStart();
			CanvasGroup.alpha = 0;

			int levelIndex = 0;
			foreach (Button button in m_levelButtons) {
				button.interactable = GameMgr.State.IsLevelUnlocked(levelIndex);
				if (button.interactable) {
					button.onClick.AddListener(() => { HandleLevelButton(levelIndex); });
				}
				levelIndex++;
			}
			GameMgr.RunTrigger(GameTriggers.OnEnterOffice);
			GameMgr.Events.Register<int>(GameEvents.LevelUnlocked, HandleLevelUnlocked);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			foreach (Button button in m_levelButtons) {
				button.onClick.RemoveAllListeners();
				button.interactable = false;
			}
			GameMgr.Events.Deregister<int>(GameEvents.LevelUnlocked, HandleLevelUnlocked);
		}


		private void HandleLevelButton(int level) {
			UIMgr.CloseThenOpen<UIOfficeScreen, UIEvidenceScreen>();
		}

		private void HandleLevelUnlocked(int level) {
			if (!m_levelButtons[level].interactable) {
				m_levelButtons[level].interactable = true;
				m_levelButtons[level].onClick.AddListener(() => { HandleLevelButton(level); });
			}
		}


		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}
		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
		}
	}

}
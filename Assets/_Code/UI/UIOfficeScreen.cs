using BeauRoutine;
using PotatoLocalization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIOfficeScreen : UIBase {

		[SerializeField]
		private Button[] m_levelButtons = null;
		[SerializeField]
		private LocalizedTextUGUI[] m_levelLabels = null;

		protected override void OnShowStart() {
			base.OnShowStart();
			CanvasGroup.alpha = 0;

			m_levelButtons[0].onClick.AddListener(() => { HandleLevelButton(0); });
			m_levelButtons[1].onClick.AddListener(() => { HandleLevelButton(1); });
			m_levelButtons[2].onClick.AddListener(() => { HandleLevelButton(2); });
			m_levelButtons[3].onClick.AddListener(() => { HandleLevelButton(3); });

			for (int index = 0; index < m_levelButtons.Length; index++) {
				m_levelButtons[index].interactable = GameMgr.State.IsLevelUnlocked(index);
				m_levelLabels[index].Key = GameMgr.State.GetLevelName(index);
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
			AudioSrcMgr.instance.PlayOneShot("click_level");
			GameMgr.SetLevelIndex(level);
			GameMgr.State.SetCurrShipOutIndex(level);
			UIMgr.CloseThenOpen<UIOfficeScreen, UIEvidenceScreen>();
		}

		private void HandleLevelUnlocked(int level) {
			for (int index = 0; index < m_levelButtons.Length; index++) {
				m_levelButtons[index].interactable = GameMgr.State.IsLevelUnlocked(index);
				m_levelLabels[index].Key = GameMgr.State.GetLevelName(index);
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
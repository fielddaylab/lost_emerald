using BeauRoutine;
using PotatoLocalization;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIMapScreen : UIBase {

		[SerializeField]
		private RectTransform m_container = null;
		[SerializeField]
		private Button m_closeButton = null;
		[SerializeField]
		private Button[] m_levelButtons = null;
		[SerializeField]
		private LocalizedTextUGUI[] m_levelLabels = null;

		protected override IEnumerator ShowRoutine() {
			m_container.anchoredPosition = new Vector2(0f, 620f);
			yield return Routine.Combine(
				CanvasGroup.FadeTo(1f, 0.25f),
				m_container.AnchorPosTo(0f, 0.2f, Axis.Y)
			);
		}
		protected override IEnumerator HideRoutine() {
			m_container.anchoredPosition = Vector2.zero;
			yield return Routine.Combine(
				m_container.AnchorPosTo(620f, 0.2f, Axis.Y),
				CanvasGroup.FadeTo(0f, 0.25f)
			);
		}

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

			GameMgr.Events.Register<int>(GameEvents.LevelUnlocked, HandleLevelUnlocked);
		}

		protected override void OnShowCompleted() {
			m_closeButton.onClick.AddListener(HandleClose);
		}
		protected override void OnHideStart() {
			m_closeButton.onClick.RemoveListener(HandleClose);

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
			UIMgr.Close<UIMapScreen>();
			UIMgr.CloseThenOpen<UIOfficeScreen, UIEvidenceScreen>();
		}

		private void HandleClose() {
			AudioSrcMgr.instance.PlayOneShot("click_map_close");
			UIMgr.Close<UIMapScreen>();
		}

		private void HandleLevelUnlocked(int level) {
			for (int index = 0; index < m_levelButtons.Length; index++) {
				m_levelButtons[index].interactable = GameMgr.State.IsLevelUnlocked(index);
				m_levelLabels[index].Key = GameMgr.State.GetLevelName(index);
			}
		}
	}

}
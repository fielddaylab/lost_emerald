using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIOfficeScreen : UIBase {

		[SerializeField]
		private Button m_level1Button = null;
		[SerializeField]
		private Button m_level2Button = null;
		[SerializeField]
		private Button m_level3Button = null;
		[SerializeField]
		private Button m_level4Button = null;

		protected override void OnShowStart() {
			base.OnShowStart();
			CanvasGroup.alpha = 0;

			m_level1Button.onClick.AddListener(() => { HandleLevelButton(1); });
			m_level2Button.onClick.AddListener(() => { HandleLevelButton(2); });
			m_level3Button.onClick.AddListener(() => { HandleLevelButton(3); });
			m_level4Button.onClick.AddListener(() => { HandleLevelButton(4); });

			GameMgr.RunTrigger(GameTriggers.OnEnterOffice);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_level1Button.onClick.RemoveAllListeners();
			m_level2Button.onClick.RemoveAllListeners();
			m_level3Button.onClick.RemoveAllListeners();
			m_level4Button.onClick.RemoveAllListeners();
		}


		private void HandleLevelButton(int level) {
			UIMgr.CloseThenOpen<UIOfficeScreen, UIEvidenceScreen>();
		}


		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}
		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
		}
	}

}
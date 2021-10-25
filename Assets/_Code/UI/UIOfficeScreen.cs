using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIOfficeScreen : UIBase {
		[SerializeField]
		private UIOfficeMapContainer m_officeMapContainer;
		[SerializeField]
		private Button m_levelMapButton;

		private void OnEnable() {
			m_levelMapButton.onClick.AddListener(HandleLevelMapButton);
		}
		private void OnDisable() {
			m_levelMapButton.onClick.RemoveListener(HandleLevelMapButton);
		}

		protected override void OnShowStart() {
			base.OnShowStart();
			base.CanvasGroup.interactable = true;

			GameMgr.RunTrigger(GameTriggers.OnEnterOffice);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
		}

		private void HandleLevelMapButton() {
			AudioSrcMgr.instance.PlayOneShot("click_level");
			UIMgr.Open<UIMapScreen>();
			// UIMgr.CloseThenOpen<UIOfficeScreen, UIEvidenceScreen>();
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}
		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
		}
	}

}
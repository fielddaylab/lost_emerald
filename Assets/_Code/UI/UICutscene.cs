using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UICutscene : UIBase {

		[SerializeField]
		private Button m_button = null;


		protected override void OnShowCompleted() {
			m_button.onClick.AddListener(HandleButtonPressed);
		}
		protected override void OnHideStart() {
			m_button.onClick.RemoveListener(HandleButtonPressed);
		}
		private void HandleButtonPressed() {
			UIMgr.Close(this);
			UIMgr.Open<UIOfficeScreen>();
			UIMgr.Open<UIModalCaseClosed>();
		}
		protected override IEnumerator HideRoutine() {
			yield return null;
		}
		protected override IEnumerator ShowRoutine() {
			yield return null;
		}
	}

}
using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIMapScreen : UIBase {

		[SerializeField]
		private RectTransform m_container = null;
		[SerializeField]
		private Button m_shipOutButton = null;
		[SerializeField]
		private Button m_closeButton = null;

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


		protected override void OnShowCompleted() {
			m_closeButton.onClick.AddListener(HandleClose);
			m_shipOutButton.onClick.AddListener(HandleShipOut);
		}
		protected override void OnHideStart() {
			m_closeButton.onClick.RemoveListener(HandleClose);
			m_shipOutButton.onClick.RemoveListener(HandleShipOut);
		}

		private void HandleClose() {
			UIMgr.Close<UIOfficeScreen>();
			UIMgr.Close<UIMapScreen>();
			UIMgr.Open<UIEvidenceScreen>();
			UIMgr.Open<UIPhoneNotif>();
		}
		private void HandleShipOut() {
			UIMgr.Close<UIOfficeScreen>();
			UIMgr.Close<UIMapScreen>();
			// before loading the ShipOut scene, specify which ShipOutData to load
			GameMgr.State.SetCurrShipOutIndex(0);
			SceneManager.LoadScene("ShipOut");
			// SceneManager.LoadScene("Dive_Ship01"); // hack
			//UIMgr.Open<UIDiveScreen>();
		}
	}

}
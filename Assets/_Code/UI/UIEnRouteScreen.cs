using BeauRoutine;
using PotatoLocalization;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck {
	public class UIEnRouteScreen : UIBase {

		[SerializeField]
		private RectTransform m_container = null;
		[SerializeField]
		private Button m_closeButton = null;
		[SerializeField]
		private Button m_shipOutButton = null;

		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}

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
		}

		protected override void OnShowCompleted() {
			m_closeButton.onClick.AddListener(HandleClose);
			m_shipOutButton.onClick.AddListener(HandleShipOutButton);
		}
		protected override void OnHideStart() {
			m_closeButton.onClick.RemoveListener(HandleClose);
			m_shipOutButton.onClick.RemoveListener(HandleShipOutButton);
		}

		private void HandleClose() {
			AudioSrcMgr.instance.PlayOneShot("click_map_close");
			UIMgr.Close<UIEnRouteScreen>();
		}

		private void HandleShipOutButton() {
			UIMgr.Close<UIEvidenceScreen>();
			UIMgr.Close<UIEnRouteScreen>();
			AudioSrcMgr.instance.PlayAudio("ship_out");
			AudioSrcMgr.instance.PlayAmbiance("ship_out_ambiance", true);
			SceneManager.LoadScene("ShipOut");
		}
	}
}
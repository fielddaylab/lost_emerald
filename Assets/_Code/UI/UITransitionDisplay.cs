using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck {
	public class UITransitionDisplay : UIBase {

		[SerializeField]
		private Image[] m_icons;
		[SerializeField]
		private float m_fadeTime = 2f;

		private Routine m_showIconRoutine;

		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}

		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.25f);
		}
		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 2f);
		}

		protected override void OnShowStart() {
			base.OnShowStart();

			CanvasGroup.alpha = 0;
			foreach (Image icon in m_icons) {
				icon.SetAlpha(0);
			}
		}

		protected override void OnShowCompleted() {
			DisplayIcons();
			UIMgr.Close<UIEvidenceScreen>();
			UIMgr.Close<UIEnRouteScreen>();
		}

		protected override void OnHideStart() {
			AudioSrcMgr.instance.PlayAudio("ship_out");
			AudioSrcMgr.instance.PlayAmbiance("ship_out_ambiance", true);
			SceneManager.LoadScene("ShipOut");
			GameMgr.Events.Dispatch(GameEvents.SceneLoaded, "ShipOut");
		}

		private void DisplayIcons() {
			m_showIconRoutine.Replace(this, FadeIcon(0))
				.OnComplete(() => m_showIconRoutine.Replace(this, FadeIcon(1))
					.OnComplete(() => m_showIconRoutine.Replace(this, FadeIcon(2))
						.OnComplete(() => FinishDisplaying())
					)
				);
		}

		private IEnumerator FadeIcon(int index) {
			yield return m_icons[index].FadeTo(1f, m_fadeTime);
		}

		private void FinishDisplaying() {
			UIMgr.Close<UITransitionDisplay>();

			foreach (Image icon in m_icons) {
				icon.SetAlpha(0);
			}
		}
	}
}
using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck {
	public class UITransitionScreen : UIBase {

		[SerializeField]
		private Image[] m_icons;
		[SerializeField]
		private float m_iconFadeTime = 1.5f;
		[SerializeField]
		private float m_holdTime = 0.5f;
		[SerializeField]
		private float m_endFadeTime = 2f;

		private float m_startFadeTime = 0.25f;

		private Routine m_showIconRoutine;

		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}

		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, m_startFadeTime);
		}
		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, m_endFadeTime);
		}

		protected override void OnShowStart() {
			base.OnShowStart();

			CanvasGroup.alpha = 0;
			foreach (Image icon in m_icons) {
				icon.SetAlpha(0);
			}

			float transitionTime = m_startFadeTime + m_iconFadeTime * m_icons.Length + m_holdTime + m_endFadeTime;
			AudioSrcMgr.instance.CrossFadeAudio("ship_out_music", transitionTime, "ship_out", true);
			AudioSrcMgr.instance.CrossFadeAmbiance("ship_out_ambiance", transitionTime, true);
		}

		protected override void OnShowCompleted() {
			DisplayIcons();
			UIMgr.Close<UIEvidenceScreen>();
			UIMgr.Close<UIEnRouteScreen>();
		}

		protected override void OnHideStart() {
			//AudioSrcMgr.instance.PlayAmbiance("ship_out_ambiance", true);
			SceneManager.LoadScene("ShipOut");
			GameMgr.Events.Dispatch(GameEvents.SceneLoaded, "ShipOut");
		}

		protected override void OnHideCompleted() {
			base.OnHideCompleted();

			foreach (Image icon in m_icons) {
				icon.SetAlpha(0);
			}
		}

		private void DisplayIcons() {
			m_showIconRoutine.Replace(this, FadeIcon(0))
				.OnComplete(() => m_showIconRoutine.Replace(this, FadeIcon(1))
					.OnComplete(() => m_showIconRoutine.Replace(this, FadeIcon(2))
						.OnComplete(() => m_showIconRoutine.Replace(this, Wait(m_holdTime))
							.OnComplete(() => FinishDisplaying())
						)
					)
				);
		}

		private IEnumerator FadeIcon(int index) {
			yield return m_icons[index].FadeTo(1f, m_iconFadeTime);
		}

		private IEnumerator Wait(float seconds) {
			yield return new WaitForSeconds(seconds);
		}

		private void FinishDisplaying() {
			UIMgr.Close<UITransitionScreen>();
		}
	}
}
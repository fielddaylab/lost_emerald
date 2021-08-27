using BeauRoutine;
using System.Collections;
using UnityEngine;

namespace Shipwreck {

	/// <summary>
	/// This screen is meant to be shown on top of other screens in order to
	/// block raycasts to them, as well as having modal windows appear on
	/// top of the overlay
	/// </summary>
	public class UIModalOverlay : UIBase {

		[SerializeField]
		private CanvasGroup m_overlay = null;
		[SerializeField]
		private TweenSettings m_tweenSettings = new TweenSettings(0.2f, Curve.QuadInOut);


		protected override IEnumerator HideRoutine() {
			yield return m_overlay.FadeTo(0f, m_tweenSettings);
		}
		protected override IEnumerator ShowRoutine() {
			yield return m_overlay.FadeTo(1f, m_tweenSettings);
		}
	}

}
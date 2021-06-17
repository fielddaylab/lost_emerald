using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Shipwreck {

	public class UIPhoneNotif : UIBase {

		[SerializeField]
		private TweenSettings m_showHideTween = new TweenSettings(0.3f, Curve.QuadOut);

		protected override void OnShowStart() {
			base.OnShowStart();
			Vector2 anchoredPos = ((RectTransform)transform).anchoredPosition;
			((RectTransform)transform).anchoredPosition = new Vector2(anchoredPos.x, -200f);
		}

		protected override IEnumerator HideRoutine() {
			yield return ((RectTransform)transform).AnchorPosTo(-200f, m_showHideTween, Axis.Y).DelayBy(0.1f);
		}
		protected override IEnumerator ShowRoutine() {
			yield return ((RectTransform)transform).AnchorPosTo(0f, m_showHideTween, Axis.Y);
		}
	}

}



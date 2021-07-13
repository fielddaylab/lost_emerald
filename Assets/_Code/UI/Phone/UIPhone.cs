using BeauRoutine;
using BeauUtil;
using Leaf.Runtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIPhone : UIBase {

		[SerializeField]
		private TweenSettings m_tweenSettings = new TweenSettings(0.3f, Curve.QuadInOut);
		[SerializeField]
		private UIContacts m_contactScreen = null;
		[SerializeField]
		private UITextMessage m_textMsgScreen = null;

		private RectTransform m_rectTransform;

		private void Awake() {
			m_rectTransform = (RectTransform)transform;
		}

		#region UIBase

		protected override void OnShowStart() {
			base.OnShowStart();

			GameMgr.Events.Dispatch(GameEvents.PhoneOpened);
			UIMgr.Close<UIPhoneNotif>();
			m_rectTransform.anchoredPosition = new Vector2(m_rectTransform.anchoredPosition.x, -660f);
		}
		protected override void OnHideCompleted() {
			base.OnHideCompleted();

			UIMgr.Close(m_contactScreen);
			UIMgr.Close(m_textMsgScreen);

			GameMgr.Events.Dispatch(GameEvents.PhoneClosed);
			UIPhoneNotif.AttemptReopen();
		}

		protected override IEnumerator HideRoutine() {
			yield return m_rectTransform.AnchorPosTo(-660f, m_tweenSettings, Axis.Y);
		}
		protected override IEnumerator ShowRoutine() {
			yield return m_rectTransform.AnchorPosTo(45f, m_tweenSettings, Axis.Y);
		}

		#endregion // UIBase
	}


}
using BeauRoutine;
using System.Collections;
using UnityEngine;

namespace Shipwreck {

	public class UIPhone : UIBase {

		[SerializeField]
		private RectTransform m_phoneTransform = null;
		[SerializeField]
		private TweenSettings m_tweenSettings = new TweenSettings(0.3f, Curve.QuadInOut);
		[SerializeField]
		private UIContacts m_contactScreen = null;
		[SerializeField]
		private UITextMessage m_textMsgScreen = null;


		#region UIBase

		protected override void OnShowStart() {
			base.OnShowStart();

			GameMgr.Events.Dispatch(GameEvents.PhoneOpened);
			UIMgr.Close<UIPhoneNotif>();
			m_phoneTransform.anchoredPosition = new Vector2(m_phoneTransform.anchoredPosition.x, -660f);
		}
		protected override void OnHideCompleted() {
			base.OnHideCompleted();

			UIMgr.Close(m_contactScreen);
			UIMgr.Close(m_textMsgScreen);

			GameMgr.Events.Dispatch(GameEvents.PhoneClosed);
			//UIPhoneNotif.AttemptReopen();
		}

		protected override IEnumerator HideRoutine() {
			yield return m_phoneTransform.AnchorPosTo(-660f, m_tweenSettings, Axis.Y);
		}
		protected override IEnumerator ShowRoutine() {
			yield return m_phoneTransform.AnchorPosTo(45f, m_tweenSettings, Axis.Y);
		}

		#endregion // UIBase
	}


}
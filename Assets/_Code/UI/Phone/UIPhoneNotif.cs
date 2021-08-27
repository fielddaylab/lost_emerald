using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIPhoneNotif : UIBase {

		[SerializeField]
		private TweenSettings m_showHideTween = new TweenSettings(0.3f, Curve.QuadOut);
		[SerializeField]
		private RectTransform m_phoneTransform = null;
		[SerializeField]
		private Button m_button = null;

		//[SerializeField]
		//private GameObject m_notificationGroup = null;
		//[SerializeField]
		//private TextMeshProUGUI m_notificationCounter = null;

		private void OnEnable() {
			m_button.onClick.AddListener(HandlePressed);
			//UpdateNotificationCounter();
			//GameMgr.Events.Register(GameEvents.PhoneNotification, UpdateNotificationCounter, this);
		}
		private void OnDisable() {
			m_button.onClick.RemoveListener(HandlePressed);
			//GameMgr.Events.DeregisterAll(this);
		}


		protected override void OnShowStart() {
			base.OnShowStart();
			m_button.interactable = false;
			m_phoneTransform.anchoredPosition = new Vector2(m_phoneTransform.anchoredPosition.x, -200f);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_button.interactable = false;
		}

		protected override IEnumerator HideRoutine() {
			yield return m_phoneTransform.AnchorPosTo(-200f, m_showHideTween, Axis.Y).DelayBy(0.1f);
		}
		protected override IEnumerator ShowRoutine() {
			yield return m_phoneTransform.AnchorPosTo(0f, m_showHideTween, Axis.Y);
			m_button.interactable = true;
		}

		private void HandlePressed() {
			UIMgr.Close(this);
			GameMgr.TryRunLastNotification(out var _);
		}

	}

}



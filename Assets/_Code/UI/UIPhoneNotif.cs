using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIPhoneNotif : UIBase {

		[SerializeField]
		private TweenSettings m_showHideTween = new TweenSettings(0.3f, Curve.QuadOut);
		[SerializeField]
		private Button m_button = null;
		[SerializeField]
		private GameObject m_notificationGroup = null;
		[SerializeField]
		private TextMeshProUGUI m_notificationCounter = null;

		private void OnEnable() {
			m_button.onClick.AddListener(HandlePressed);
			UpdateNotificationCounter();
			GameMgr.Events.Register(GameEvents.PhoneNotification, UpdateNotificationCounter, this);
		}
		private void OnDisable() {
			m_button.onClick.RemoveListener(HandlePressed);
			GameMgr.Events.DeregisterAll(this);
		}


		protected override void OnShowStart() {
			base.OnShowStart();
			m_button.interactable = false;
			Vector2 anchoredPos = ((RectTransform)transform).anchoredPosition;
			((RectTransform)transform).anchoredPosition = new Vector2(anchoredPos.x, -200f);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_button.interactable = false;
		}

		protected override IEnumerator HideRoutine() {
			yield return ((RectTransform)transform).AnchorPosTo(-200f, m_showHideTween, Axis.Y).DelayBy(0.1f);
		}
		protected override IEnumerator ShowRoutine() {
			yield return ((RectTransform)transform).AnchorPosTo(0f, m_showHideTween, Axis.Y);
			m_button.interactable = true;
		}

		private void HandlePressed() {
			if (!GameMgr.TryRunLastNotification(out var _)) {
				UIMgr.CloseThenOpen<UIPhoneNotif,UIContacts>();
			} else {
				UIMgr.Close(this);
				UpdateNotificationCounter();
			}
		}

		private void UpdateNotificationCounter() {
			uint counter = GameMgr.State.NotificationCount();
			if (counter > 0) {
				m_notificationGroup.SetActive(true);
				m_notificationCounter.SetText(counter.ToString());
			} else {
				m_notificationGroup.SetActive(false);
			}
		}

	}

}



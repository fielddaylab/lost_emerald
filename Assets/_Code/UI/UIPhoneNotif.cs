using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIPhoneNotif : UIBase {

		[SerializeField]
		private TweenSettings m_showHideTween = new TweenSettings(0.3f, Curve.QuadOut);
		[SerializeField]
		private Button m_button = null;

		private void OnEnable() {
			m_button.onClick.AddListener(HandlePressed);
		}
		private void OnDisable() {
			m_button.onClick.RemoveListener(HandlePressed);
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
			UIMgr.CloseThenOpen<UIPhoneNotif,UIContacts>();

		}


	}

}



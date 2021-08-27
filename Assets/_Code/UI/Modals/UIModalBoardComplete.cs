using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIModalBoardComplete : UIBase {

		[SerializeField]
		private RectTransform m_groupTransform = null;
		[SerializeField]
		private TweenSettings m_tweenSettings = new TweenSettings(0.25f, Curve.QuadInOut);
		[SerializeField]
		private Button m_continueButton = null;


		protected override IEnumerator HideRoutine() {
			yield return m_groupTransform.ScaleTo(0f, m_tweenSettings);
		}
		protected override IEnumerator ShowRoutine() {
			m_groupTransform.localScale = Vector3.zero;
			yield return m_groupTransform.ScaleTo(1f, m_tweenSettings);
		}

		protected override void OnShowCompleted() {
			base.OnShowCompleted();
			m_continueButton.onClick.AddListener(HandleContinueButton);
		}
		protected override void OnShowStart() {
			UIMgr.Open<UIModalOverlay>();
		}

		protected override void OnHideStart() {
			base.OnHideStart();
			m_continueButton.onClick.RemoveListener(HandleContinueButton);
		}

		private void HandleContinueButton() {
			UIMgr.Close<UIModalBoardComplete>();
			UIMgr.Close<UIModalOverlay>();
			GameMgr.Events.Dispatch(GameEvents.BoardComplete);
			GameMgr.RunTrigger(GameTriggers.OnBoardComplete);
		}

	}

}
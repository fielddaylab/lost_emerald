using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {
	public class UICloseInspect : UIBase {
		[SerializeField]
		private Image m_inspectImage = null;
		[SerializeField]
		private Button m_closeButton = null;

		private static int REFERENCE_DIM = 600;

		protected override void OnShowStart() {
			base.OnShowStart();
			base.CanvasGroup.interactable = true;
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_closeButton.onClick.RemoveListener(HandleClose);
		}

		protected override void OnShowCompleted() {
			base.OnShowCompleted();
			m_closeButton.onClick.AddListener(HandleClose);
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}
		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
		}
		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}

		public void OpenCloseInspect(string imageId) {
			Sprite inspectSprite = GameDb.GetImageData(imageId);
			m_inspectImage.sprite = inspectSprite;
			m_inspectImage.rectTransform.sizeDelta = inspectSprite.rect.size.normalized * REFERENCE_DIM;
			// more space for landscape photos
			if (m_inspectImage.rectTransform.sizeDelta.x > m_inspectImage.rectTransform.sizeDelta.y) {
				m_inspectImage.rectTransform.sizeDelta *=
					m_inspectImage.rectTransform.sizeDelta.x
					/ m_inspectImage.rectTransform.sizeDelta.y;
			}
			UIMgr.Open<UICloseInspect>();
			GameMgr.Events.Dispatch(GameEvents.CloseInspect, imageId);
		}

		public void OpenCloseInspect(Sprite inspectSprite, string imageId) {
			m_inspectImage.sprite = inspectSprite;
			m_inspectImage.rectTransform.sizeDelta = inspectSprite.rect.size.normalized * REFERENCE_DIM;
			// more space for landscape photos
			if (m_inspectImage.rectTransform.sizeDelta.x > m_inspectImage.rectTransform.sizeDelta.y) {
				m_inspectImage.rectTransform.sizeDelta *=
					m_inspectImage.rectTransform.sizeDelta.x
					/ m_inspectImage.rectTransform.sizeDelta.y;
			}
			UIMgr.Open<UICloseInspect>();
			GameMgr.Events.Dispatch(GameEvents.CloseInspect, imageId);
		}

		private void HandleClose() {
			AudioSrcMgr.instance.PlayOneShot("click_map_close");
			UIMgr.Close<UICloseInspect>();
		}
	}
}

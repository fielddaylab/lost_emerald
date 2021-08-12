using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck {


	public class UIDiveScreen : UIBase {

		[SerializeField]
		private Button m_buttonAscend = null;
		[SerializeField]
		private Button m_buttonSurface = null;
		[SerializeField]
		private Button m_buttonJournal = null;
		[SerializeField]
		private Button m_buttonCameraActivate = null;
		[SerializeField]
		private Button m_buttonCameraDeactivate = null;
		[SerializeField]
		private Button m_buttonTakePhoto = null;
		[SerializeField]
		private Slider m_sliderZoom = null;
		[SerializeField]
		private CanvasGroup m_flashGroup = null;
		[SerializeField]
		private GameObject m_cameraGroup = null;



		protected override void OnShowCompleted() {
			base.OnShowCompleted();
			m_buttonAscend.onClick.AddListener(HandleAscend);
			m_buttonSurface.onClick.AddListener(HandleSurface);
			m_buttonJournal.onClick.AddListener(HandleJournal);
			m_buttonCameraActivate.onClick.AddListener(HandleCameraActivate);
			m_buttonCameraDeactivate.onClick.AddListener(HandleCameraDeactivate);
			m_buttonTakePhoto.onClick.AddListener(HandleTakePhoto);
			m_sliderZoom.onValueChanged.AddListener(HandleZoom);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_buttonAscend.onClick.RemoveListener(HandleAscend);
			m_buttonSurface.onClick.RemoveListener(HandleSurface);
			m_buttonJournal.onClick.RemoveListener(HandleJournal);
			m_buttonCameraActivate.onClick.RemoveListener(HandleCameraActivate);
			m_buttonCameraDeactivate.onClick.RemoveListener(HandleCameraDeactivate);
			m_buttonTakePhoto.onClick.RemoveListener(HandleTakePhoto);
			m_sliderZoom.onValueChanged.RemoveListener(HandleZoom);
		}


		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}
		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
			CanvasGroup.interactable = true;
		}

		private void HandleAscend() {

		}
		private void HandleSurface() {
			UIMgr.Close<UIDiveScreen>();
			SceneManager.LoadScene("Main");
			UIMgr.Open<UIOfficeScreen>();
			UIMgr.Open<UIPhoneNotif>();
		}
		private void HandleJournal() {

		}
		private void HandleCameraActivate() {
			SetCameraActive(true);
		}
		private void HandleCameraDeactivate() {
			SetCameraActive(false);
		}
		private void HandleTakePhoto() {

		}
		private void HandleZoom(float value) {

		}

		private void SetCameraActive(bool isCameraActive) {
			if (isCameraActive) {
				m_cameraGroup.SetActive(true);
				m_buttonCameraActivate.gameObject.SetActive(false);
				m_buttonCameraDeactivate.gameObject.SetActive(true);
			} else {
				m_cameraGroup.SetActive(false);
				m_buttonCameraActivate.gameObject.SetActive(true);
				m_buttonCameraDeactivate.gameObject.SetActive(false);
			}
		}


	}
}
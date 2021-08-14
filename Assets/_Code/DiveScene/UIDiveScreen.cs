using BeauRoutine;
using PotatoLocalization;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck {


	public sealed partial class UIDiveScreen : UIBase { 

		private interface IDiveScreen {

			DiveScreenState Previous { get; }
			
			void SetState(DiveScreenState state);
			void SetCameraActive(bool isActive);
			void SetNavigationActive(bool isActive);
			void FlashCamera(Action callback);
			void WaitForCameraTransitionEnd(Action callback);
			void AssignPreviousState(DiveScreenState state);
		}

		private class StateLinkage : IDiveScreen {

			public DiveScreenState Previous { 
				get { return m_owner.m_previousState; } 
			}

			private readonly UIDiveScreen m_owner;

			public StateLinkage(UIDiveScreen owner) {
				m_owner = owner;
			}
			public void FlashCamera(Action callback) {
				m_owner.FlashCamera(callback);
			}
			public void SetCameraActive(bool isActive) {
				m_owner.SetCameraActive(isActive);
			}
			public void SetNavigationActive(bool isActive) {
				m_owner.SetNavigationActive(isActive);
			}
			public void SetState(DiveScreenState state) {
				m_owner.SetState(state);
			}
			public void AssignPreviousState(DiveScreenState state) {
				m_owner.m_previousState = state;
			}
			public void WaitForCameraTransitionEnd(Action callback) {
				m_owner.WaitForCameraTransitionEnd(callback);
			}
		}


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

		[SerializeField, Header("Message Box")]
		private GameObject m_messageGroup = null; 
		[SerializeField]
		private LocalizedTextUGUI m_messageText = null;
		[SerializeField]
		private Button m_messageButton = null;
		[SerializeField]
		private LocalizedTextUGUI m_messageButtonText = null;
		[SerializeField]
		private float m_messageHiddenY = -88f;
		[SerializeField]
		private float m_messageShownY = 112f;

		private StateLinkage m_stateLink;
		private DiveScreenState m_currentState;
		private DiveScreenState m_previousState;
		private bool m_isAscended = true;
		private Routine m_flashRoutine;


		protected override void OnShowCompleted() {
			base.OnShowCompleted();
			m_isAscended = true;
			m_stateLink = new StateLinkage(this);
			SetState(new DiveMoving(m_stateLink));

			SetNavigationActive(false);

			m_buttonAscend.onClick.AddListener(HandleAscend);
			m_buttonSurface.onClick.AddListener(HandleSurface);
			m_buttonJournal.onClick.AddListener(HandleJournalOpen);
			m_buttonCameraActivate.onClick.AddListener(HandleCameraActivate);
			m_buttonCameraDeactivate.onClick.AddListener(HandleCameraDeactivate);
			m_buttonTakePhoto.onClick.AddListener(HandleAttemptPhoto);
			m_messageButton.onClick.AddListener(HandleCloseMessage);
			m_sliderZoom.onValueChanged.AddListener(HandleZoom);

			GameMgr.Events.Register(GameEvents.Dive.ConfirmPhoto, HandleConfirmPhoto);
			GameMgr.Events.Register<LocalizationKey>(GameEvents.Dive.ShowMessage, HandleShowMessage);
			GameMgr.Events.Register(GameEvents.Dive.LocationChanging, HandleLocationChanging);

			GameMgr.Events.Dispatch(GameEvents.Dive.NavigationDeactivated);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_buttonAscend.onClick.RemoveListener(HandleAscend);
			m_buttonSurface.onClick.RemoveListener(HandleSurface);
			m_buttonJournal.onClick.RemoveListener(HandleJournalOpen);
			m_buttonCameraActivate.onClick.RemoveListener(HandleCameraActivate);
			m_buttonCameraDeactivate.onClick.RemoveListener(HandleCameraDeactivate);
			m_buttonTakePhoto.onClick.RemoveListener(HandleAttemptPhoto);
			m_sliderZoom.onValueChanged.RemoveListener(HandleZoom);
		}


		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}
		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
			CanvasGroup.interactable = true;
		}

		#region Event Handlers

		private void HandleAscend() {
			
		}
		private void HandleSurface() {
			UIMgr.Close<UIDiveScreen>();
			SceneManager.LoadScene("Main");
			UIMgr.Open<UIOfficeScreen>();
			UIMgr.Open<UIPhoneNotif>();
		}
		private void HandleJournalOpen() {
			
		}
		private void HandleCameraActivate() {
			m_currentState.OnCameraActivate();
		}
		private void HandleCameraDeactivate() {
			m_currentState.OnCameraDeactivate();
		}
		private void HandleAttemptPhoto() {
			m_currentState.OnAttemptPhoto();
		}
		private void HandleConfirmPhoto() {
			m_currentState.OnConfirmPhoto();
		}

		private void HandleZoom(float value) {
			GameMgr.Events.Dispatch(GameEvents.Dive.CameraZoomChanged, value);
		}

		private void HandleShowMessage(LocalizationKey text) {
			m_currentState.OnShowMessage(text);
		}
		private void HandleCloseMessage() {
			m_currentState.OnCloseMessage();
		}

		private void HandleLocationChanging() {
			m_currentState.OnLocationChange();
		}

		#endregion

		#region IDiveScreen

		private void SetState(DiveScreenState state) {
			if (state == null) {
				throw new NullReferenceException("UIDiveScreen cannot set its state to a null value!");
			}
			if (m_currentState != null) {
				m_currentState.OnEnd();
			}
			m_currentState = state;
			Debug.Log("State is: " + m_currentState.GetType().Name);
			m_currentState.OnStart();
		}

		private void SetCameraActive(bool isCameraActive) {
			if (isCameraActive) {
				m_cameraGroup.SetActive(true);
				m_buttonCameraDeactivate.gameObject.SetActive(true);
			} else {
				m_sliderZoom.value = 0f;
				m_cameraGroup.SetActive(false);
				m_buttonCameraDeactivate.gameObject.SetActive(false);
			}
		}

		private void SetNavigationActive(bool isNavActive) {
			if (isNavActive) {
				m_buttonCameraActivate.gameObject.SetActive(true);
				m_buttonAscend.gameObject.SetActive(!m_isAscended);
				m_buttonSurface.gameObject.SetActive(m_isAscended);
				m_buttonJournal.gameObject.SetActive(true);

			} else {
				m_buttonCameraActivate.gameObject.SetActive(false);
				m_buttonAscend.gameObject.SetActive(false);
				m_buttonSurface.gameObject.SetActive(false);
				m_buttonJournal.gameObject.SetActive(false);
			}
		}
		private void FlashCamera(Action callback) {
			m_flashRoutine.Replace(this, FlashCameraRoutine()).OnComplete(callback).OnStop(callback);
		}

		private void WaitForCameraTransitionEnd(Action callback) {
			Routine.Start(this, WaitForCameraTransitionEndRoutine()).OnComplete(callback).OnStop(callback);
		}

		private IEnumerator FlashCameraRoutine() {
			m_flashGroup.alpha = 1f;
			yield return m_flashGroup.FadeTo(0f, 1.5f);
		}

		private IEnumerator WaitForCameraTransitionEndRoutine() {
			bool isTransitionComplete = false;
			Action handleTransitionComplete = () => {
				isTransitionComplete = true;
			};
			GameMgr.Events.Register(GameEvents.Dive.CameraTransitionComplete, handleTransitionComplete);
			while (!isTransitionComplete) {
				yield return null;
			}
			GameMgr.Events.Deregister(GameEvents.Dive.CameraTransitionComplete, handleTransitionComplete);
		}


		#endregion



	}
}
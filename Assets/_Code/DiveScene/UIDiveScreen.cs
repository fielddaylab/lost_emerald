using BeauRoutine;
using BeauUtil;
using PotatoLocalization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck {


	public sealed partial class UIDiveScreen : UIBase {

		#region Classes

		private interface IDiveScreen {
			DiveScreenState Previous { get; }
			bool IsAtAscendNode { get; set; }

			void SetState(DiveScreenState state);
			void SetCameraActive(bool isActive);
			void SetNavigationActive(bool isActive);
			void SetCameraZoom(float value);
			void FlashCamera(Action callback);
			void WaitForCameraTransitionEnd(Action callback);
			void AssignPreviousState(DiveScreenState state);
			void ShowMessageBox(LocalizationKey text, LocalizationKey button);
			void HideMessageBox();
			void ShowJournal();
			void HideJournal();
		}

		private class StateLinkage : IDiveScreen {

			public DiveScreenState Previous { 
				get { return m_owner.m_previousState; } 
			}
			public bool IsAtAscendNode { 
				get { return m_owner.m_isAscended; }
				set { m_owner.m_isAscended = value; }
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
			public void SetCameraZoom(float value) {
				m_owner.m_sliderZoom.value = value;
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
			public void ShowMessageBox(LocalizationKey text, LocalizationKey button) {
				m_owner.ShowMessageBox(text, button);
			}
			public void HideMessageBox() {
				m_owner.HideMessageBox();
			}
			public void ShowJournal() {
				m_owner.ShowJournal();
			}
			public void HideJournal() {
				m_owner.HideJournal();
			}
		}

		#endregion

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
		private RectTransform m_messageGroup = null; 
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

		[SerializeField, Header("Journal")]
		private RectTransform m_journalGroup = null;
		[SerializeField]
		private float m_journalHiddenX = -400f;
		[SerializeField]
		private float m_journalShownX = 0f;
		[SerializeField]
		private DiveJournalItem m_journalItemPrefab = null;

		[SerializeField]
		private RectTransform m_journalChecklist = null;
		[SerializeField]
		private Button m_journalCloseButton = null;

		private Routine m_journalShowHideRoutine;

		


		private StateLinkage m_stateLink;
		private DiveScreenState m_currentState;
		private DiveScreenState m_previousState;
		private bool m_isAscended = true;
		private Routine m_flashRoutine;
		private Routine m_messageRoutine;

		#region UIBase

		protected override void OnShowCompleted() {
			base.OnShowCompleted();
			m_isAscended = true;
			m_stateLink = new StateLinkage(this);
			SetState(new DiveMoving(m_stateLink));

			SetNavigationActive(false);

			m_buttonAscend.onClick.AddListener(HandleAscendButton);
			m_buttonSurface.onClick.AddListener(HandleSurfaceButton);
			m_buttonJournal.onClick.AddListener(HandleJournalOpenButton);
			m_buttonCameraActivate.onClick.AddListener(HandleCameraActivateButton);
			m_buttonCameraDeactivate.onClick.AddListener(HandleCameraDeactivateButton);
			m_buttonTakePhoto.onClick.AddListener(HandleAttemptPhotoButton);
			m_journalCloseButton.onClick.AddListener(HandleJournalCloseButton);
			m_messageButton.onClick.AddListener(HandleCloseMessage);
			m_sliderZoom.onValueChanged.AddListener(HandleZoomSlider);


			GameMgr.Events.Register<StringHash32>(GameEvents.Dive.ConfirmPhoto, HandleConfirmPhoto);
			GameMgr.Events.Register<LocalizationKey>(GameEvents.Dive.ShowMessage, HandleShowMessage);
			GameMgr.Events.Register<bool>(GameEvents.Dive.LocationChanging, HandleLocationChanging);
			GameMgr.Events.Register<List<DivePointOfInterest>>(GameEvents.Dive.SendPhotoList, HandlePhotoListSent);

			//Routine.Delay(() => { GameMgr.Events.Dispatch(GameEvents.Dive.NavigationDeactivated); }, 0.1f);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_buttonAscend.onClick.RemoveListener(HandleAscendButton);
			m_buttonSurface.onClick.RemoveListener(HandleSurfaceButton);
			m_buttonJournal.onClick.RemoveListener(HandleJournalOpenButton);
			m_buttonCameraActivate.onClick.RemoveListener(HandleCameraActivateButton);
			m_buttonCameraDeactivate.onClick.RemoveListener(HandleCameraDeactivateButton);
			m_buttonTakePhoto.onClick.RemoveListener(HandleAttemptPhotoButton);
			m_messageButton.onClick.RemoveListener(HandleCloseMessage);
			m_sliderZoom.onValueChanged.RemoveListener(HandleZoomSlider);
			m_journalCloseButton.onClick.RemoveListener(HandleJournalCloseButton);

			GameMgr.Events.Deregister<StringHash32>(GameEvents.Dive.ConfirmPhoto, HandleConfirmPhoto);
			GameMgr.Events.Deregister<LocalizationKey>(GameEvents.Dive.ShowMessage, HandleShowMessage);
			GameMgr.Events.Deregister<bool>(GameEvents.Dive.LocationChanging, HandleLocationChanging);
			GameMgr.Events.Deregister<List<DivePointOfInterest>>(GameEvents.Dive.SendPhotoList, HandlePhotoListSent);
		}


		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}
		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
			CanvasGroup.interactable = true;
		}

		#endregion

		#region Event Handlers

		private void HandlePhotoListSent(List<DivePointOfInterest> list) {
			for (int ix = 0; ix < m_journalChecklist.childCount; ix++) {
				Destroy(m_journalChecklist.GetChild(ix).gameObject);
			}
			foreach (DivePointOfInterest poi in list) {
				DiveJournalItem item = Instantiate(m_journalItemPrefab, m_journalChecklist);
				item.transform.localScale = Vector3.one;
				item.SetChecked(GameMgr.State.CurrentLevel.IsEvidenceUnlocked(poi.EvidenceUnlock));
				item.SetText(poi.PhotoName);
			}
		}

		private void HandleAscendButton() {
			m_currentState.OnAscend();
		}
		private void HandleSurfaceButton() {
			m_currentState.OnSurface();
		}
		private void HandleJournalOpenButton() {
			m_currentState.OnOpenJournal();
		}
		private void HandleJournalCloseButton() {
			m_currentState.OnCloseJournal();
		}
		private void HandleCameraActivateButton() {
			m_currentState.OnCameraActivate();
		}
		private void HandleCameraDeactivateButton() {
			m_currentState.OnCameraDeactivate();
		}
		private void HandleAttemptPhotoButton() {
			m_currentState.OnAttemptPhoto();
		}
		private void HandleConfirmPhoto(StringHash32 evidence) {
			m_currentState.OnConfirmPhoto(evidence);
		}

		private void HandleZoomSlider(float value) {
			GameMgr.Events.Dispatch(GameEvents.Dive.CameraZoomChanged, value);
			//TODO: split these up into start, middle, and end sounds
			if (value > 0)
			{
				AudioSrcMgr.instance.PlayOneShot("zoom_in");
			}
			else if (value < 0)
			{
				AudioSrcMgr.instance.PlayOneShot("zoom_out");
			}
		}

		private void HandleShowMessage(LocalizationKey text) {
			m_currentState.OnShowMessage(text);
		}
		private void HandleCloseMessage() {
			m_currentState.OnCloseMessage();
			AudioSrcMgr.instance.PlayOneShot("click_dialog_continue");
		}

		private void HandleLocationChanging(bool isAscendNode) {			
			m_currentState.OnLocationChange(isAscendNode);
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
			m_currentState.OnStart();
		}

		private void SetCameraActive(bool isCameraActive) {
			if (isCameraActive) {
				m_cameraGroup.SetActive(true);
				m_buttonCameraDeactivate.gameObject.SetActive(true);
				m_buttonJournal.gameObject.SetActive(true);
			} else {
				m_cameraGroup.SetActive(false);
				m_buttonCameraDeactivate.gameObject.SetActive(false);
				m_buttonJournal.gameObject.SetActive(false);
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
		private void ShowMessageBox(LocalizationKey message, LocalizationKey buttonText) {
			m_messageText.Key = message;
			m_messageButtonText.Key = buttonText;
			m_messageGroup.anchoredPosition = new Vector2(m_messageGroup.anchoredPosition.x, m_messageHiddenY);
			m_messageRoutine.Replace(this, m_messageGroup.AnchorPosTo(m_messageShownY, 0.25f, Axis.Y).Ease(Curve.QuadOut));
		}
		private void HideMessageBox() {
			m_messageRoutine.Replace(this, m_messageGroup.AnchorPosTo(m_messageHiddenY, 0.25f, Axis.Y).Ease(Curve.QuadOut));
		}

		private void ShowJournal() {
			GameMgr.Events.Dispatch(GameEvents.Dive.RequestPhotoList);
			m_buttonJournal.gameObject.SetActive(false);
			m_journalShowHideRoutine.Replace(this, ((RectTransform)m_journalGroup).AnchorPosTo(m_journalShownX, 0.25f, Axis.X).Ease(Curve.BackInOut));
		}
		private void HideJournal() {
			m_buttonJournal.gameObject.SetActive(true);
			m_journalShowHideRoutine.Replace(this, ((RectTransform)m_journalGroup).AnchorPosTo(m_journalHiddenX, 0.25f, Axis.X).Ease(Curve.QuadOut));
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
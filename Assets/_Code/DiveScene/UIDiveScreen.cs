using BeauRoutine;
using BeauUtil;
using PotatoLocalization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {


	public sealed partial class UIDiveScreen : UIBase {

		#region Classes

		private interface IDiveScreen {
			DiveScreenState Previous { get; }
			bool IsAtAscendNode { get; set; }
			bool HasDescended { get; set; }
			void SetState(DiveScreenState state);
			void SetCameraActive(bool isActive);
			void SetNavigationActive(bool isActive);
			void SetCameraZoom(float value);
			void FlashCamera(Action callback);
			void WaitForCameraTransitionEnd(Action callback);
			void WaitForMessageClosed(Action callback);
			void AssignPreviousState(DiveScreenState state);
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
			public bool HasDescended {
				get { return m_owner.m_hasDescended; }
				set { m_owner.m_hasDescended = value; }
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
			public void WaitForMessageClosed(Action callback) {
				m_owner.WaitForMessageClosed(callback);
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


		private StateLinkage m_stateLink;
		private DiveScreenState m_currentState;
		private DiveScreenState m_previousState;
		private bool m_isAscended = true;
		private bool m_hasDescended = false;
		private Routine m_flashRoutine;
		private Routine m_journalShowHideRoutine;

		#region UIBase

		protected override void OnShowStart() {
			base.OnShowStart();

			m_isAscended = true;
			m_hasDescended = false;
			m_stateLink = new StateLinkage(this);
			SetNavigationActive(false);

			SetState(new DiveMoving(m_stateLink));
		}

		protected override void OnShowCompleted() {
			base.OnShowCompleted();
			
			GameMgr.RunTrigger(GameTriggers.OnEnterDive);

			m_buttonAscend.onClick.AddListener(HandleAscendButton);
			m_buttonSurface.onClick.AddListener(HandleSurfaceButton);
			m_buttonJournal.onClick.AddListener(HandleJournalOpenButton);
			m_buttonCameraActivate.onClick.AddListener(HandleCameraActivateButton);
			m_buttonCameraDeactivate.onClick.AddListener(HandleCameraDeactivateButton);
			m_buttonTakePhoto.onClick.AddListener(HandleAttemptPhotoButton);
			m_journalCloseButton.onClick.AddListener(HandleJournalCloseButton);
			m_sliderZoom.onValueChanged.AddListener(HandleZoomSlider);

			GameMgr.Events.Register<StringHash32>(GameEvents.Dive.ConfirmPhoto, HandleConfirmPhoto);
			GameMgr.Events.Register<bool>(GameEvents.Dive.LocationChanging, HandleLocationChanging);
			GameMgr.Events.Register<List<DivePointOfInterest>>(GameEvents.Dive.SendPhotoList, HandlePhotoListSent);
			GameMgr.Events.Register(GameEvents.Dive.ShowMessage, HandleShowMessage);
			GameMgr.Events.Register(GameEvents.Dive.NavigationActivated, HandleNavActivated, this);

		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_buttonAscend.onClick.RemoveListener(HandleAscendButton);
			m_buttonSurface.onClick.RemoveListener(HandleSurfaceButton);
			m_buttonJournal.onClick.RemoveListener(HandleJournalOpenButton);
			m_buttonCameraActivate.onClick.RemoveListener(HandleCameraActivateButton);
			m_buttonCameraDeactivate.onClick.RemoveListener(HandleCameraDeactivateButton);
			m_buttonTakePhoto.onClick.RemoveListener(HandleAttemptPhotoButton);
			m_sliderZoom.onValueChanged.RemoveListener(HandleZoomSlider);
			m_journalCloseButton.onClick.RemoveListener(HandleJournalCloseButton);

			GameMgr.Events.Deregister<StringHash32>(GameEvents.Dive.ConfirmPhoto, HandleConfirmPhoto);
			GameMgr.Events.Deregister<bool>(GameEvents.Dive.LocationChanging, HandleLocationChanging);
			GameMgr.Events.Deregister<List<DivePointOfInterest>>(GameEvents.Dive.SendPhotoList, HandlePhotoListSent);
			GameMgr.Events.Deregister(GameEvents.Dive.ShowMessage, HandleShowMessage);
		}


		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}
		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
			CanvasGroup.interactable = true;
		}
		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}

		#endregion

		#region Event Handlers

		private void HandlePhotoListSent(List<DivePointOfInterest> list) {
			for (int ix = 0; ix < m_journalChecklist.childCount; ix++) {
				Destroy(m_journalChecklist.GetChild(ix).gameObject);
			}
			List<LocalizationKey> itemLocalizations = new List<LocalizationKey>();
			foreach (DivePointOfInterest poi in list) {
				if (itemLocalizations.Contains(poi.PhotoName)) { continue; }
				DiveJournalItem item = Instantiate(m_journalItemPrefab, m_journalChecklist);
				item.transform.localScale = Vector3.one;
				item.SetChecked(GameMgr.State.CurrentLevel.IsEvidenceUnlocked(poi.EvidenceUnlock));
				item.SetText(poi.PhotoName);
				itemLocalizations.Add(poi.PhotoName);
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
			AudioSrcMgr.instance.PlayOneShot("click_notebook");
		}
		private void HandleJournalCloseButton() {
			m_currentState.OnCloseJournal();
			AudioSrcMgr.instance.PlayOneShot("click_notebook");
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

		private void HandleShowMessage() {
			m_currentState.OnShowMessages();
		}

		private void HandleLocationChanging(bool isAscendNode) {			
			m_currentState.OnLocationChange(isAscendNode);
		}

		private void HandleNavActivated() {
			m_sliderZoom.value = 0;
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
				m_buttonSurface.gameObject.SetActive(m_isAscended && m_hasDescended);
				m_buttonJournal.gameObject.SetActive(true);

			} else {
				m_buttonCameraActivate.gameObject.SetActive(false);
				m_buttonAscend.gameObject.SetActive(false);
				m_buttonSurface.gameObject.SetActive(false);
				m_buttonJournal.gameObject.SetActive(false);
			}
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

		private void WaitForMessageClosed(Action callback) {
			Routine.Start(this, WaitForMessageClosedRoutine()).OnComplete(callback).OnStop(callback);
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

		private IEnumerator WaitForMessageClosedRoutine() {
			bool isMessageClosed = false;
			Action handleMessageClosed = () => {
				isMessageClosed = true;
			};
			GameMgr.Events.Register(GameEvents.DialogClosed, handleMessageClosed);
			while (!isMessageClosed) {
				yield return null;
			}
			GameMgr.Events.Deregister(GameEvents.DialogClosed, handleMessageClosed);
		}


		#endregion
 
	}
}

using BeauUtil;
using PotatoLocalization;
using System;
using UnityEngine.SceneManagement;

namespace Shipwreck {


	public sealed partial class UIDiveScreen : UIBase { // DiveScreenStates.cs

		private abstract class DiveScreenState {

			protected IDiveScreen Screen { get; private set; }

			public DiveScreenState(IDiveScreen screen) {
				Screen = screen;
			}

			public virtual void OnStart() { }
			public virtual void OnEnd() { }
			public virtual void OnAscend() { }
			public virtual void OnSurface() { }
			public virtual void OnCameraActivate() { }
			public virtual void OnCameraDeactivate() { }
			public virtual void OnShowMessage(LocalizationKey key) { }
			public virtual void OnAttemptPhoto() { }
			public virtual void OnConfirmPhoto(StringHash32 evidence) { }
			public virtual void OnCloseMessage() { }
			public virtual void OnLocationChange(bool isAscendNode) { }
			public virtual void OnOpenJournal() { }
			public virtual void OnCloseJournal() { }
		}

		private class DiveNavigation : DiveScreenState {
			public DiveNavigation(IDiveScreen screen) : base(screen) {
			}

			public override void OnStart() {
				Screen.SetNavigationActive(true);
				GameMgr.Events.Dispatch(GameEvents.Dive.NavigationActivated);
			}
			public override void OnEnd() {
				Screen.SetNavigationActive(false);
				Screen.AssignPreviousState(this);
				GameMgr.Events.Dispatch(GameEvents.Dive.NavigationDeactivated);
			}
			public override void OnLocationChange(bool isAscendNode) {
				Screen.IsAtAscendNode = isAscendNode;
				Screen.SetState(new DiveMoving(Screen));
			}
			public override void OnCameraActivate() {
				Screen.SetState(new DiveCamera(Screen));
			}
			public override void OnOpenJournal() {
				Screen.SetState(new DiveJournal(Screen));
			}
			public override void OnAscend() {
				if (!Screen.IsAtAscendNode) {
					GameMgr.Events.Dispatch(GameEvents.Dive.NavigateToAscendNode);
				}
			}
			public override void OnSurface() {
				if (Screen.IsAtAscendNode) {
					UIMgr.Close<UIDiveScreen>();
					SceneManager.LoadScene("Main");
					UIMgr.Open<UIOfficeScreen>();
					UIMgr.Open<UIPhoneNotif>();
				}
			}

		}
		private class DiveMoving : DiveScreenState {
			public DiveMoving(IDiveScreen screen) : base(screen) {
			}

			public override void OnStart() {
				Screen.WaitForCameraTransitionEnd(HandleTransitionEnded);
			}
			private void HandleTransitionEnded() {
				if (GameMgr.State.HasTakenTopDownPhoto()) {
					Screen.SetState(new DiveNavigation(Screen));
				} else {
					Screen.SetState(new DiveTutorialNav(Screen));
				}
			}
		}
		private class DiveCamera : DiveScreenState {
			public DiveCamera(IDiveScreen screen) : base(screen) {
			}
			public override void OnStart() {
				Screen.SetCameraActive(true);
				GameMgr.Events.Dispatch(GameEvents.Dive.CameraActivated);
			}
			public override void OnEnd() {
				Screen.SetCameraActive(false);
				Screen.AssignPreviousState(this);
				GameMgr.Events.Dispatch(GameEvents.Dive.CameraDeactivated);
			}
			public override void OnAttemptPhoto() {
				GameMgr.Events.Dispatch(GameEvents.Dive.AttemptPhoto);
			}
			public override void OnConfirmPhoto(StringHash32 evidence) {
				GameMgr.UnlockEvidence(evidence);
				Screen.SetState(new DiveTakePhoto(Screen));
			}
			public override void OnCameraDeactivate() {
				Screen.SetCameraZoom(0f);
				Screen.SetState(new DiveNavigation(Screen));
			}
			public override void OnShowMessage(LocalizationKey key) {
				Screen.SetState(new DiveMessage(Screen,key,new LocalizationKey("UI/General/Continue"),false));
			}
			public override void OnOpenJournal() {
				Screen.SetState(new DiveJournal(Screen));
			}
		}
		private class DiveMessage : DiveScreenState {

			private LocalizationKey m_text;
			private LocalizationKey m_button;
			private bool m_showJournal;

			public DiveMessage(IDiveScreen screen, LocalizationKey textKey, LocalizationKey buttonKey, bool showJournal) : base(screen) {
				m_text = textKey;
				m_button = buttonKey;
				m_showJournal = showJournal;
			}
			public override void OnStart() {
				Screen.ShowMessageBox(m_text,m_button);
			}
			public override void OnCloseMessage() {
				Screen.HideMessageBox();
				if (m_showJournal) {
					Screen.SetState(new DiveJournal(Screen));
				} else {
					Screen.SetState(Screen.Previous);
				}
				
			}
		}
		private class DiveTakePhoto : DiveScreenState {

			private LocalizationKey m_cachedMessage;

			public DiveTakePhoto(IDiveScreen screen) : base(screen) {
			}

			public override void OnStart() {
				Screen.FlashCamera(HandleFlashComplete);
			}
			public override void OnShowMessage(LocalizationKey key) {
				m_cachedMessage = key;
			}
			private void HandleFlashComplete() {
				if (m_cachedMessage.Equals(LocalizationKey.Empty)) {
					Screen.SetState(new DiveCamera(Screen));
				} else {
					if (Screen.Previous.GetType() == typeof(DiveTutorialCamera)) {
						if (GameMgr.State.HasTakenTopDownPhoto()) {
							Screen.AssignPreviousState(new DiveCamera(Screen));
						}
					}
					Screen.SetState(new DiveMessage(Screen, m_cachedMessage, new LocalizationKey("UI/Dive/SavePhoto"),true));
				}
			}

		}
		private class DiveJournal : DiveScreenState {
			public DiveJournal(IDiveScreen screen) : base(screen) {
			}
			public override void OnStart() {
				Screen.ShowJournal();
			}
			public override void OnEnd() {
				Screen.HideJournal();
			}
			public override void OnCloseJournal() {
				Screen.SetState(Screen.Previous);
			}
		}
		private class DiveTutorialNav : DiveScreenState {
			public DiveTutorialNav(IDiveScreen screen) : base(screen) {
			}
			public override void OnStart() {
				Screen.SetNavigationActive(true);
			}
			public override void OnEnd() {
				Screen.SetNavigationActive(false);
				Screen.AssignPreviousState(this);
			}
			public override void OnCameraActivate() {
				Screen.SetState(new DiveTutorialCamera(Screen));
			}
			public override void OnOpenJournal() {
				Screen.SetState(new DiveJournal(Screen));
			}
			public override void OnSurface() {
				if (Screen.IsAtAscendNode) {
					UIMgr.Close<UIDiveScreen>();
					SceneManager.LoadScene("Main");
					UIMgr.Open<UIOfficeScreen>();
					UIMgr.Open<UIPhoneNotif>();
				}
			}
		}
		private class DiveTutorialCamera : DiveScreenState {
			public DiveTutorialCamera(IDiveScreen screen) : base(screen) {
			}
			public override void OnStart() {
				Screen.SetCameraActive(true);
				GameMgr.Events.Dispatch(GameEvents.Dive.CameraActivated);
			}
			public override void OnEnd() {
				Screen.SetCameraActive(false);
				Screen.AssignPreviousState(this);
				GameMgr.Events.Dispatch(GameEvents.Dive.CameraDeactivated);
			}
			public override void OnAttemptPhoto() {
				GameMgr.Events.Dispatch(GameEvents.Dive.AttemptPhoto);
			}
			public override void OnConfirmPhoto(StringHash32 evidence) {
				GameMgr.UnlockEvidence(evidence);
				Screen.SetState(new DiveTakePhoto(Screen));
			}
			public override void OnCameraDeactivate() {
				Screen.SetCameraZoom(0f);
				Screen.SetState(new DiveTutorialNav(Screen));
			}
			public override void OnShowMessage(LocalizationKey key) {
				Screen.SetState(new DiveMessage(Screen, key, new LocalizationKey("UI/General/Continue"), false));
			}
			public override void OnOpenJournal() {
				Screen.SetState(new DiveJournal(Screen));
			}
		}

	}

}
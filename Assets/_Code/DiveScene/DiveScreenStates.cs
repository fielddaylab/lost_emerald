using PotatoLocalization;

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
			public virtual void OnConfirmPhoto() { }
			public virtual void OnCloseMessage() { }
			public virtual void OnLocationChange() { }
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
			public override void OnLocationChange() {
				Screen.SetState(new DiveMoving(Screen));
			}
			public override void OnCameraActivate() {
				Screen.SetState(new DiveCamera(Screen));
			}

		}
		private class DiveMoving : DiveScreenState {
			public DiveMoving(IDiveScreen screen) : base(screen) {
			}

			public override void OnStart() {
				Screen.WaitForCameraTransitionEnd(HandleTransitionEnded);
			}
			private void HandleTransitionEnded() {
				Screen.SetState(new DiveNavigation(Screen));
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
			public override void OnConfirmPhoto() {
				Screen.SetState(new DiveTakePhoto(Screen));
			}
			public override void OnCameraDeactivate() {
				Screen.SetState(new DiveNavigation(Screen));
			}
			public override void OnShowMessage(LocalizationKey key) {
				Screen.SetState(new DiveMessage(Screen,key,new LocalizationKey("UI/General/Continue")));
			}
		}
		private class DiveMessage : DiveScreenState {

			private LocalizationKey m_text;
			private LocalizationKey m_button;
			public DiveMessage(IDiveScreen screen, LocalizationKey textKey, LocalizationKey buttonKey) : base(screen) {
				m_text = textKey;
				m_button = buttonKey;
			}
			public override void OnStart() {
				
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
					Screen.SetState(new DiveMessage(Screen, m_cachedMessage, new LocalizationKey("UI/Dive/SavePhoto")));
				}
			}

		}


	}

}
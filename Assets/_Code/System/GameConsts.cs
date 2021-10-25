using BeauUtil;

namespace Shipwreck {
    public static class GameEvents {
        
        public static readonly StringHash32 PhoneNotification = "PhoneNotification";
        public static readonly StringHash32 ContactUnlocked = "ContactUnlocked";
		public static readonly StringHash32 LevelUnlocked = "LevelUnlocked";
		public static readonly StringHash32 EvidenceUnlocked = "EvidenceUnlocked";
		public static readonly StringHash32 ChainSolved = "ChainSolved";

        public static readonly StringHash32 PhoneOpened = "PhoneOpened";
        public static readonly StringHash32 PhoneClosed = "PhoneClosed";
        public static readonly StringHash32 DialogOpened = "DialogOpened";
        public static readonly StringHash32 DialogClosed = "DialogClosed";

		public static readonly StringHash32 BoardComplete = "BoardComplete";
		public static readonly StringHash32 CutsceneComplete = "CutsceneComplete";
		public static readonly StringHash32 CaseClosed = "CaseClosedOpened";
		public static readonly StringHash32 LocationDiscovered = "LocationDiscovered";

		public static class Dive {
			public static readonly StringHash32 NavigationActivated = "NavigationActivated";
			public static readonly StringHash32 NavigationDeactivated = "NavigationDeactivated";
			public static readonly StringHash32 NavigateToAscendNode = "NavigateToAscendNode";
			public static readonly StringHash32 LocationChanging = "LocationChanging";
			public static readonly StringHash32 CameraActivated = "CameraActivated";
			public static readonly StringHash32 CameraDeactivated = "CameraDeactivated";
			public static readonly StringHash32 CameraZoomChanged = "CameraZoomChanged";
			public static readonly StringHash32 CameraTransitionComplete = "CameraTransitionComplete";
			public static readonly StringHash32 ShowMessage = "ShowDiveMessage";
			public static readonly StringHash32 AttemptPhoto = "AttemptPhoto";
			public static readonly StringHash32 ConfirmPhoto = "ConfirmPhoto";
			public static readonly StringHash32 RequestPhotoList = "RequestPhotoList";
			public static readonly StringHash32 SendPhotoList = "ConfirmPhotoList";
		}
	}

    public static class GameTriggers {

		public static class Dive {
			public static readonly StringHash32 OnPhotoAlreadyTaken = "OnPhotoAlreadyTaken";
			public static readonly StringHash32 OnZoomIn = "OnZoomIn";
			public static readonly StringHash32 OnZoomOut = "OnZoomOut";
			public static readonly StringHash32 OnNothingOfInterest = "OnNothingOfInterest";
		}

        public static readonly StringHash32 OnContactAdded = "OnContactAdded";
		public static readonly StringHash32 OnLevelUnlock = "OnLevelUnlock";
		public static readonly StringHash32 OnChainSolved = "OnChainSolved";
		public static readonly StringHash32 OnEvidenceUnlock = "OnEvidenceUnlock";
        public static readonly StringHash32 OnContactText = "OnContactText";
        public static readonly StringHash32 OnEnterOffice = "OnEnterOffice";
		public static readonly StringHash32 OnDialogClosed = "OnDialogClosed";
		public static readonly StringHash32 OnBoardComplete = "OnBoardComplete";
		public static readonly StringHash32 OnCaseClosed = "OnCaseClosed";
		public static readonly StringHash32 OnFindReya = "OnFindReya";
		public static readonly StringHash32 OnEnterDive = "OnEnterDive";
		public static readonly StringHash32 OnEnterSonar = "OnEnterSonar";
		public static readonly StringHash32 OnMowCompleted = "OnMowCompleted";
	}

    public static class GameVars {
        
        public static readonly StringHash32 LastNotifiedContactId = "lastNotifiedContactId";
    }
}
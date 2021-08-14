﻿using BeauUtil;

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

		public static class Dive {
			public static readonly StringHash32 NavigationActivated = "NavigationActivated";
			public static readonly StringHash32 NavigationDeactivated = "NavigationDeactivated";
			public static readonly StringHash32 LocationChanging = "LocationChanging";
			public static readonly StringHash32 CameraActivated = "CameraActivated";
			public static readonly StringHash32 CameraDeactivated = "CameraDeactivated";
			public static readonly StringHash32 CameraZoomChanged = "CameraZoomChanged";
			public static readonly StringHash32 CameraTransitionComplete = "CameraTransitionComplete";
			public static readonly StringHash32 ShowMessage = "ShowDiveMessage";
			public static readonly StringHash32 AttemptPhoto = "AttemptPhoto";
			public static readonly StringHash32 ConfirmPhoto = "ConfirmPhoto";
		}
	}

    public static class GameTriggers {

        public static readonly StringHash32 OnContactAdded = "OnContactAdded";
		public static readonly StringHash32 OnLevelUnlock = "OnLevelUnlock";
		public static readonly StringHash32 OnChainSolved = "OnChainSolved";
		public static readonly StringHash32 OnEvidenceUnlock = "OnEvidenceUnlock";
        public static readonly StringHash32 OnContactText = "OnContactText";
        public static readonly StringHash32 OnEnterOffice = "OnEnterOffice";
    }

    public static class GameVars {
        
        public static readonly StringHash32 LastNotifiedContactId = "lastNotifiedContactId";
    }
}
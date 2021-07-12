using BeauUtil;

namespace Shipwreck {
    public static class GameEvents {
        
        public static readonly StringHash32 PhoneNotification = "PhoneNotification";
        public static readonly StringHash32 ContactUnlocked = "ContactUnlocked";

        public static readonly StringHash32 PhoneOpened = "PhoneOpened";
        public static readonly StringHash32 PhoneClosed = "PhoneClosed";
        public static readonly StringHash32 DialogOpened = "DialogOpened";
        public static readonly StringHash32 DialogClosed = "DialogClosed";
    }

    public static class GameTriggers {

        public static readonly StringHash32 OnContactAdded = "OnContactAdded";
        public static readonly StringHash32 OnContactText = "OnContactText";
        public static readonly StringHash32 OnEnterOffice = "OnEnterOffice";
    }

    public static class GameVars {
        
        public static readonly StringHash32 LastNotifiedContactId = "lastNotifiedContactId";
    }
}
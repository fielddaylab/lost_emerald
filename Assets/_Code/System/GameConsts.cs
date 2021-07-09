using BeauUtil;

namespace Shipwreck {
    public static class GameEvents {
        
        public static readonly StringHash32 PhoneNotification = "PhoneNotification";
        public static readonly StringHash32 ContactUnlocked = "ContactUnlocked";
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
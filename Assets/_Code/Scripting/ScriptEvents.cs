using BeauUtil;

namespace Shipwreck {

	public static class ScriptEvents {

		public static class Global {

			public static readonly StringHash32 Wait = "wait";
		}

		public static class Dialog {

			public static readonly StringHash32 Target = "set-target";
			public static readonly StringHash32 Image = "image";
			public static readonly StringHash32 HideImage = "hide-image";
			public static readonly StringHash32 Object = "object";
			public static readonly StringHash32 HideObject = "hide-object";

		}

	}

}
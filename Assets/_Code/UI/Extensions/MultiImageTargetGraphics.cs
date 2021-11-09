using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {
	/// <summary>
	/// Used with MultiImageButton.cs to tint multiple graphics with one button
	/// (specifically, for the level selection assets)
	/// Source script: https://forum.unity.com/threads/tint-multiple-targets-with-single-button.279820/
	/// </summary>
	public class MultiImageTargetGraphics : MonoBehaviour {
		[SerializeField] private Graphic[] targetGraphics;

		public Graphic[] GetTargetGraphics => targetGraphics;
	}
}

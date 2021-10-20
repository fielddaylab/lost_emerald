using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {
	public class LevelMarker : MonoBehaviour {

		[SerializeField]
		private Image m_image;
		[SerializeField]
		private Button m_button;

		public Button Button {
			get { return m_button; }
		}

		private Routine m_colorRoutine;

		public void SetColor(Color color) {
			m_colorRoutine.Replace(this, ColorTo(color));
		}

		private IEnumerator ColorTo(Color color) {
			yield return m_image.ColorTo(color, 0.1f).Ease(Curve.QuadOut);
		}

		public void SetSprite(Sprite sprite) {
			m_image.sprite = sprite;
			m_image.SetNativeSize();
			// hack? ideally images would be uploaded as the correct size
			m_image.transform.SetScale(new Vector3(.5f, .5f, 1));
		}

	}
}


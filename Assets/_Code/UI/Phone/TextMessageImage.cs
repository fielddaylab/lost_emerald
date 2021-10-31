using BeauRoutine;
using PotatoLocalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class TextMessageImage : MonoBehaviour {

		[SerializeField]
		private TextMessageLayout m_layout = null;
		[SerializeField]
		private Image m_bodyImage = null;
		[SerializeField]
		private LayoutElement m_imageSizer = null;

		public void Populate(CharacterData character, Sprite image) {
			m_layout.Populate(character);
			UIBase.AssignSpritePreserveAspect(m_bodyImage, m_imageSizer, image, Axis.X);
		}
	}


}
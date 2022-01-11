using BeauRoutine;
using BeauUtil;
using PotatoLocalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	[RequireComponent(typeof(Inspectable))]
	public class TextMessageImage : MonoBehaviour {

		[SerializeField]
		private TextMessageLayout m_layout = null;
		[SerializeField]
		private Image m_bodyImage = null;
		[SerializeField]
		private LayoutElement m_imageSizer = null;
		[SerializeField]
		private Inspectable m_inspectable;

		public void Populate(CharacterData character, Sprite image, StringHash32 imageId) {
			m_layout.Populate(character);
			m_inspectable.SetSprite(image);
			m_inspectable.Button.onClick.AddListener(delegate { HandleCloseInspect(m_inspectable.Sprite, imageId); });
			UIBase.AssignSpritePreserveAspect(m_bodyImage, m_imageSizer, image, Axis.X);
		}

		private void HandleCloseInspect(Sprite sprite, StringHash32 imageId) {
			UIMgr.Open<UICloseInspect>().OpenCloseInspect(sprite, imageId);
		}
	}


}
using BeauUtil;
using PotatoLocalization;
using UnityEngine;

namespace Shipwreck {

	[CreateAssetMenu(fileName = "NewCharacter", menuName = "Shipwrecks/Character")]
	public class CharacterData : ScriptableObject {

		public StringHash32 Hash { 
			get { return m_tag.ToLower(); } 
		}
		public LocalizationKey DisplayName {
			get { return m_displayName; }
		}
		public Sprite TextingIcon {
			get { return m_textingIcon; }
		}
		public Color TextingColor {
			get { return m_textingColor; }
		}
		public Color TextingBackgroundColor {
			get { return m_textingBackgroundColor; }
		}
		public Sprite Portrait {
			get { return m_portrait; }
		}
		public Sprite Background {
			get { return m_background; }
		}
		public Color DialogBackgroundColor {
			get { return m_dialogBackgroundColor; }
		}
		public Color DialogTextColor { 
			get { return m_dialogTextColor; }
		}

		[SerializeField, Tooltip("Tag used to refer to this Characer from Leaf scripts")]
		private string m_tag = string.Empty;
		[SerializeField, Tooltip("Name shown in the character box when this Character is speaking")]
		private LocalizationKey m_displayName = LocalizationKey.Empty;
		
		[Header("Texting")]
		[SerializeField, Tooltip("Icon used for text messages sent by this Character")]
		private Sprite m_textingIcon = null;
		[SerializeField, Tooltip("Color used to tint phone icon and texting outlines")]
		private Color m_textingColor = Color.red;
		[SerializeField, Tooltip("Color used to tint phone icon and texting outlines")]
		private Color m_textingBackgroundColor = ColorBank.DarkRed;

		[Header("Dialog")]
		[SerializeField, Tooltip("Sprite used when the player is directly talking to this Character")]
		private Sprite m_portrait = null;
		[SerializeField, Tooltip("Sprite used behind the portrait when the player is talking directly to this Character")]
		private Sprite m_background = null;
		[SerializeField, Tooltip("Color used to tint the dialog text")]
		private Color m_dialogTextColor = Color.white;
		[SerializeField, Tooltip("Color used to tint the dialog background")]
		private Color m_dialogBackgroundColor = Color.grey;

	}

}
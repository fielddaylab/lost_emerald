using PotatoLocalization;
using UnityEngine;

namespace Shipwreck {

	[CreateAssetMenu(fileName = "NewCharacter", menuName = "Shipwrecks/Character")]
	public class CharacterData : ScriptableObject {

		public string Tag { 
			get { return m_tag; } 
		}
		public LocalizationKey DisplayName {
			get { return m_displayName; }
		}
		public Sprite TextingIcon {
			get { return m_textingIcon; }
		}
		public Sprite Portrait {
			get { return m_portrait; }
		}

		[SerializeField, Tooltip("Tag used to refer to this Characer from Leaf scripts")]
		private string m_tag = string.Empty;
		[SerializeField, Tooltip("Name shown in the character box when this Character is speaking")]
		private LocalizationKey m_displayName = LocalizationKey.Empty;
		[SerializeField, Tooltip("Icon used for text messages sent by this Character")]
		private Sprite m_textingIcon = null;
		[SerializeField, Tooltip("Sprite used when the player is directly talking to this Character")]
		private Sprite m_portrait = null;


	}

}
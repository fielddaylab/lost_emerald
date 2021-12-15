using UnityEngine;

namespace Shipwreck {

	public class TextMessageObject : MonoBehaviour {

		[SerializeField]
		private TextMessageLayout m_layout;
		[SerializeField]
		private RectTransform m_content;

		public void Populate(CharacterData character, GameObject prefab) {
			m_layout.Populate(character);
			GameObject obj = Instantiate(prefab, m_content);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
		}
		public void Populate(CharacterData character, EvidenceGroup prefab) {
			m_layout.Populate(character);
			EvidenceGroup obj = Instantiate(prefab, m_content);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			obj.RemoveNodes();
		}
	}

}
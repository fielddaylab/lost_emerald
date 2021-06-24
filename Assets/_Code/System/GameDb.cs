using BeauUtil;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {

	public class GameDb : Singleton<GameDb> {

		[SerializeField]
		private CharacterData[] m_characters;


		private Dictionary<StringHash32, CharacterData> m_characterMap;
		
		public static CharacterData GetCharacterData(StringHash32 tag) {
			// initialize the map if it does not exist
			if (I.m_characterMap == null) {
				I.m_characterMap = new Dictionary<StringHash32, CharacterData>();
				foreach (CharacterData data in I.m_characters) {
					I.m_characterMap.Add(data.Hash, data);
					Debug.Log(data.Hash);
				}
			}
			// find the tag within the map
			if (I.m_characterMap.ContainsKey(tag)) {
				return I.m_characterMap[tag];
			} else {
				throw new KeyNotFoundException(string.Format("No " +
					"Character with tag `{0}' is in the database", tag
				));
			}
		}


	}


}
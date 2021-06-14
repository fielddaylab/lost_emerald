using BeauUtil;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {

	public class GameDb : Singleton<GameDb> {

		[SerializeField]
		private CharacterData[] m_characters;


		private Dictionary<string, CharacterData> m_characterMap;
		
		public static CharacterData GetCharacterData(string tag) {
			// initialize the map if it does not exist
			if (I.m_characterMap == null) {
				I.m_characterMap = new Dictionary<string, CharacterData>();
				foreach (CharacterData data in I.m_characters) {
					I.m_characterMap.Add(data.Tag.ToLower(), data);
				}
			}
			// find the tag within the map
			string lowered = tag.ToLower();
			if (I.m_characterMap.ContainsKey(lowered)) {
				return I.m_characterMap[lowered];
			} else {
				throw new KeyNotFoundException(string.Format("No Character " +
					"with tag `{0}' is in the database", tag
				));
			}
		}


	}


}
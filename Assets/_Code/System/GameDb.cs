using BeauUtil;
using BeauUtil.Debugger;
using PotatoLocalization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {

	public class GameDb : Singleton<GameDb> {

		[Serializable]
		private struct NodeKeyPair {
			public SerializedHash32 NodeID;
			public LocalizationKey LocalizationKey;
		}

		[SerializeField]
		private CharacterData[] m_characters;
		[SerializeField]
		private EvidenceData[] m_evidenceData;
		[SerializeField]
		private Sprite[] m_images;
		[SerializeField]
		private NodeKeyPair[] m_nodeKeyPairs;

		[SerializeField,Header("Colors")]
		private Color m_stickyNoteDefault = Color.black;
		[SerializeField]
		private Color m_stickyNoteIncorrect = Color.black;
		[SerializeField]
		private Color m_stickyNoteComplete = Color.black;
		[SerializeField]
		private Color m_lineDefault = Color.black;
		[SerializeField]
		private Color m_lineIncorrect = Color.black;
		[SerializeField]
		private Color m_lineComplete = Color.black;
		[SerializeField]
		private Color m_pinDefault = Color.black;
		[SerializeField]
		private Color m_pinIncorrect = Color.black;
		[SerializeField]
		private Color m_pinComplete = Color.black;

		[NonSerialized]
		private Dictionary<StringHash32, CharacterData> m_characterMap;
		[NonSerialized]
		private Dictionary<StringHash32, EvidenceData> m_evidenceMap;		
		[NonSerialized]
		private Dictionary<StringHash32, Sprite> m_imageMap;
		[NonSerialized]
		private Dictionary<StringHash32, LocalizationKey> m_nodeKeyPairMap;


		public static CharacterData GetCharacterData(StringHash32 hash) {
			// initialize the map if it does not exist
			if (I.m_characterMap == null) {
				I.m_characterMap = new Dictionary<StringHash32, CharacterData>();
				foreach (CharacterData data in I.m_characters) {
					I.m_characterMap.Add(data.Hash, data);
				}
			}
			// find the tag within the map
			if (I.m_characterMap.ContainsKey(hash)) {
				return I.m_characterMap[hash];
			} else {
				throw new KeyNotFoundException(string.Format("No Character " +
					"with tag `{0}' is in the database", hash
				));
			}
		}
		public static Sprite GetImageData(StringHash32 hash) {
			if (I.m_imageMap == null) {
				I.m_imageMap = new Dictionary<StringHash32, Sprite>();
				foreach(Sprite sprite in I.m_images) {
					I.m_imageMap.Add(sprite.name, sprite);
				}
			}
			// find the tag within the map
			if (I.m_imageMap.ContainsKey(hash)) {
				return I.m_imageMap[hash];
			} else {
				throw new KeyNotFoundException(Log.Format("No " +
					"Image with id `{0}' is in the database", hash
				));
			}
		}

		public static EvidenceGroup GetEvidenceGroup(StringHash32 groupID) {
			// initialize the map if it does not exist
			if (I.m_evidenceMap == null) {
				I.m_evidenceMap = new Dictionary<StringHash32, EvidenceData>();
				foreach (EvidenceData data in I.m_evidenceData) {
					I.m_evidenceMap.Add(data.GroupID, data);
				}
			}
			if (I.m_evidenceMap.ContainsKey(groupID)) {
				return I.m_evidenceMap[groupID].NodeGroup;
			} else {
				throw new KeyNotFoundException(string.Format("No Evidence " +
					"with groupID `{0}' is in the database", groupID
				));
			}
		}
		public static EvidenceData GetEvidenceData(StringHash32 groupID) {
			// initialize the map if it does not exist
			if (I.m_evidenceMap == null) {
				I.m_evidenceMap = new Dictionary<StringHash32, EvidenceData>();
				foreach (EvidenceData data in I.m_evidenceData) {
					I.m_evidenceMap.Add(data.GroupID, data);
				}
			}
			if (I.m_evidenceMap.ContainsKey(groupID)) {
				return I.m_evidenceMap[groupID];
			} else {
				throw new KeyNotFoundException(string.Format("No Evidence " +
					"with groupID `{0}' is in the database", groupID
				));
			}
		}

		public static Color GetStickyColor(ChainStatus status) {
			switch (status) {
				case ChainStatus.Normal: return I.m_stickyNoteDefault;
				case ChainStatus.Incorrect: return I.m_stickyNoteIncorrect;
				case ChainStatus.Complete: return I.m_stickyNoteComplete;
				default: throw new NotImplementedException();
			}
		}
		public static Color GetLineColor(ChainStatus status) {
			switch (status) {
				case ChainStatus.Normal: return I.m_lineDefault;
				case ChainStatus.Incorrect: return I.m_lineIncorrect;
				case ChainStatus.Complete: return I.m_lineComplete;
				default: throw new NotImplementedException();
			}
		}

		public static Color GetPinColor(ChainStatus status) {
			switch (status) {
				case ChainStatus.Normal: return I.m_pinDefault;
				case ChainStatus.Incorrect: return I.m_pinIncorrect;
				case ChainStatus.Complete: return I.m_pinComplete;
				default: throw new NotImplementedException();
			}
		}

		public static LocalizationKey GetNodeLocalizationKey(StringHash32 nodeId) {
			if (I.m_nodeKeyPairMap == null) {
				I.m_nodeKeyPairMap = new Dictionary<StringHash32, LocalizationKey>();
				foreach (NodeKeyPair pair in I.m_nodeKeyPairs) {
					I.m_nodeKeyPairMap.Add(pair.NodeID, pair.LocalizationKey);
				}
			}
			return I.m_nodeKeyPairMap[nodeId];
		}

	}


}

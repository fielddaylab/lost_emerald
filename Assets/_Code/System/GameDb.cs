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
		private Sprite[] m_backgrounds;
		[SerializeField]
		private NodeKeyPair[] m_nodeKeyPairs;
		[SerializeField]
		private LevelData[] m_levelData;
		[SerializeField]
		private ShipOutData[] m_shipOutData;
		[SerializeField]
		private AudioData[] m_audioData;
		[SerializeField]
		private DisplayObjData[] m_displayObjects;

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
		private Dictionary<StringHash32, Sprite> m_backgroundMap;
		[NonSerialized]
		private Dictionary<StringHash32, LocalizationKey> m_nodeKeyPairMap;
		[NonSerialized]
		private Dictionary<int, LevelData> m_levelMap;
		[NonSerialized]
		private Dictionary<int, ShipOutData> m_shipOutMap;
		[NonSerialized]
		private Dictionary<string, AudioData> m_audioMap;
		[NonSerialized]
		private Dictionary<StringHash32, DisplayObjData> m_displayObjMap;


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

		public static Sprite GetBackground(StringHash32 hash) {
			if (I.m_backgroundMap == null) {
				I.m_backgroundMap = new Dictionary<StringHash32, Sprite>();
				foreach (Sprite sprite in I.m_backgrounds) {
					I.m_backgroundMap.Add(sprite.name, sprite);
				}
			}
			// find the tag within the map
			if (I.m_backgroundMap.ContainsKey(hash)) {
				return I.m_backgroundMap[hash];
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

		public static GameObject GetDisplayObject(StringHash32 hash) {
			if (I.m_displayObjMap == null) {
				I.m_displayObjMap = new Dictionary<StringHash32, DisplayObjData>();
				foreach (DisplayObjData data in I.m_displayObjects) {
					I.m_displayObjMap.Add(data.Hash, data);
				}
			}
			if (I.m_displayObjMap.ContainsKey(hash)) {
				return I.m_displayObjMap[hash].Prefab;
			} else {
				throw new KeyNotFoundException(string.Format("No Display Object" +
					"with id `{0}' is in the database", hash.ToDebugString()
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

		public static LevelData GetLevelData(int index) {
			if (I.m_levelMap == null) {
				I.m_levelMap = new Dictionary<int, LevelData>();
				foreach (LevelData data in I.m_levelData) {
					I.m_levelMap.Add(data.LevelIndex, data);
				}
			}
			return I.m_levelMap[index];
		}

		public static ShipOutData GetShipOutData(int index)
		{
			if (I.m_shipOutMap == null)
			{
				I.m_shipOutMap = new Dictionary<int, ShipOutData>();
				foreach (ShipOutData data in I.m_shipOutData)
				{
					I.m_shipOutMap.Add(data.ShipOutIndex, data);
				}
			}
			return I.m_shipOutMap[index];
		}

		public static AudioData GetAudioData(string id)
		{
			// initialize the map if it does not exist
			if (I.m_audioMap == null)
			{
				I.m_audioMap = new Dictionary<string, AudioData>();
				foreach (AudioData data in I.m_audioData)
				{
					I.m_audioMap.Add(data.ID, data);
				}
			}
			if (I.m_audioMap.ContainsKey(id))
			{
				return I.m_audioMap[id];
			}
			else
			{
				throw new KeyNotFoundException(string.Format("No Audio " +
					"with id `{0}' is in the database", id
				));
			}
		}

	}


}

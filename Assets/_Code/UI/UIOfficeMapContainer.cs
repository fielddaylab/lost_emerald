using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {
	public class UIOfficeMapContainer : MonoBehaviour {

		[SerializeField]
		private LevelMarker[] m_levelMarkers = null;

		private void OnEnable() {
			GameMgr.Events.Register(GameEvents.LevelUnlocked, RefreshMarkers); // new marker
			GameMgr.Events.Register(GameEvents.LocationDiscovered, RefreshMarkers); // unknown -> known
			GameMgr.Events.Register(GameEvents.BoardComplete, RefreshMarkers); // change marker color
			RefreshMarkers();
		}

		private void OnDisable() {
			GameMgr.Events.Deregister(GameEvents.LevelUnlocked, RefreshMarkers);
			GameMgr.Events.Deregister(GameEvents.LocationDiscovered, RefreshMarkers);
			GameMgr.Events.Deregister(GameEvents.BoardComplete, RefreshMarkers);
		}

		private void RefreshMarkers() {
			for (int index = 0; index < m_levelMarkers.Length; index++) {
				bool isUnlocked = GameMgr.State.IsLevelUnlocked(index);
				LevelMarker marker = m_levelMarkers[index];
				marker.gameObject.SetActive(isUnlocked);
				Vector2 markerPos = GameMgr.State.GetLevel(index).MarkerPos;
				marker.transform.localPosition = markerPos;
				Sprite markerSprite;
				if (GameMgr.State.GetLevel(index).IsLocationKnown) {
					markerSprite = GameDb.GetMarkerSprite("marker-default");
				}
				else {
					markerSprite = GameDb.GetMarkerSprite(GameMgr.State.GetLevel(index).MarkerUnknownSpriteID);
				}
				marker.SetSprite(markerSprite);
				marker.SetColor(GameDb.GetMarkerColor(index));
			}
		}
	}
}

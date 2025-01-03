﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {
	public class UIOfficeMapContainer : MonoBehaviour {

		[SerializeField]
		private LevelMarker[] m_levelMarkers = null;

		private static Vector2 MAP_DISCREPANCY = new Vector2((740-940) / 2.0f, (500-600) / -2.0f);
		private static Vector2 MAP_RATIO = new Vector2(740.0f / 940, 500.0f / 600);

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
				marker.transform.localPosition = markerPos * MAP_RATIO;
				Sprite markerSprite;
				if (GameMgr.State.GetLevel(index).IsLocationKnown) {
					markerSprite = GameDb.GetMarkerSprite("marker-default");
				}
				else {
					ILevelState state = GameMgr.State.GetLevel(index);
					markerSprite = GameDb.GetMarkerSprite(state.MarkerUnknownSpriteID);
					marker.transform.localPosition += new Vector3(state.MarkerUnknownSpriteOffset.x * MAP_RATIO.x, state.MarkerUnknownSpriteOffset.y * MAP_RATIO.y, 0f);
				}
				marker.SetSprite(markerSprite);
				marker.SetColor(GameDb.GetMarkerColor(index));
				marker.transform.localScale = MAP_RATIO;
			}
		}
	}
}

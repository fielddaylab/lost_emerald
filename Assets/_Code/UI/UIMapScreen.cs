using BeauRoutine;
using PotatoLocalization;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIMapScreen : UIBase {

		[SerializeField]
		private RectTransform m_container = null;
		[SerializeField]
		private Button m_closeButton = null;
		[SerializeField]
		private LevelMarker[] m_levelMarkers = null;
		[SerializeField]
		private LocalizedTextUGUI[] m_levelLabels = null;

		protected override IEnumerator ShowRoutine() {
			m_container.anchoredPosition = new Vector2(0f, 620f);
			yield return Routine.Combine(
				CanvasGroup.FadeTo(1f, 0.25f),
				m_container.AnchorPosTo(0f, 0.2f, Axis.Y)
			);
		}
		protected override IEnumerator HideRoutine() {
			m_container.anchoredPosition = Vector2.zero;
			yield return Routine.Combine(
				m_container.AnchorPosTo(620f, 0.2f, Axis.Y),
				CanvasGroup.FadeTo(0f, 0.25f)
			);
		}

		protected override void OnShowStart() {
			base.OnShowStart();

			CanvasGroup.alpha = 0;

			m_levelMarkers[0].Button.onClick.AddListener(() => { HandleLevelButton(0); });
			m_levelMarkers[1].Button.onClick.AddListener(() => { HandleLevelButton(1); });
			m_levelMarkers[2].Button.onClick.AddListener(() => { HandleLevelButton(2); });
			m_levelMarkers[3].Button.onClick.AddListener(() => { HandleLevelButton(3); });


			for (int index = 0; index < m_levelMarkers.Length; index++) {
				bool isUnlocked = GameMgr.State.IsLevelUnlocked(index);
				LevelMarker marker = m_levelMarkers[index];
				marker.Button.interactable = isUnlocked;
				marker.gameObject.SetActive(isUnlocked);
				Vector2 markerPos = GameMgr.State.GetLevel(index).MarkerPos;
				Vector2 bannerPos = GameMgr.State.GetLevel(index).BannerPos;
				marker.transform.localPosition = markerPos;
				marker.Banner.transform.localPosition = bannerPos;
				Sprite markerSprite;
				if (GameMgr.State.GetLevel(index).IsLocationKnown) {
					markerSprite = GameDb.GetMarkerSprite("marker-default");
				}
				else {
					ILevelState state = GameMgr.State.GetLevel(index);
					markerSprite = GameDb.GetMarkerSprite(state.MarkerUnknownSpriteID);
					marker.transform.localPosition += new Vector3(state.MarkerUnknownSpriteOffset.x, state.MarkerUnknownSpriteOffset.y, 0f);
				}
				marker.SetSprite(markerSprite);
				marker.SetColor(GameDb.GetMarkerColor(index));
				m_levelLabels[index].Key = GameMgr.State.GetLevelName(index);
			}

			GameMgr.Events.Register<int>(GameEvents.LevelUnlocked, HandleLevelUnlocked);
		}

		protected override void OnShowCompleted() {
			m_closeButton.onClick.AddListener(HandleClose);
		}
		protected override void OnHideStart() {
			m_closeButton.onClick.RemoveListener(HandleClose);

			foreach (LevelMarker marker in m_levelMarkers) {
				marker.Button.onClick.RemoveAllListeners();
				marker.Button.interactable = false;
				marker.gameObject.SetActive(false);
			}

			GameMgr.Events.Deregister<int>(GameEvents.LevelUnlocked, HandleLevelUnlocked);
		}

		private void HandleLevelButton(int level) {
			AudioSrcMgr.instance.PlayOneShot("click_level");
			GameMgr.SetLevelIndex(level);
			GameMgr.State.SetCurrShipOutIndex(level);
			UIMgr.Close<UIMapScreen>();
			UIMgr.CloseThenOpen<UIOfficeScreen, UIEvidenceScreen>();
		}

		private void HandleClose() {
			AudioSrcMgr.instance.PlayOneShot("click_map_close");
			UIMgr.Close<UIMapScreen>();
		}

		private void HandleLevelUnlocked(int level) {
			for (int index = 0; index < m_levelMarkers.Length; index++) {
				m_levelMarkers[index].Button.interactable = GameMgr.State.IsLevelUnlocked(index);
				m_levelLabels[index].Key = GameMgr.State.GetLevelName(index);
			}
		}
	}

}
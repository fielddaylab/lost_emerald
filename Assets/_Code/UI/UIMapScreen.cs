using BeauRoutine;
using PotatoLocalization;
using System.Collections;
using UnityEngine;
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
		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}

		protected override void OnShowStart() {
			base.OnShowStart();

			CanvasGroup.alpha = 0;

			m_levelMarkers[0].BannerButton.onClick.AddListener(() => { HandleLevelButton(0); });
			m_levelMarkers[0].MarkerButton.onClick.AddListener(() => { HandleLevelButton(0); });
			m_levelMarkers[1].BannerButton.onClick.AddListener(() => { HandleLevelButton(1); });
			m_levelMarkers[1].MarkerButton.onClick.AddListener(() => { HandleLevelButton(1); });
			m_levelMarkers[2].BannerButton.onClick.AddListener(() => { HandleLevelButton(2); });
			m_levelMarkers[2].MarkerButton.onClick.AddListener(() => { HandleLevelButton(2); });
			m_levelMarkers[3].BannerButton.onClick.AddListener(() => { HandleLevelButton(3); });
			m_levelMarkers[3].MarkerButton.onClick.AddListener(() => { HandleLevelButton(3); });

			for (int index = 0; index < m_levelMarkers.Length; index++) {
				bool isUnlocked = GameMgr.State.IsLevelUnlocked(index);
				LevelMarker marker = m_levelMarkers[index];
				marker.BannerButton.interactable = isUnlocked;
				marker.MarkerButton.interactable = isUnlocked;
				marker.gameObject.SetActive(isUnlocked);
				Vector2 markerPos = GameMgr.State.GetLevel(index).MarkerPos;
				marker.transform.localPosition = markerPos;
				Sprite markerSprite;
				if (GameMgr.State.GetLevel(index).IsLocationKnown) {
					markerSprite = GameDb.GetMarkerSprite("marker-default");
					marker.SetBannerOrder("back");
				}
				else {
					ILevelState state = GameMgr.State.GetLevel(index);
					markerSprite = GameDb.GetMarkerSprite(state.MarkerUnknownSpriteID);
					marker.transform.localPosition += new Vector3(state.MarkerUnknownSpriteOffset.x, state.MarkerUnknownSpriteOffset.y, 0f);
					marker.SetBannerOrder("front");
				}
				marker.SetSprite(markerSprite);
				marker.SetColor(GameDb.GetMarkerColor(index));
				if (GameMgr.State.GetLevel(index).IsBoardComplete()) {
					marker.SetBannerSprite(GameDb.GetMarkerSprite("bar-closed-minimal"));
					marker.SetTagVisible(true);
				}
				else {
					marker.SetBannerSprite(GameDb.GetMarkerSprite("bar-open-minimal"));
					marker.SetTagVisible(false);
				}
				m_levelLabels[index].Key = GameMgr.State.GetLevelName(index);

				Vector2 bannerPos = GameMgr.State.GetLevel(index).BannerPos;
				marker.SetBannerPos(bannerPos);
			}

			GameMgr.Events.Register<int>(GameEvents.LevelUnlocked, HandleLevelUnlocked);

            GameMgr.Events.Dispatch(GameEvents.MapOpened);
		}

		protected override void OnShowCompleted() {
			m_closeButton.onClick.AddListener(HandleClose);
		}
		protected override void OnHideStart() {
			m_closeButton.onClick.RemoveListener(HandleClose);

			foreach (LevelMarker marker in m_levelMarkers) {
				marker.BannerButton.onClick.RemoveAllListeners();
				marker.MarkerButton.onClick.RemoveAllListeners();
				marker.BannerButton.interactable = false;
				marker.MarkerButton.interactable = false;
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
				m_levelMarkers[index].BannerButton.interactable = GameMgr.State.IsLevelUnlocked(index);
				m_levelMarkers[index].MarkerButton.interactable = GameMgr.State.IsLevelUnlocked(index);
				m_levelLabels[index].Key = GameMgr.State.GetLevelName(index);
			}
		}
	}

}
using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {
	public class LevelMarker : MonoBehaviour {

		private static Vector2 BANNER_DIMS = new Vector2(130, 30);

		[SerializeField]
		private Image m_markerImage;
		[SerializeField]
		private Button m_bannerButton;
		[SerializeField]
		private Button m_markerButton;
		[SerializeField]
		private GameObject m_banner;
		[SerializeField]
		private Image m_bannerImage;
		[SerializeField]
		private Image m_caseClosedTag;

		public Button BannerButton {
			get { return m_bannerButton; }
		}
		public Button MarkerButton {
			get { return m_markerButton; }
		}
		public GameObject Banner {
			get { return m_banner; }
		}

		private Routine m_colorRoutine;

		public void SetColor(Color color) {
			m_colorRoutine.Replace(this, ColorTo(color));
		}

		private IEnumerator ColorTo(Color color) {
			yield return m_markerImage.ColorTo(color, 0.1f).Ease(Curve.QuadOut);
		}

		public void SetSprite(Sprite sprite) {
			m_markerImage.sprite = sprite;
			m_markerImage.SetNativeSize();
			// hack? ideally images would be uploaded as the correct size
			m_markerImage.transform.SetScale(new Vector3(.5f, .5f, 1));
		}

		public void SetBannerSprite(Sprite sprite) {
			m_bannerImage.sprite = sprite;
			m_bannerImage.rectTransform.sizeDelta = BANNER_DIMS;
			// hack? ideally images would be uploaded as the correct size
			//m_bannerImage.transform.SetScale(new Vector3(.5f, .5f, 1));
		}

		public void SetTagVisible(bool reveal) {
			m_caseClosedTag.gameObject.SetActive(reveal);
		}

		public void SetBannerOrder(string order) {
			switch (order) {
				case "back":
					m_bannerImage.GetComponent<RectTransform>().SetSiblingIndex(0);
					break;
				case "front":
					m_markerImage.GetComponent<RectTransform>().SetSiblingIndex(0);
					break;
				default:
					break;
			}
		}

		public void SetBannerPos(Vector2 rawPos) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(m_bannerImage.rectTransform);
			Vector2 textWidthOffset = new Vector2(
				(m_bannerImage.rectTransform.rect.width / 2.0f) + 3,
				0f
				);
			m_banner.transform.localPosition = rawPos + textWidthOffset;
		}

	}
}


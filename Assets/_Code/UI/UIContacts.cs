using BeauRoutine;
using BeauUtil;
using System.Collections;
using UnityEngine;

namespace Shipwreck {

	public class UIContacts : UIBase {

		[SerializeField]
		private TweenSettings m_tweenSettings = new TweenSettings(0.3f, Curve.QuadInOut);
		[SerializeField]
		private ContactItem m_contactPrefab = null;
		[SerializeField]
		private RectTransform m_content = null;


		private RectTransform m_rectTransform;
		
		private void OnEnable() {
			m_rectTransform = (RectTransform)transform;
		}

		protected override void OnShowStart() {
			m_rectTransform.anchoredPosition = new Vector2(m_rectTransform.anchoredPosition.x, -660f);
			foreach (StringHash32 hash in GameMgr.State.GetUnlockedContacts()) {
				CharacterData data = GameDb.GetCharacterData(hash);
				ContactItem item = Instantiate(m_contactPrefab, m_content);
				item.SetCharacter(data);
			}
		}
		protected override void OnHideCompleted() {
			for (int ix = m_content.childCount-1; ix >= 0; ix--) {
				Destroy(m_content.GetChild(ix).gameObject);
			}
		}

		protected override IEnumerator HideRoutine() {
			yield return m_rectTransform.AnchorPosTo(-660f, m_tweenSettings, Axis.Y);
		}
		protected override IEnumerator ShowRoutine() {
			yield return m_rectTransform.AnchorPosTo(45f, m_tweenSettings, Axis.Y);
		}
	}


}
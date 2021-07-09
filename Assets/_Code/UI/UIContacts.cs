using BeauRoutine;
using BeauUtil;
using Leaf.Runtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIContacts : UIBase {

		[SerializeField]
		private TweenSettings m_tweenSettings = new TweenSettings(0.3f, Curve.QuadInOut);
		[SerializeField]
		private ContactItem m_contactPrefab = null;
		[SerializeField]
		private RectTransform m_content = null;
		[SerializeField]
		private Button m_backButton = null;


		private RectTransform m_rectTransform;

		private void Awake() {
			m_rectTransform = (RectTransform)transform;
			m_backButton.onClick.AddListener(HandleBackClicked);
		}

		protected override void OnShowStart() {
			base.OnShowStart();

			m_rectTransform.anchoredPosition = new Vector2(m_rectTransform.anchoredPosition.x, -660f);
			foreach (StringHash32 hash in GameMgr.State.GetUnlockedContacts()) {
				CharacterData data = GameDb.GetCharacterData(hash);
				ContactItem item = Instantiate(m_contactPrefab, m_content);
				item.OnClicked += HandleContactClicked;
				item.SetCharacter(data);

				StringHash32 notificationId = GameMgr.State.GetContactNotificationId(hash);
				item.SetNotifications(!notificationId.IsEmpty ? 1u : 0u);
			}
		}
		protected override void OnHideCompleted() {
			for (int ix = m_content.childCount-1; ix >= 0; ix--) {
				Destroy(m_content.GetChild(ix).gameObject);
			}
			
			base.OnHideCompleted();
		}

		protected override IEnumerator HideRoutine() {
			yield return m_rectTransform.AnchorPosTo(-660f, m_tweenSettings, Axis.Y);
		}
		protected override IEnumerator ShowRoutine() {
			yield return m_rectTransform.AnchorPosTo(45f, m_tweenSettings, Axis.Y);
		}

		private void HandleContactClicked(StringHash32 character) {
			LeafThreadHandle convo;
			if (!GameMgr.TryRunNotification(character, out convo)) {
				convo = GameMgr.RunTrigger(GameTriggers.OnContactText, null, null, character);
			}
			if (convo.IsRunning()) {
				UIMgr.Close(this);
			}
		}

		private void HandleBackClicked() { 
			UIMgr.CloseThenOpen<UIContacts,UIPhoneNotif>();
		}

	}


}
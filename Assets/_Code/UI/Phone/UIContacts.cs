﻿using BeauRoutine;
using BeauUtil;
using Leaf.Runtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UIContacts : UIBase {

		[SerializeField]
		private ContactItem m_contactPrefab = null;
		[SerializeField]
		private RectTransform m_content = null;
		[SerializeField]
		private Button m_backButton = null;

		private void Awake() {
			m_backButton.onClick.AddListener(HandleBackClicked);
		}

		#region UIBase

		protected override void OnShowStart() {
			base.OnShowStart();
			UIMgr.Open<UIPhone>();
			UIMgr.Open<UIModalOverlay>();
			UIMgr.Close<UITextMessage>();

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
			yield break;
		}
		protected override IEnumerator ShowRoutine() {
			yield break;
		}
		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}

		#endregion // UIBase

		#region Handlers

		private void HandleContactClicked(StringHash32 character) {
			AudioSrcMgr.instance.PlayOneShot("click_contact");
			LeafThreadHandle convo;
			if (!GameMgr.TryRunNotification(character, out convo)) {
				convo = GameMgr.RunTrigger(GameTriggers.OnContactText, null, null, character);
			}
		}

		private void HandleBackClicked() {
			AudioSrcMgr.instance.PlayOneShot("click_contacts_back");
			UIMgr.Close<UIPhone>();
		}

		#endregion // Handlers
	}


}
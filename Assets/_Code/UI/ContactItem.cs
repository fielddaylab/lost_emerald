using BeauUtil;
using PotatoLocalization;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class ContactItem : MonoBehaviour {

		public event Action<StringHash32> OnClicked;

		[SerializeField]
		private Image m_icon = null;
		[SerializeField]
		private LocalizedTextUGUI m_name = null;
		[SerializeField]
		private GameObject m_notificationGroup = null;
		[SerializeField]
		private TextMeshProUGUI m_notificationNumber = null;
		[SerializeField]
		private Button m_button = null;

		private StringHash32 m_characterHash;

		public void SetCharacter(CharacterData data) {
			m_icon.sprite = data.TextingIcon;
			m_name.Key = data.DisplayName;
			m_characterHash = data.Hash;
		}
		public void SetNotifications(uint count) {
			if (count == 0) {
				m_notificationGroup.SetActive(false);
			} else {
				m_notificationGroup.SetActive(true);
				m_notificationNumber.text = count.ToString();
			}
		}

		private void OnEnable() {
			m_button.onClick.AddListener(HandleClicked);
		}
		private void OnDisable() {
			m_button.onClick.RemoveListener(HandleClicked);
		}
		private void HandleClicked() {
			OnClicked?.Invoke(m_characterHash);
		}


	}

}
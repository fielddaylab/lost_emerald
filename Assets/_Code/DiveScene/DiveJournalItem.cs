using PotatoLocalization;
using UnityEngine;

namespace Shipwreck {

	public class DiveJournalItem : MonoBehaviour {

		[SerializeField]
		private GameObject m_iconEmpty = null;
		[SerializeField]
		private GameObject m_iconChecked = null;
		[SerializeField]
		private LocalizedTextUGUI m_text = null;

		public void SetChecked(bool isChecked) {
			m_iconEmpty.SetActive(!isChecked);
			m_iconChecked.SetActive(isChecked);
		}
		public void SetText(LocalizationKey key) {
			m_text.Key = key;
		}

	}

}
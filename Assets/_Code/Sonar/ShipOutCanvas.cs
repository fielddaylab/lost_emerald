using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck
{
	/// <summary>
	/// The canvas of the ShipOut scene
	/// </summary>
	public class ShipOutCanvas : MonoBehaviour
	{
		[SerializeField]
		private Button m_returnToOfficeButton; // button that returns the player to office
		[SerializeField]
		private GameObject m_diveButtonPrefab; // the prefab from which the dive button is generated
		private GameObject m_diveButton; // the dive button
		[SerializeField]
		private Slider m_diveSlider; // the dive progress bar

		// Awake is called when the script instance is being loaded (before Start)
		private void Awake()
		{
			m_returnToOfficeButton.onClick.AddListener(HandleReturnToOfficeButton);
		}

		/// <summary>
		/// Handles when the office button is clicked
		/// </summary>
		private void HandleReturnToOfficeButton()
		{
			SceneManager.LoadScene("Main");
			UIMgr.Open<UIOfficeScreen>();
		}

		/// <summary>
		/// Handles when the dive button is clicked
		/// </summary>
		private void HandleDiveButton()
		{
			// ensure the dive is locked
			if (GameMgr.State.IsDiveUnlocked(ShipOutMgr.instance.GetData().ShipOutIndex))
			{
				// TODO: pull this from ShipOutData
				SceneManager.LoadScene("Dive_Ship01");
				UIMgr.Open<UIDiveScreen>();
			}
		}

		/// <summary>
		/// Replaces the filled slider with the clickable dive button
		/// </summary>
		public void SwapButtonForSlider()
		{
			// destory old bar
			Destroy(m_diveSlider.gameObject);

			// create new button
			m_diveButton = Instantiate(m_diveButtonPrefab, this.transform);

			// add button functionality
			m_diveButton.GetComponent<Button>().onClick.AddListener(HandleDiveButton);
		}
	}

}
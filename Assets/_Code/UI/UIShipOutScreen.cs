using PotatoLocalization;
using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

namespace Shipwreck
{
	/// <summary>
	/// The canvas of the ShipOut scene
	/// </summary>
	public class UIShipOutScreen : MonoBehaviour
	{
		public enum MessageState
		{
			hidden,
			showing
		}

		[SerializeField]
		private Button m_returnToOfficeButton; // button that returns the player to office
		[SerializeField]
		private GameObject m_diveButtonPrefab; // the prefab from which the dive button is generated
		private GameObject m_diveButton; // the dive button
		[SerializeField]
		private Slider m_diveSlider; // the dive progress bar

		[SerializeField, Header("Message Box")]
		private RectTransform m_messageGroup = null;
		[SerializeField]
		private TextMeshProUGUI m_messageText = null;
		[SerializeField]
		private Button m_messageButton = null;
		[SerializeField]
		private TextMeshProUGUI m_messageButtonText = null;
		[SerializeField]
		private float m_messageHiddenY = -88f;
		[SerializeField]
		private float m_messageShownY = 150f;

		private Routine m_messageRoutine;

		private MessageState m_currentMessageState;

		// Awake is called when the script instance is being loaded (before Start)
		private void Awake()
		{
			m_returnToOfficeButton.onClick.AddListener(HandleReturnToOfficeButton);
			m_currentMessageState = MessageState.hidden;
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

		public MessageState GetCurrMessageState()
		{
			return m_currentMessageState;
		}

		#region MessageBox

		public void ShowBuoyMessage()
		{
			m_messageText.text = "There it is! I’ll drop a buoy to mark the location.";
			m_messageButtonText.text = "Continue";
			m_messageGroup.anchoredPosition = new Vector2(m_messageGroup.anchoredPosition.x, m_messageHiddenY);
			m_messageRoutine.Replace(this, m_messageGroup.AnchorPosTo(m_messageShownY, 0.25f, Axis.Y).Ease(Curve.QuadOut));
			m_currentMessageState = MessageState.showing;
			m_messageButton.onClick.AddListener(HideMessageBox);
			m_messageButton.onClick.AddListener(ShipOutMgr.instance.UnlockDive);
		}
		private void HideMessageBox()
		{
			m_messageRoutine.Replace(this, m_messageGroup.AnchorPosTo(m_messageHiddenY, 0.25f, Axis.Y).Ease(Curve.QuadOut));
			GameMgr.State.SetTutorialBuoyDropped(true);
			m_currentMessageState = MessageState.hidden;
			m_messageButton.onClick.RemoveListener(HideMessageBox);
		}

		#endregion

	}
}
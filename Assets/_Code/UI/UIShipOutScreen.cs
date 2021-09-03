using PotatoLocalization;
using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Events;

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


		// IDs for UnityActions that can occur when a messageBox is closed
		public enum ActionCode
		{
			TutorialSonar,
			TutorialBuoy,
			UnlockDive
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

		private List<UnityAction> GetActions(ActionCode[] codes)
		{
			List<UnityAction> actions = new List<UnityAction>();
			foreach (ActionCode code in codes)
			{
				switch (code)
				{
					case (ActionCode.TutorialSonar):
						actions.Add(FlagDisplaySonarTutorial);
						break;
					case (ActionCode.TutorialBuoy):
						actions.Add(FlagDropTutorialBuoy);
						break;
					case (ActionCode.UnlockDive):
						actions.Add(ShipOutMgr.instance.UnlockDive);
						break;
					default:
						break;
				}
			}

			return actions;
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

		private void FlagDropTutorialBuoy()
		{
			GameMgr.State.SetTutorialBuoyDropped(true);
		}

		private void FlagDisplaySonarTutorial()
		{
			GameMgr.State.SetTutorialSonarDisplayed(true);
		}

		public MessageState GetCurrMessageState()
		{
			return m_currentMessageState;
		}

		#region MessageBox

		public void ShowMessage(string message, string buttonText, ActionCode[] actionCodes)
		{
			m_messageText.text = message;
			m_messageButtonText.text = buttonText;
			m_messageGroup.anchoredPosition = new Vector2(m_messageGroup.anchoredPosition.x, m_messageHiddenY);
			m_messageRoutine.Replace(this, m_messageGroup.AnchorPosTo(m_messageShownY, 0.25f, Axis.Y).Ease(Curve.QuadOut));
			m_currentMessageState = MessageState.showing;
			m_messageButton.onClick.AddListener(HideMessageBox);

			List<UnityAction> actions = GetActions(actionCodes);
			foreach (UnityAction action in actions)
			{
				m_messageButton.onClick.AddListener(action);
			}
		}
		private void HideMessageBox()
		{
			m_messageRoutine.Replace(this, m_messageGroup.AnchorPosTo(m_messageHiddenY, 0.25f, Axis.Y).Ease(Curve.QuadOut));
			m_currentMessageState = MessageState.hidden;
			m_messageButton.onClick.RemoveListener(HideMessageBox);
		}
		#endregion

	}
}
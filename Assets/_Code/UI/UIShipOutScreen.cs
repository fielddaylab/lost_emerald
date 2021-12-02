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

namespace Shipwreck {
	/// <summary>
	/// The canvas of the ShipOut scene
	/// </summary>
	public class UIShipOutScreen : UIBase {
		public enum MessageState {
			hidden,
			showing
		}

		[SerializeField]
		private Button m_returnToOfficeButton; // button that returns the player to office
		[SerializeField]
		private GameObject m_diveButtonPrefab; // the prefab from which the dive button is generated
		private Button m_diveButton = null; // the dive button
		[SerializeField]
		private Slider m_diveSlider; // the dive progress bar

		private Routine m_messageRoutine;

		private MessageState m_currentMessageState;

		public static UIShipOutScreen instance;

		private void Awake() {
			// ensure there is only one ShipOutScreen at any given time
			if (UIShipOutScreen.instance == null) {
				UIShipOutScreen.instance = this;
			}
			else if (UIShipOutScreen.instance != this) {
				Destroy(this.gameObject);
			}

			m_returnToOfficeButton.onClick.AddListener(HandleReturnToOfficeButton);
			m_currentMessageState = MessageState.hidden;
		}

		protected override void OnShowStart() {
			GameMgr.Events.Register(GameEvents.PhoneNotification, HandlePhoneNotification);
			GameMgr.Events.Register(GameEvents.DialogClosed, HandleDialogClosed);
		}

		protected override void OnHideStart() {
			GameMgr.Events.Deregister(GameEvents.PhoneNotification, HandlePhoneNotification);
			GameMgr.Events.Deregister(GameEvents.DialogClosed, HandleDialogClosed);
		}

		/// <summary>
		/// Handles when the office button is clicked
		/// </summary>
		private void HandleReturnToOfficeButton() {
			if (m_currentMessageState == MessageState.showing) {
				return;
			}
			AudioSrcMgr.instance.PlayOneShot("click_return_office");
			AudioSrcMgr.instance.StopAmbiance();
			SceneManager.LoadScene("Main");
			UIMgr.Close<UIShipOutScreen>();
			UIMgr.Open<UIOfficeScreen>();
			AudioSrcMgr.instance.PlayAudio("office_music", true);
			AudioSrcMgr.instance.StopAmbiance();
			AudioSrcMgr.instance.ClearAmbiance();
		}

		/// <summary>
		/// Handles when the dive button is clicked
		/// </summary>
		private void HandleDiveButton() {
			if (m_currentMessageState == MessageState.showing) {
				return;
			}
			// ensure the dive is locked
			if (GameMgr.State.IsDiveUnlocked(ShipOutMgr.instance.GetData().ShipOutIndex)) {
				SceneManager.LoadScene(ShipOutMgr.instance.GetData().DiveDest);
				AudioSrcMgr.instance.PlayOneShot("click_dive");
				AudioSrcMgr.instance.PlayAudio("dive");
				UIMgr.Close<UIShipOutScreen>();
				UIMgr.Open<UIDiveScreen>();
				AudioSrcMgr.instance.QueueAudio("dive_music", true);
				AudioSrcMgr.instance.PlayAmbiance("underwater_ambiance", true);
			}
		}

		/// <summary>
		/// Replaces the filled slider with the clickable dive button
		/// </summary>
		public void SwapButtonForSlider(Vector2 buttonPosition) {
			// destory old bar
			m_diveSlider.gameObject.SetActive(false);

			// create new button
			m_diveButton = Instantiate(m_diveButtonPrefab, transform).GetComponent<Button>();
			((RectTransform)m_diveButton.transform).anchoredPosition = buttonPosition;

			// add button functionality
			m_diveButton.onClick.AddListener(HandleDiveButton);
		}

		/// <summary>
		/// Replaces the filled slider with the clickable dive button
		/// </summary>
		public void ResetUI() {
			// restore initial bar
			m_diveSlider.gameObject.SetActive(true);

			// destory button if it exists
			if (m_diveButton != null) {
				Destroy(m_diveButton.gameObject);
				m_diveButton = null;
			}
		}

		public void DropSonarTutorialBuoy() {
			GameMgr.State.SetTutorialBuoyDropped(true);
			AudioSrcMgr.instance.PlayOneShot("drop_buoy");
		}

		public void MarkSonarTutorialComplete() {
			GameMgr.State.SetTutorialSonarDisplayed(true);
		}

		public MessageState GetCurrMessageState() {
			return m_currentMessageState;
		}

		public Slider GetDiveSlider() {
			return m_diveSlider;
		}

		public void HideDiveSlider() {
			m_diveSlider.gameObject.SetActive(false);
		}

		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}

		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}

		private void HandlePhoneNotification() {
			m_currentMessageState = MessageState.showing;
			m_returnToOfficeButton.interactable = false;
			if (m_diveButton != null) {
				m_diveButton.interactable = false;
			}
		}
		private void HandleDialogClosed() {
			m_currentMessageState = MessageState.hidden;
			m_returnToOfficeButton.interactable = true;
			if (m_diveButton != null) {
				m_diveButton.interactable = true;
			}
		}

	}
}
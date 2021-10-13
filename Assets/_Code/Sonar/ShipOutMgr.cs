/*
 * Organization: Field Day Lab
 * Author(s): Levi Huillet
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck
{
	/// <summary>
	/// Manages the ShipOut scene
	/// </summary>
	public class ShipOutMgr : MonoBehaviour
	{
		public static ShipOutMgr instance; // there can only be one
		public SonarDotGenerator m_sdmgr; // the scene's SonarDotMgr

		[SerializeField]
		private Vector2 m_targetDimensions; // the dimensions of the scene
		[SerializeField]
		private float m_completionPercent; // the percent of solar dots a player must reveal before diving
		private int m_sonarProgress; // the number of dots that have been revealed

		private ShipOutData m_shipOutData;

		[SerializeField]
		private GameObject m_sonarDotPrefab; // the prefab for a sonar dot
		[SerializeField]
		private GameObject m_sonarDotParentPrefab; // prefab for object that groups the generated dots
		private GameObject m_sonarDotParent; // // an empty object that groups the generated dots
		private int m_targetNumDots;

		[SerializeField]
		private GameObject m_buoyPrefab; // prefab for buoy that is dropped
		[SerializeField]
		private GameObject m_playerShip; // the player's ship
		[SerializeField]
		private GameObject m_reyaShipPrefab; // prefab for reya's ship
		private GameObject m_reyaShip; // reya's ship

		public UnityEvent EnableSonar = new UnityEvent();
		public UnityEvent DisableSonar = new UnityEvent();

		private static int DIM_TO_WORLD_PROP = 100; // the proportion of scene dimensions to world space is 100 pixels per unit
		private static Vector3 BUOY_SHIP_OFFSET = new Vector3(-0.7f, -0.7f, 0f); // where the ship is placed relative to buoy when
																				 // re-entering a scene with completed dive

		private bool m_interactIsOverUI; // whether the interaction is over some UI

		public bool GetInteractIsOverUI()
		{
			return m_interactIsOverUI;
		}
		public void SetInteractIsOverUI(bool isOver)
		{
			m_interactIsOverUI = isOver;
		}

		private void Start()
		{
			AudioSrcMgr.instance.PlayAudio("ship_out_music", true);

			// ensure there is only one ShipOutMgr at any given time
			if (ShipOutMgr.instance == null)
			{
				ShipOutMgr.instance = this;
			}
			else if (ShipOutMgr.instance != this)
			{
				Destroy(this.gameObject);
			}

			// convert dimensions to world space
			m_targetDimensions /= DIM_TO_WORLD_PROP;

			m_interactIsOverUI = false;

			m_shipOutData = GameDb.GetShipOutData(GameMgr.State.GetCurrShipOutIndex());

			ShowSonarScene();
		}

		private void GenerateSonarDots(bool revealed = false)
		{
			m_sonarDotParent = Instantiate(m_sonarDotParentPrefab, this.transform);

			// Instantiate SonarDot Prefabs at each point
			List<Vector2> sonarPoints = m_shipOutData.SonarPoints;

			foreach (Vector2 point in sonarPoints)
			{
				GameObject newDot = Instantiate(m_sonarDotPrefab, m_sonarDotParent.transform);
				newDot.transform.position = point + m_shipOutData.WreckLocation;

				if (revealed)
				{
					// reveal dots
					newDot.GetComponent<SpriteRenderer>().enabled = true;

					// remove all box colliders, since sonar is already revealed
					newDot.GetComponent<BoxCollider2D>().enabled = false;
				}
			}

			m_targetNumDots = sonarPoints.Count;
		}

		public void ShowSonarScene()
		{
			UIMgr.Open<UIShipOutScreen>();
			UIShipOutScreen.instance.ResetUI();

			if (m_shipOutData.ShipOutIndex == 1)
			{
				ShowLevel2Sonar();
			}
			else
			{
				ShowDefaultSonar();
			}
		}

		public void ShowDefaultSonar()
		{
			// when dive is unlocked, load the buoy without sonar
			if (GameMgr.State.IsDiveUnlocked(GameMgr.State.GetCurrShipOutIndex()))
			{
				// activate button
				UIShipOutScreen.instance.SwapButtonForSlider();

				// drop buoy
				GameObject buoy = DropBuoy();

				m_playerShip.transform.position = buoy.transform.position + BUOY_SHIP_OFFSET;
			}
			// when the dive is not unlocked, laod the sonar without buoy
			else
			{
				GenerateSonarDots();

				m_sonarProgress = 0;
				UIShipOutScreen.instance.GetDiveSlider().normalizedValue = 0;

				if (!GameMgr.State.HasTutorialSonarDisplayed())
				{
					UIShipOutScreen.ActionCode[] codes = new UIShipOutScreen.ActionCode[]
					{
						UIShipOutScreen.ActionCode.TutorialSonar,
						UIShipOutScreen.ActionCode.EnableSonar
					};
					UIShipOutScreen.instance.ShowMessage(
						"Time to use my sonar to see if I can find the wreck Lou shared",
						"Continue",
						codes
						);
				}
				else
				{
					EnableSonar.Invoke();
				}
			}
		}

		public void ShowLevel2Sonar()
		{
			// when dive is unlocked, load the buoy without sonar
			if (GameMgr.State.IsDiveUnlocked(GameMgr.State.GetCurrShipOutIndex()))
			{
				GenerateSonarDots(true);

				// activate button
				UIShipOutScreen.instance.SwapButtonForSlider();

				// add reya's ship
				AddReya();

				// drop buoy
				GameObject buoy = DropBuoy();

				m_playerShip.transform.position = buoy.transform.position + BUOY_SHIP_OFFSET;
			}
			// when the dive is not unlocked, load the sonar without buoy
			else
			{
				GenerateSonarDots(true);

				UIShipOutScreen.instance.HideDiveSlider();

				// add reya's ship
				AddReya();

				// display reya's speech bubble
				m_reyaShip.GetComponent<ReyaShip>().ActivateBubble();

				// drop buoy
				DropBuoy();
			}
		}

		public GameObject DropBuoy()
		{
			GameObject buoy = Instantiate(m_buoyPrefab);
			buoy.transform.position = m_shipOutData.BuoyLocation;
			AudioSrcMgr.instance.PlayOneShot("drop_buoy");

			return buoy;
		}

		public void AddReya()
		{
			m_reyaShip = Instantiate(m_reyaShipPrefab);
		}

		/// <summary>
		/// Returns the scene's target dimensions
		/// </summary>
		/// <returns></returns>
		public Vector2 GetTargetDimensions()
		{
			return m_targetDimensions;
		}

		/// <summary>
		/// Increments the number of sonar dots that have been revealed, 
		/// then checks if the dive should be unlocked
		/// </summary>
		public void IncrementRevealCount()
		{
			m_sonarProgress = m_sonarProgress + 1;

			// if the dive has not been unlocked yet, check if it should unlock
			if (!GameMgr.State.IsDiveUnlocked(m_shipOutData.ShipOutIndex))
			{
				// how many dots out of the total have been revealed
				float percentComplete = ((float)m_sonarProgress / m_targetNumDots) * 100;

				// what percentage of the proportion needed for the dive have been revealed
				float percentTargetCompletion = percentComplete / m_completionPercent;

				UIShipOutScreen.instance.GetDiveSlider().normalizedValue = percentTargetCompletion;

				// when enough dots have been revealed, show buoy message
				if (percentComplete >= m_completionPercent)
				{
					// when the message is already showing, don't show it again
					if (IsMessageShowing())
					{
						return;
					}

					// in the first sonar scene, the tutorial buoy must display a message before the dive is unlocked
					if (GameMgr.State.HasTutorialBuoyDropped())
					{
						// no tutorial buoy case
						UnlockDive();
					}
					else
					{
						// tutorial buoy case
						UIShipOutScreen.ActionCode[] codes = new UIShipOutScreen.ActionCode[]
						{
							UIShipOutScreen.ActionCode.TutorialBuoy,
							UIShipOutScreen.ActionCode.UnlockDive
						};
						UIShipOutScreen.instance.ShowMessage(
							"There it is! I’ll drop a buoy to mark the location.",
							"Continue",
							codes
							);
					}
				}
			}
		}

		/// <summary>
		/// Loads the relevant scene
		/// </summary>
		/// <param name="sceneName"></param>
		public void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}

		public ShipOutData GetData()
		{
			return m_shipOutData;
		}

		public bool IsMessageShowing()
		{
			return UIShipOutScreen.instance.GetCurrMessageState() == UIShipOutScreen.MessageState.showing;
		}

		/// <summary>
		/// Unlocks dive, activates button, and drops a buoy
		/// </summary>
		public void UnlockDive()
		{
			// unlock dive
			GameMgr.State.UnlockDive(m_shipOutData.ShipOutIndex);

			// activate button
			UIShipOutScreen.instance.SwapButtonForSlider();

			if (m_shipOutData.ShipOutIndex == 1)
			{
				// buoy starts dropped
				return;
			}

			// drop buoy
			DropBuoy();
		}
	}
}

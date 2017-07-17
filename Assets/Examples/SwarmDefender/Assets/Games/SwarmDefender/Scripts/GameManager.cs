using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SwarmDefender
{
	public class GameManager : AGameChangerGameManager {

		public string m_loadLevelName = "Credits";
		[Range(0.5f,10.0f)]
		public float m_weakFireInterval = 1.5f;
		[Range(0.5f,100f)]
		public float m_midFireInterval = 2f;
		[Range(0.5f,100f)]
		public float m_strongFireInterval = 2f;
		public bool IsInGameOverState
		{
			get { return m_isInGameOverState; }
			set { m_isInGameOverState = value; }
		}
		public bool IsInGameWonState
		{
			get { return m_isInGameWonState; }
			set { m_isInGameWonState = value; }
		}
		public bool IsInStartWaitState {
			get { return m_isInStartWaitState; }
			set { m_isInStartWaitState = value; }
		}
		private bool m_isInStartWaitState = true;
		private bool m_isInGameOverState = false;
		private bool m_isInGameWonState = false;
		private int m_showInfoTime = 0;
		private string m_infoString = "";
		private bool m_itIsOver = false;
		public GameObject GameWin;
		public GameObject GameLost;

		private void Update()
		{
			CheckInputs();
		}

		private void Start()
		{
			InvokeRepeating("DecreaseShowInfoTime", 3f, 1f);
			GameWin.GetComponent<Renderer>().enabled = false;
			GameLost.GetComponent<Renderer>().enabled = false;
		}

		private void CheckInputs ()
		{
			

			if (Input.GetButtonDown("Quit Game") || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
			{

				Application.LoadLevel(m_loadLevelName);
			}
			if(Input.GetButtonDown("Start New Round"))
			{


//				if(FadeManager.Instance.IsFading)
//				{
//					m_infoString = "Cannot restart game before intro is done. please wait a bit...";
//					m_showInfoTime = 3;
//					return;
//				}
				Application.LoadLevel(Application.loadedLevel);
			}
//			if (Input.GetButtonDown("Reconnect Tracking Service"))
//			{
//				m_infoString = "Reconnecting tracking service... (Only do this when no avatars appear.)";
//				m_showInfoTime = 3;
//			}
		}

		public IEnumerator RestartGame(bool won, bool immediate)
		{
			if(m_itIsOver) yield break;
			m_itIsOver = true;

			Debug.Log ("### GAME OVER ###");
			if(!won && !immediate) 
			{
				//AudioManager.Instance.FireGameOverSfx();
				GameWin.GetComponent<Renderer>().enabled = false;
				GameLost.GetComponent<Renderer>().enabled = true;
			}
			else if(won && !immediate) 
			{
				//AudioManager.Instance.FireGameWonSfx();
				GameWin.GetComponent<Renderer>().enabled = true;
				GameLost.GetComponent<Renderer>().enabled = false;
			}
			if(won && !immediate)
			{ 
				IsInGameWonState = true;
			}
			else if(!immediate)
			{
				IsInGameOverState = true;
				FadeManager.Instance.PlayMusic = false;
			}
			if(won && !immediate)
			{
				yield return new WaitForSeconds(2);
				FadeManager.Instance.PlayMusic = false;
				//AudioManager.Instance.FireMissionBriefingWon();
				yield return new WaitForSeconds(2);
			}
			else if(!immediate) yield return new WaitForSeconds(3);

			Debug.Log("### ENDING GAME ### ");
			IsInGameOverState = false;
			IsInGameWonState = false;

            Application.LoadLevel(Application.loadedLevel);
        }

		public float GetFireInvertal (SpaceshipPlayer.EFireMode mode)
		{
			float interval = 0;
			switch (mode) {
			case SpaceshipPlayer.EFireMode.WEAK:
				interval = m_weakFireInterval;
				break;
			case SpaceshipPlayer.EFireMode.MID:
				interval = m_midFireInterval;
				break;
			case SpaceshipPlayer.EFireMode.STRONG:
				interval = m_strongFireInterval;
				break;
			default:
				interval = 9999.0f;
				break;
			}

			return interval;
		}
		
		private void OnGUI()
		{
			if(m_showInfoTime > 0)
			{
				GUI.Label(new Rect(10, 10,500,20), m_infoString);
			}
		}
		
		private void DecreaseShowInfoTime()
		{
			if(m_showInfoTime > 0) m_showInfoTime--;
		}
	}
}
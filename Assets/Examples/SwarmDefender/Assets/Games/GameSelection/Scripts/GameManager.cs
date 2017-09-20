using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameSelection
{
	public enum EGameType
	{
		LEFTTOP,
		CENTERTOP,
		RIGHTTOP,
		LEFTBOTTOM,
		CENTERBOTTOM,
		RIGHTBOTTOM,
		Count
	}

	public class GameManager : AGameChangerGameManager {

		public string m_LevelNameF1;
		public string m_LevelNameF2;
		public string m_LevelNameF3;
		public string m_LevelNameF4;
		public string m_LevelNameF5;
		public string m_SceneNameLeftTop;
		public string m_SceneNameCenterTop;
		public string m_SceneNameRightTop;
		public string m_SceneNameLeftBottom;
		public string m_SceneNameRightBottom;
		[Range(3,30)]
		public int m_countdownTime;
		[Range(0,10)]
		public int m_waitToLoadTime;
		public TextMesh m_countdownTextMesh; 
		public SpriteRenderer m_waitingSprite;
		public SpriteRenderer m_pauseSprite;
		public SpriteRenderer m_maskLeftTopSprite;
		public SpriteRenderer m_maskCenterTopSprite;
		public SpriteRenderer m_maskRightTopSprite;
		public SpriteRenderer m_maskLeftBottomSprite;
		public SpriteRenderer m_maskRightBottomSprite;

		public bool NextGameDecided 
		{
			get { return m_nextGameDecided; }
		}

		private string m_loadSceneName;
		private int m_maxValue;
		private EGameType m_winnerGameType;
		private bool m_nextGameDecided = false;
		private bool m_countdownRunning = false;
		private Dictionary<EGameType, int> m_voteDict = new Dictionary<EGameType, int>();
		private List<EGameType> m_drawGameTypes = new List<EGameType>();
		private bool m_pause = false;

		protected override void Awake()
		{
			base.Awake();
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Confined;
		}

		private void Start()
		{
			m_voteDict.Add(EGameType.LEFTTOP, 0);
			m_voteDict.Add(EGameType.CENTERTOP, 0);
			m_voteDict.Add(EGameType.RIGHTTOP, 0);
			m_voteDict.Add(EGameType.LEFTBOTTOM, 0);
			m_voteDict.Add(EGameType.CENTERBOTTOM, 0);
			m_voteDict.Add(EGameType.RIGHTBOTTOM, 0);

			m_countdownTextMesh.text = m_countdownTime.ToString();
			ResetCountdown();
		}

		private void Update()
		{
			ListenForInputs();

			if(
				m_countdownRunning && 
				(PlayerManager.Instance as PlayerManager).GetPlayerList().Count < 1 && 
				!m_pause &&
				!m_nextGameDecided
				)
			{
				StopCountdown ();
				ResetCountdown();
				m_waitingSprite.GetComponent<Renderer>().enabled = true;
			}
			if(
				!m_countdownRunning && 
			   	(PlayerManager.Instance as PlayerManager).GetPlayerList().Count > 0 && 
				!m_pause &&
				!m_nextGameDecided
				)
			{
				m_waitingSprite.GetComponent<Renderer>().enabled = false;
				StartCountdown();
			}
		}

		private void ListenForInputs ()
		{
			if(Input.GetButtonDown("Quit Game") || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
			{
				Application.LoadLevel("Credits");
			}
			if(Input.GetButtonDown("Start New Round"))
			{
				Application.LoadLevel(Application.loadedLevel);
			}
			if(Input.GetKeyDown(KeyCode.P))
			{
				// toggle Pause
				TogglePause();
			}
			if(Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.Alpha1))
			{
				Application.LoadLevel(m_LevelNameF1);
			}
			if(Input.GetKeyDown(KeyCode.F2) || Input.GetKeyDown(KeyCode.Alpha2))
			{
				Application.LoadLevel(m_LevelNameF2);
			}
			if(Input.GetKeyDown(KeyCode.F3) || Input.GetKeyDown(KeyCode.Alpha3))
			{
				Application.LoadLevel(m_LevelNameF3);
			}
			if(Input.GetKeyDown(KeyCode.F4) || Input.GetKeyDown(KeyCode.Alpha4))
			{
				Application.LoadLevel(m_LevelNameF4);
			}
			if(Input.GetKeyDown(KeyCode.F5) || Input.GetKeyDown(KeyCode.Alpha5))
			{
				Application.LoadLevel(m_LevelNameF5);
			}
		}

		private IEnumerator Countdown()
		{
			float endTime = Time.time + m_countdownTime + 1;
			int i = m_countdownTime;
			while(Time.time < endTime)
			{
				i--;
				if(i < 4) AudioManager.Instance.FireCountdownSfx();
				yield return new WaitForSeconds(1);
				m_countdownTextMesh.text = (i+1).ToString();
			}

			m_countdownTextMesh.transform.localPosition = new Vector3(-14.2f, 0,0);

			CountVotes();
			AudioManager.Instance.FireCountdownEndSfx();
			yield return new WaitForSeconds(m_waitToLoadTime);
			LoadGame();
		}

		private void ResetCountdown ()
		{
			m_countdownTextMesh.text = m_countdownTime.ToString();
		}

		private void CountVotes ()
		{
			List<Player> playerList = (PlayerManager.Instance as PlayerManager).GetPlayerList();
			foreach (Player player in playerList) 
			{
				m_voteDict[player.VoteForGameType] = m_voteDict[player.VoteForGameType] + 1;
			}

			m_maxValue = 0;
			m_winnerGameType = EGameType.CENTERBOTTOM;

			foreach (KeyValuePair<EGameType, int> entry in m_voteDict) 
			{
				// center bottom can't be winning
				if(entry.Key == EGameType.CENTERBOTTOM) continue;
				if(entry.Value > m_maxValue)
				{
					m_winnerGameType = entry.Key;
					m_maxValue = entry.Value;
				}
			}
			foreach (KeyValuePair<EGameType, int> entry in m_voteDict) 
			{
				// center bottom can't be winning
				if(entry.Key == EGameType.CENTERBOTTOM) continue;
				if(entry.Value == m_maxValue)
				{
					m_drawGameTypes.Add(entry.Key);
				}
			}

			if(m_drawGameTypes.Count > 1)
			{
				if(m_maxValue >= 1)
				{
					Debug.Log ("--- Voting draw ---");
					m_winnerGameType = m_drawGameTypes[Random.Range(0,m_drawGameTypes.Count)];
				}
				else // if there are no votes for an actual game
				{
					m_winnerGameType = EGameType.CENTERBOTTOM;
				}
			}

			Debug.Log (string.Format ("--- Voting winner: {0} with {1} votes ---", m_winnerGameType, m_maxValue));

			switch(m_winnerGameType)
			{
				case EGameType.LEFTTOP:
					m_loadSceneName = m_SceneNameLeftTop;
					m_maskLeftTopSprite.GetComponent<Renderer>().enabled = true;
					break;
				case EGameType.CENTERTOP:
					m_loadSceneName = m_SceneNameCenterTop;
					m_maskCenterTopSprite.GetComponent<Renderer>().enabled = true;
					break;
				case EGameType.RIGHTTOP:
					m_loadSceneName = m_SceneNameRightTop;
					m_maskRightTopSprite.GetComponent<Renderer>().enabled = true;
					break;
				case EGameType.LEFTBOTTOM:
					m_loadSceneName = m_SceneNameLeftBottom;
					m_maskLeftBottomSprite.GetComponent<Renderer>().enabled = true;
					break;
				case EGameType.RIGHTBOTTOM:
					m_loadSceneName = m_SceneNameRightBottom;
					m_maskRightBottomSprite.GetComponent<Renderer>().enabled = true;
					break;
				case EGameType.CENTERBOTTOM:
					m_loadSceneName = Application.loadedLevelName;
					break;
				default:
					m_loadSceneName = Application.loadedLevelName;
					break;
			}
			m_nextGameDecided = true;
		}

		private void LoadGame()
		{
			if(m_maxValue < 1)
			{
				Application.LoadLevel(Application.loadedLevel);
				return;
			}

			Debug.Log (string.Format ("--- Starting Game: {0} ---", m_loadSceneName));
			if(m_loadSceneName != "") 
			{
				Application.LoadLevel(m_loadSceneName);
			}
			else
			{
				Debug.LogWarning("--- Oops, I don't know which game to load ---");
				Application.LoadLevel(Application.loadedLevel);
			}
		}

		private void TogglePause ()
		{
			if(m_nextGameDecided) return;

			if(m_pause)
			{
				m_pause = false;
				m_pauseSprite.GetComponent<Renderer>().enabled = false;
				m_waitingSprite.GetComponent<Renderer>().enabled = true;
			}
			else
			{
				m_pause = true;
				StopCountdown();
				m_pauseSprite.GetComponent<Renderer>().enabled = true;
				m_waitingSprite.GetComponent<Renderer>().enabled = false;

			}
		}

		void StartCountdown ()
		{
			m_countdownRunning = true;
			StartCoroutine("Countdown");
		}

		void StopCountdown ()
		{
			StopCoroutine ("Countdown");
			m_countdownRunning = false;
			ResetCountdown();
		}
	}
}
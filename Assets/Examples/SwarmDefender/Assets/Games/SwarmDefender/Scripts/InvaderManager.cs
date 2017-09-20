using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SwarmDefender
{
	public class InvaderManager : AManager<InvaderManager> {

		public GameObject m_invaderPrefab;
		[Range(0,200)]
		public int m_invaderTotalAmountMin = 50;
		[Range(0,50)]
		public int m_invaderTotalAmountPerPlayer = 12;
		[Range(0f,1f)]
		public float m_MidInvaderProbability = 0.7f;
		[Range(0f,1f)]
		public float m_BossProbability = 0.9f;
		public Color m_weakInvaderColor = Color.green;
		[Range(0,50)]
		public float m_weakDamage = 5;
		[Range(1,10)]
		public float m_weakHealth = 1;
		public Color m_midInvaderColor = Color.blue;
		[Range(0,50)]
		public float m_midDamage = 15;
		[Range(1,10)]
		public float m_midHealth = 5;
		public Color m_bossInvaderColor = Color.red;
		[Range(0,50)]
		public float m_bossDamage = 35;
		[Range(1,10)]
		public float m_bossHealth = 10;
		[Range(0.0f,4.0f)]
		public float m_initInvadersPerPlayer = 1f;
		[Range(0.0f,4.0f)]
		public float m_endInvadersPerPlayer = 2f;
		public TextMesh m_remainingInvaderText;

		private int m_invaderTotalAmount = 0;
		private int m_minInvaderAmount = 0;
		private int m_destroyedInvaderAmount = 0;
		private float m_currentInvadersPerPlayer = 0;
		
		private List<Invader> m_invaderList;
		private Queue<string> invaderSpawnQueue;
		private void Start()
		{
			m_remainingInvaderText.color = Color.clear;
			m_invaderList = new List<Invader>();
			m_currentInvadersPerPlayer = m_initInvadersPerPlayer;
			invaderSpawnQueue = new Queue<string>();

			Invoke ("ComputeTotalInvaderAmount", 3f);
			Invoke ("UpdateInvaderAmountGUI", 3.5f);
		}

		private void Update()
		{
			while
			(
				(m_invaderList.Count < m_minInvaderAmount) && 
				!(GameManager.Instance as GameManager).IsInStartWaitState &&
				((m_destroyedInvaderAmount + m_invaderList.Count) < m_invaderTotalAmount)
			)
			{
				SpawnInvader();
			}
		}
		public void EnqueueInvader(string name)
		{
			invaderSpawnQueue.Enqueue(name);
		}

		private void SpawnInvader ()
		{
			Invader newInvader = (GameObject.Instantiate(m_invaderPrefab, GetInvaderSpawnPos(), Quaternion.identity) as GameObject).GetComponent<Invader>();

			Invader.EType type = Invader.EType.INVADER_WEAK;
			float random = Random.value;

			if(random >= m_BossProbability && (PlayerManager.Instance as PlayerManager).GetPlayerList().Count >= 3) type = Invader.EType.INVADER_BOSS;
			else if (random >= m_MidInvaderProbability && (PlayerManager.Instance as PlayerManager).GetPlayerList().Count >= 2) type = Invader.EType.INVADER_MID; 

			newInvader.Type = type;
			m_invaderList.Add (newInvader);
			if (invaderSpawnQueue.Count > 0)
			{
				newInvader.NameText.text = invaderSpawnQueue.Dequeue();

			}
			else
			{
				newInvader.NameText.text = "";
			}


		}

		private Vector3 GetInvaderSpawnPos ()
		{
			float y = UnityTracking.TrackingAdapter.TargetScreenHeight + 100;
			float x = Random.Range(0, UnityTracking.TrackingAdapter.TargetScreenWidth);
			return new Vector3(x,y,0);
		}

		public void PlayerCountChange (int count)
		{
			ComputeMinInvaderAmount (count);
		}

		public void RemoveInvaderFromList (Invader invader)
		{
			if(m_invaderList.Contains(invader))
			{
				m_invaderList.Remove(invader);
				m_destroyedInvaderAmount++;
				if(!(GameManager.Instance as GameManager).IsInGameWonState && !(GameManager.Instance as GameManager).IsInGameOverState)
				{
					UpdateInvaderAmountGUI ();
				}
				ComputeInvadersPerPlayer();
				StartCoroutine(CheckForGameWon ());
			}
		}

		private IEnumerator CheckForGameWon ()
		{
			yield return new WaitForSeconds(1);
			if ((m_destroyedInvaderAmount >= m_invaderTotalAmount) && 
				!(GameManager.Instance as GameManager).IsInGameOverState) 
			{
				StartCoroutine ((GameManager.Instance as GameManager).RestartGame (true, false));
			}
		}

		public float GetDamageValueFromType (Invader.EType invaderType)
		{
			float returnValue = 0;
			switch (invaderType) {
				case Invader.EType.INVADER_WEAK:
					returnValue = m_weakDamage;
					break;
				case Invader.EType.INVADER_MID:
					returnValue = m_midDamage;
					break;
				case Invader.EType.INVADER_BOSS:
					returnValue = m_bossDamage;
					break;
				default:
					returnValue = 0;
					break;
			}

			return returnValue;
		}

		private void ComputeInvadersPerPlayer ()
		{
			float lerpProgress = ((m_destroyedInvaderAmount*1f) / (m_invaderTotalAmount*1f));
			m_currentInvadersPerPlayer = Mathf.Lerp(m_initInvadersPerPlayer, m_endInvadersPerPlayer, lerpProgress);
			ComputeMinInvaderAmount((PlayerManager.Instance as PlayerManager).GetPlayerList().Count);
		}

		private void ComputeMinInvaderAmount (int playerCount)
		{
			m_minInvaderAmount = Mathf.RoundToInt (playerCount * m_currentInvadersPerPlayer);
//			Debug.Log ("r: " + (m_invaderTotalAmount - m_destroyedInvaderAmount) + ", pp:" + m_currentInvadersPerPlayer + ", currentAmount:" + m_minInvaderAmount);
		}

		private void ComputeTotalInvaderAmount ()
		{
			m_invaderTotalAmount = (m_invaderTotalAmountPerPlayer * (PlayerManager.Instance as PlayerManager).GetPlayerList().Count);

			if(m_invaderTotalAmount < m_invaderTotalAmountMin)
			{
				m_invaderTotalAmount = m_invaderTotalAmountMin;
			}
		}

		private void UpdateInvaderAmountGUI ()
		{
			m_remainingInvaderText.color = new Color(1f,1f,1f,(192f/255f));
			m_remainingInvaderText.text = (m_invaderTotalAmount - m_destroyedInvaderAmount).ToString () + "x";
		}
	}
}

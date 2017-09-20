using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SwarmDefender
{
	public class PlayerManager : AGameChangerPlayerManager {

		public List<Sprite> m_playerSprites;
		
		private int m_spriteCounter = 0;
		private Dictionary<int, bool> m_playerSpriteAvailabilityDict;
		private int m_playerNum = 0;
		
		protected override void Awake()
		{
			base.Awake();
			
			// init dictionary for each sprite as available
			m_playerSpriteAvailabilityDict = new Dictionary<int, bool>();
			for (int i = 0; i < m_playerSprites.Count; i++)
			{
				m_playerSpriteAvailabilityDict.Add(i, true);
			}
		}
		
		private void AddSpriteToPlayer (AGameChangerPlayer player)
		{
			
			bool spriteSet = false;
			for (int i = 0; i < m_playerSprites.Count; i++) 
			{
				int ii = (m_spriteCounter + i) % m_playerSprites.Count;
				if (m_playerSpriteAvailabilityDict [ii]) 
				{
					(player as SpaceshipPlayer).m_spaceshipSpriteRenderer.sprite  = m_playerSprites [ii];
					m_playerSpriteAvailabilityDict [ii] = false;
					(player as SpaceshipPlayer).SpriteIndex = ii;
					m_spriteCounter = ii;
//					Debug.Log("apply sprite " + ii + " to: " + player.gameObject.name);
					spriteSet = true;
					break;
				}
			}
			
			if(!spriteSet)
			{
				int iii = Random.Range(0, m_playerSprites.Count);
				(player as SpaceshipPlayer).m_spaceshipSpriteRenderer.sprite  = m_playerSprites [iii];
				(player as SpaceshipPlayer).SpriteIndex = iii;
				m_spriteCounter = iii;
//				Debug.Log("apply sprite " + iii + " to: " + player.gameObject.name);
				spriteSet = true;
				return;
			}

		}
		
		public override void AddPlayer (long sessionID, Vector2 position)
		{
			AGameChangerPlayer player = (GameObject.Instantiate(m_playerPrefab, new Vector3(position.x,position.y,0), Quaternion.identity) as GameObject).GetComponent<AGameChangerPlayer>();
			
			player.gameObject.name = string.Format ("player_{0}", ++m_playerNum);
			
			player.TrackID = sessionID;
			m_playerList.Add(player);
			InvaderManager.Instance.PlayerCountChange(m_playerList.Count);
			
			AddSpriteToPlayer (player);
		}
		
		public override void RemovePlayer (long sessionID)
		{
			foreach (AGameChangerPlayer player in m_playerList.ToArray()) 
			{
				if(player.TrackID.Equals(sessionID))
				{
//					Debug.Log("remove sprite " + (player as SpaceshipPlayer).SpriteIndex + " from: " + player.gameObject.name);
					m_playerSpriteAvailabilityDict[(player as SpaceshipPlayer).SpriteIndex] = true;
					GameObject.Destroy(player.gameObject);
					m_playerList.Remove(player);
				}	
			}
			InvaderManager.Instance.PlayerCountChange(m_playerList.Count);
		}
		
		public List<AGameChangerPlayer> GetPlayerList()
		{
			return this.m_playerList;
		}

	}
}


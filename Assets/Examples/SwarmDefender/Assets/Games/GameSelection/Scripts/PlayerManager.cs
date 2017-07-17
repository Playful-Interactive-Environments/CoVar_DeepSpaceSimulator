using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameSelection
{
	public class PlayerManager : AGameChangerPlayerManager {

		public List<Player> GetPlayerList()
		{
			List<Player> pList = new List<Player>();
			foreach (AGameChangerPlayer aPlayer in m_playerList) 
			{
				pList.Add(aPlayer as Player);
			}
			return pList;
		}
	}
}
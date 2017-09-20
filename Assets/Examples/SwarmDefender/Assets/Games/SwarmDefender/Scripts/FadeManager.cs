using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SwarmDefender
{
	public class FadeManager : AManager<FadeManager> {
		
		private bool m_playMusic = false;
		private bool m_isFading = false;

		public bool IsFading
		{
			get { return m_isFading; }
			set { m_isFading = value; }
		}
		public bool PlayMusic
		{
			get { return m_playMusic; }
			set { m_playMusic = value; }
		}

		protected override void Awake()
		{
			base.Awake();
		}
		
		private void Start () 
		{
			if(!m_isFading)
			{
				m_isFading = true;
				StartCoroutine(FireGameStartFading());
			}
		}
		
		private IEnumerator FireGameStartFading ()
		{
			m_playMusic = true;
			AudioManager.Instance.FireFadeSfx();
//			yield return new WaitForSeconds(10);
//			AudioManager.Instance.FireMissionBriefingInvaderStart();
			yield return new WaitForSeconds(3);
			//AudioManager.Instance.FireMissionBriefingStart();
			yield return new WaitForSeconds(2);
			(GameManager.Instance as GameManager).IsInStartWaitState = false;
			m_isFading = false;
		}
	}
}

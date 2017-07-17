using UnityEngine;
using System.Collections;

namespace GameSelection
{
	public class Player : AGameChangerPlayer {

		public EGameType VoteForGameType 
		{
			get { return m_voteForGameType; }
		}

		private EGameType m_voteForGameType;

		private void Update()
		{
			DetermineVoteGameType ();
		}

		private void DetermineVoteGameType ()
		{
			m_voteForGameType = EGameType.CENTERBOTTOM;
			if(this.transform.position.y > UnityTracking.TrackingAdapter.TargetScreenHeight/2)
			{
				if(
					this.transform.position.x > 0 && 
					this.transform.position.x < (UnityTracking.TrackingAdapter.TargetScreenWidth/3*1)
					)
				{
					m_voteForGameType = EGameType.LEFTTOP;
				}
				else if(
					this.transform.position.x > (UnityTracking.TrackingAdapter.TargetScreenWidth/3*1) && 
					this.transform.position.x < (UnityTracking.TrackingAdapter.TargetScreenWidth/3*2)
					)
				{
					m_voteForGameType = EGameType.CENTERTOP;
				}
				else if(
					this.transform.position.x > (UnityTracking.TrackingAdapter.TargetScreenWidth/3*2) && 
					this.transform.position.x < (UnityTracking.TrackingAdapter.TargetScreenWidth)
					)
				{
					m_voteForGameType = EGameType.RIGHTTOP;
				}
			}
			else
			{
				if(
					this.transform.position.x > 0 && 
					this.transform.position.x < (UnityTracking.TrackingAdapter.TargetScreenWidth/3*1)
					)
				{
					m_voteForGameType = EGameType.LEFTBOTTOM;
				}
				else if(
					this.transform.position.x > (UnityTracking.TrackingAdapter.TargetScreenWidth/3*1) && 
					this.transform.position.x < (UnityTracking.TrackingAdapter.TargetScreenWidth/3*2)
					)
				{
					m_voteForGameType = EGameType.CENTERBOTTOM;
				}
				else if(
					this.transform.position.x > (UnityTracking.TrackingAdapter.TargetScreenWidth/3*2) && 
					this.transform.position.x < (UnityTracking.TrackingAdapter.TargetScreenWidth)
					)
				{
					m_voteForGameType = EGameType.RIGHTBOTTOM;
				}
			}
		}
	}
}
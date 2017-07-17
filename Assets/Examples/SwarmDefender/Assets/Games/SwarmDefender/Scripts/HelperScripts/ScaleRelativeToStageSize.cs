using UnityEngine;
using System.Collections;
using UnityTracking;

namespace SwarmDefender
{
	public class ScaleRelativeToStageSize : MonoBehaviour 
	{

		public float initStageXScale = 1600f;
		public float initStageYScale = 900f;

		void Start()
		{
			this.transform.localScale = new Vector3 (this.transform.localScale.x * (initStageXScale / TrackingAdapter.TrackingStageX), this.transform.localScale.y * (initStageYScale / TrackingAdapter.TrackingStageY), this.transform.localScale.z);
		}
	}
}
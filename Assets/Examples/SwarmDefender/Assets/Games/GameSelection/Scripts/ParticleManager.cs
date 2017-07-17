using UnityEngine;
using System.Collections;

namespace GameSelection
{

	public class ParticleManager : MonoBehaviour {

		public GameObject m_particlesPrefabWhite;
		[Range(0,30)]
		public int m_paricleSystemAmount;

		private bool m_particlesFired = false;

		private void Update()
		{
			if((GameManager.Instance as GameManager).NextGameDecided && !m_particlesFired)
			{
				m_particlesFired = true;
				FireParticles();
			}
		}

		private void FireParticles ()
		{
			GameObject myObject = new GameObject();
			Quaternion quat = Quaternion.Euler(new Vector3(-90,0,0));
			myObject = m_particlesPrefabWhite;
			myObject.GetComponent<Renderer>().sortingLayerName = "5";

			for (int i = 0; i < m_paricleSystemAmount; i++) 
			{
				Vector3 pos = new Vector3(Random.Range(0,UnityTracking.TrackingAdapter.TargetScreenWidth),Random.Range(0,UnityTracking.TrackingAdapter.TargetScreenHeight),120);
				GameObject.Destroy(GameObject.Instantiate(myObject,pos,quat),4f);
			}
		}
	}
}
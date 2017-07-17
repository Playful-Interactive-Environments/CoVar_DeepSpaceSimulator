using UnityEngine;
using System.Collections;

namespace SwarmDefender
{
	[RequireComponent(typeof(TriggerScript))]
	public class Bullet : MonoBehaviour {

		[Range(0,64)]
		public float m_speed;
		[Range(0,10)]
		public float m_damage;
		public TriggerScript m_triggerScript;
		public bool m_destroyAfterTimer = false;
		public float m_destroyTimer;

		private float m_maxHeight;
		private SpaceshipPlayer m_owner;

		public SpaceshipPlayer Owner
		{
			get { return m_owner; }
			set { m_owner = value; }
		}

		private void Start () 
		{
			if(m_triggerScript != null){
				m_triggerScript.FireTriggerEnterEvent += DestroyMe;
			} else {
				Debug.LogWarning("TriggerScript could not be found!");
			}

			m_maxHeight = UnityTracking.TrackingAdapter.TargetScreenHeight + 20.0f;

			if(m_destroyAfterTimer) Invoke("DestroyMeInstantly", m_destroyTimer);

			// move object to bottom pivot of sprite
			this.transform.localPosition = new Vector3(this.transform.localPosition.x,this.transform.localPosition.y + ((GetComponent<Renderer>() as SpriteRenderer).sprite.rect.height/2), this.transform.localPosition.z);
		}

		private void FixedUpdate () 
		{
			if(m_speed != 0) Move();
			if(this.transform.position.y > m_maxHeight) DestroyMeInstantly();
		}

		private void Move ()
		{
			this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y + m_speed,0);
		}		
		
		public void DestroyMe (GameObject theSender)
		{
//			Debug.Log("destroy bullet from " + theSender.transform.parent.gameObject.name);
			GameObject.Destroy(this.gameObject);
		}

		private void DestroyMeInstantly ()
		{
			GameObject.Destroy(this.gameObject);
		}
	}
}

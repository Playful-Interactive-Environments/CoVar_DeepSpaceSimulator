using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SwarmDefender
{
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Animator))]
	public class Invader : MonoBehaviour {

		public enum EType
		{
			INVADER_WEAK,
			INVADER_MID,
			INVADER_BOSS,
			Count
		}

		public TriggerScript m_triggerScript;
		[Range(10f,60f)]
		public float m_minMoveTimeInSec = 30f;
		[Range(10f,60f)]
		public float m_maxMoveTimeInSec = 50f;
		[Range(0f,2f)]
		public float m_midMoveTimeMultiplicator = 1.75f;
		[Range(0f,2f)]
		public float m_bossMoveTimeMultiplicator = 1.5f;
		public GameObject m_explosionPrefab;
		public GameObject m_invasionEffectPrefab;
		public EType Type {
			get {
				return m_type;
			}
			set {
				m_type = value;
				switch (value) {
				case EType.INVADER_WEAK:
					(this.GetComponent<Renderer>() as SpriteRenderer).color = InvaderManager.Instance.m_weakInvaderColor;
					m_triggerScript.m_tagsToCheck.Add("Bullet_Weak");
					m_triggerScript.transform.gameObject.tag = "Invader_Weak";
					m_health = InvaderManager.Instance.m_weakHealth;
					this.gameObject.name = "Invader_Weak";
					break;
				case EType.INVADER_MID:
					(this.GetComponent<Renderer>() as SpriteRenderer).color = InvaderManager.Instance.m_midInvaderColor;
					m_triggerScript.m_tagsToCheck.Add("Bullet_Mid");
					m_triggerScript.transform.gameObject.tag = "Invader_Mid";
					m_health = InvaderManager.Instance.m_midHealth;
					this.gameObject.name = "Invader_Mid";
					break;
				case EType.INVADER_BOSS:
					(this.GetComponent<Renderer>() as SpriteRenderer).color = InvaderManager.Instance.m_bossInvaderColor;
					this.transform.localScale = new Vector3(3,3,0);
					m_triggerScript.m_tagsToCheck.Add("Bullet_Strong");
					m_triggerScript.transform.gameObject.tag = "Invader_Boss";
					m_health = InvaderManager.Instance.m_bossHealth;
					this.gameObject.name = "Invader_Boss";
					break;
				}
			}
		}

		private Animator m_animator;
		private float m_moveLerpTime = 0;
		private float m_moveLerpProgress = 0;
		private Vector3 m_target = Vector3.zero;
		private Vector3 m_spawnPos = Vector3.zero;
		private EType m_type = EType.INVADER_WEAK;
		private float m_health;
	    public TextMesh NameText;
		private void Start()
		{
			m_animator = this.GetComponent<Animator>();

			if(m_triggerScript != null){
				m_triggerScript.FireTriggerEnterEvent += GetDamage;
			} else {
				Debug.LogWarning("TriggerScript could not be found!");
			}

			m_moveLerpTime = Random.Range(m_minMoveTimeInSec,m_maxMoveTimeInSec);
			if(Type == EType.INVADER_BOSS) m_moveLerpTime *= m_bossMoveTimeMultiplicator;
			if(Type == EType.INVADER_MID) m_moveLerpTime *= m_midMoveTimeMultiplicator;

			m_spawnPos = this.transform.position;
			FindMoveTarget();

			SetAnimation();
		    NameText.color = GetComponent<SpriteRenderer>().color;

		}

		private void Update()
		{
			MoveTowardsTarget();
			if(!IsFarFromTarget())
			{
//				Debug.LogWarning ("This should not happen!");
				DestroyMeInstantly();
			} 
		}

		private void SetAnimation ()
		{
			int invaderType = Mathf.RoundToInt(Random.Range(1,4));
			m_animator.Play ("Invader_" + invaderType.ToString() + "_anim_1");
		}

		private void FindMoveTarget ()
		{
			float randX = Random.Range(0 + 200,UnityTracking.TrackingAdapter.TargetScreenWidth - 200);
			float targetY = -100.0f;
			
			m_target = new Vector3(
				randX,
				targetY,
				0);
		}

		private void MoveTowardsTarget ()
		{
			if (m_moveLerpProgress < 1.0f) {
				m_moveLerpProgress += (Time.deltaTime/m_moveLerpTime);
				this.transform.position = Vector3.Lerp (m_spawnPos,m_target,m_moveLerpProgress);
			}
		}

		private bool IsFarFromTarget ()
		{
			return (Vector3.Distance(this.transform.position, m_target) > 20);
		}

		private void SpawnExplosions (bool isInvasion)
		{
			int amount = 1;
			bool random = false;
			Vector3 pos = this.transform.position;
			if(Type == EType.INVADER_BOSS)
			{
				random = true;
				amount = 3;
			}
			for (int i = 0; i < amount; i++) {
				if(random)
				{
					float randX = Random.Range(-64,64);
					float randY = Random.Range(-64,64);
					pos = new Vector3(this.transform.position.x + randX, this.transform.position.y + randY, 1);
				}
				GameObject explosion;
				if(isInvasion)
				{
					explosion = (GameObject)GameObject.Instantiate(m_invasionEffectPrefab,pos,Quaternion.identity);
				}
				else
				{
					explosion = (GameObject)GameObject.Instantiate(m_explosionPrefab,pos,Quaternion.identity);
					explosion.GetComponent<SpriteRenderer>().color = (this.GetComponent<Renderer>() as SpriteRenderer).color;
				}
				GameObject.Destroy(explosion,1f);
			}
		}

		public void InvadeEarth ()
		{
			SpawnExplosions(true);
			DestroyMe();
		}
		
		private void GetDamage(GameObject theSender)
		{
			m_health -= theSender.GetComponent<Bullet>().m_damage;
			if(m_health <= 0)
			{
				SpawnExplosions(false);
				DestroyMe();
			}
		}

		private void DestroyMe ()
		{
			InvaderManager.Instance.RemoveInvaderFromList(this);
			GameObject.Destroy(this.gameObject);
		}

		private void DestroyMeInstantly ()
		{
			// only called when Invader reaches bottom if board and is not destroyed by earth for some reason
			InvaderManager.Instance.RemoveInvaderFromList(this);
			GameObject.Destroy(this.gameObject);
		}
	}
}

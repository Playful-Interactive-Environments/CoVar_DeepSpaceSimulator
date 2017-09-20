using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SwarmDefender
{
	public class Earth : MonoBehaviour {

		public float m_health = 100.0f;
		public TriggerScript m_earthZoneTriggerScript;
		public SpriteRenderer m_eartSprite;
		public SpriteRenderer m_eartIcon;
		public SpriteRenderer m_border;
		public TextMesh m_healthLabel;
		public Color m_healthbarColorStart = Color.white;
		public Color m_healthbarColorEnd = Color.red;
		public Color m_earthIconColorStart = Color.white;
		public Color m_earthIconColorEnd = Color.white;
		public Color m_earthSpriteColorStart = Color.white;
		public Color m_earthSpriteColorEnd = Color.white;
		public Color m_borderFlashColor = Color.red;
		public Transform m_explosionPos;
		public GameObject m_explosionPrefab;
		public Color m_explosionColor = Color.yellow;
		[Range(-100f,0f)]
		public float m_destroyedMoveSpeed = -50.0f;

		private bool m_earthDestroyed = false;

		public GameObject invaderblank;

		private void Start()
		{
			m_earthZoneTriggerScript.FireTriggerEnterEvent += GetDamage;
			UpdateHealthIndicator(m_health);
		}

		private void Update()
		{
			if (m_earthDestroyed)
			{
				this.transform.position = new Vector3(transform.position.x,transform.position.y + m_destroyedMoveSpeed * Time.deltaTime,0);
			}
		}

		private void GetDamage(GameObject theSenderGO)
		{
			Invader invader = theSenderGO.transform.parent.GetComponent<Invader>();
			m_health -= InvaderManager.Instance.GetDamageValueFromType(invader.Type);

			invader.InvadeEarth();
			StartCoroutine(FlashBorder());

//			Debug.Log ("### EARTH HEALTH: " + m_health + " ###");

			if(m_health <= 0 && !m_earthDestroyed)
			{
				DestroyEarth ();
			}

			UpdateHealthIndicator(m_health);
		}

		private void DestroyEarth ()
		{
			m_earthDestroyed = true;
			StartCoroutine ("Explode");
			StartCoroutine ((GameManager.Instance as GameManager).RestartGame (false, false));
		}

		private IEnumerator Explode ()
		{
			InvokeRepeating("SpawnExplosion",0f,0.3f);
			yield return new WaitForSeconds(4);
			CancelInvoke("SpawnExplosion");
		}

		private void SpawnExplosion()
		{
			float randX = Random.Range(-300,300);
			float randY = Random.Range(-64,64);
			Vector3 pos = new Vector3(m_explosionPos.position.x + randX, m_explosionPos.position.y + randY, 1);
			GameObject explosion = (GameObject)GameObject.Instantiate(m_explosionPrefab,pos,Quaternion.identity);
			explosion.GetComponent<SpriteRenderer>().color = m_explosionColor;
			GameObject.Destroy(explosion,1f);
		}

		private void UpdateHealthIndicator(float newHealth)
		{
			if(newHealth < 0) newHealth = 0;
			float lerpProgress = (1-(newHealth/100.0f));
			Color colorIndicator = Color.Lerp(m_healthbarColorStart, m_healthbarColorEnd, lerpProgress);
			Color colorEarthSprite = Color.Lerp(m_earthSpriteColorStart, m_earthSpriteColorEnd, lerpProgress);
			Color colorEarthIcon = Color.Lerp(m_earthIconColorStart, m_earthIconColorEnd, lerpProgress);
			m_healthLabel.color = colorIndicator;
			m_healthLabel.text = newHealth + "%";

			m_eartSprite.color = colorEarthSprite;
			m_eartIcon.color = colorEarthIcon;
		}

		private IEnumerator FlashBorder()
		{
			m_border.color = m_borderFlashColor;
			yield return new WaitForSeconds(0.75f);
			m_border.color = Color.white;
		}
	}
}

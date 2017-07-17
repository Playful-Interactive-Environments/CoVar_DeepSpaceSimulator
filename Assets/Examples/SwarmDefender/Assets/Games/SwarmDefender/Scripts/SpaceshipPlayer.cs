using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SwarmDefender
{
	public class SpaceshipPlayer : AGameChangerPlayer {

		public enum EFireMode
		{
			NONE,
			WEAK,
			MID,
			STRONG,
			Count
		}

//		public bool m_debug = false;
		public TriggerScript m_DockLeftTriggerScript;
		public TriggerScript m_DockRightTriggerScript;
		public Transform m_gunLeftPos;
		public Transform m_gunCenterPos;
		public GameObject m_bulletWeakPrefab;
		public GameObject m_bulletMidPrefab;
		public GameObject m_bulletStrongPrefab;
		public SpriteRenderer m_spaceshipSpriteRenderer;
		public SpaceshipLineController m_lineControllerLeft;
		public SpaceshipLineController m_lineControllerRight;
		public int SpriteIndex {
			get {
				return m_spriteIndex;
			}
			set {
				m_spriteIndex = value;
			}
		}
		public EFireMode FireMode {
			get {
				return m_fireMode;
			}
			set {
				m_fireMode = value;
//				ComputeNextShootTime();
			}
		}
		public bool LeftDockActive {
			get {
				return m_leftDockActive;
			}
		}
		public bool RightDockActive {
			get {
				return m_rightDockActive;
			}
		}

		private int m_spriteIndex;
		private float m_fireInverval;
		private float m_nextShootTime;
		private EFireMode m_fireMode;
		private bool m_leftDockActive = false;
		private bool m_rightDockActive = false;
		private GameObject m_rightPartnerDockGO;
		private GameObject m_leftPartnerDockGO;
		private bool m_stopNewDocking = false;

		private void Start()
		{
			m_fireMode = EFireMode.WEAK;
			m_DockRightTriggerScript.FireTriggerEnterEvent += ActivateRightDock;
			m_DockLeftTriggerScript.FireTriggerEnterEvent += ActivateLeftDock;
			m_DockRightTriggerScript.FireTriggerExitEvent += DeactivateRightDock;
			m_DockLeftTriggerScript.FireTriggerExitEvent += DeactivateLeftDock;

			InvokeRepeating("CheckDocks",1f,1f);
		}

		private void Update()
		{
			if(
				(m_fireMode != EFireMode.NONE) &&
				(Time.time > m_nextShootTime) &&
				!(GameManager.Instance as GameManager).IsInStartWaitState &&
				!(GameManager.Instance as GameManager).IsInGameWonState
				)
			{
				Shoot();
				ComputeNextShootTime();
			}
		}

		private void ComputeNextShootTime ()
		{
			m_fireInverval = (GameManager.Instance as GameManager).GetFireInvertal(FireMode);
			m_nextShootTime = Time.time + m_fireInverval + Random.Range(0,0.3f);
		}

		private void Shoot ()
		{
			Vector3 bulletSpawnPos;
			GameObject bulletPrefab;
			bool parentToMe = false;

			switch (m_fireMode) {
			case EFireMode.WEAK:
				bulletSpawnPos = m_gunCenterPos.position;
				bulletPrefab = m_bulletWeakPrefab;
				break;
			case EFireMode.MID:
				if(m_leftPartnerDockGO != null)
				{
					Vector3 spawnCenter = Vector3.Lerp(this.transform.position, m_leftPartnerDockGO.transform.parent.gameObject.transform.position, 0.5f);
					bulletSpawnPos = new Vector3(spawnCenter.x,m_gunLeftPos.position.y,0);
				}else
				{
					bulletSpawnPos = m_gunLeftPos.position;
				}
				bulletPrefab = m_bulletMidPrefab;
				break;
			case EFireMode.STRONG:
				bulletSpawnPos = m_gunCenterPos.position;
				bulletPrefab = m_bulletStrongPrefab;
				parentToMe = true;
				break;
			default:
				bulletSpawnPos = m_gunCenterPos.position;
				bulletPrefab = m_bulletWeakPrefab;
				Debug.LogWarning ("this should not happen!");
				break;
			}

			GameObject bullet = (GameObject)GameObject.Instantiate(bulletPrefab, bulletSpawnPos, Quaternion.identity);
			bullet.GetComponent<Bullet>().Owner = this;
			if(parentToMe) bullet.transform.parent = this.transform;

//			Debug.Log("Shoot: " + FireMode);
		}

		private void ActivateRightDock (GameObject theSenderGO)
		{
			if(!m_rightDockActive && 
			   !m_stopNewDocking && 
			   !theSenderGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer>().m_stopNewDocking)
			{ 
//				Debug.Log (this.gameObject.name + " ActivateRightDock");
				m_rightDockActive = true;
//				m_dockRightAnimator.SetBool("dockedRight",true);
				m_rightPartnerDockGO = theSenderGO;
				m_lineControllerLeft.StopDrawing();
				m_lineControllerRight.StopDrawing();
				if(!m_leftDockActive)
				{
					this.FireMode = EFireMode.NONE;
				}
				else
				{
					this.FireMode = EFireMode.STRONG;
					Invoke ("StrongDockingBehaviorRight",0.3f);
				}
			}
		}

		private void ActivateLeftDock (GameObject theSenderGO)
		{
			if(!m_leftDockActive &&
			   !m_stopNewDocking && 
			   !theSenderGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer>().m_stopNewDocking)
			{ 
//				Debug.Log (this.gameObject.name + " ActivateLeftDock");
				m_leftDockActive = true;
//				m_dockLeftAnimator.SetBool("dockedLeft",true);
				m_leftPartnerDockGO = theSenderGO;
				m_lineControllerLeft.StopDrawing();
				m_lineControllerRight.StopDrawing();
				if(!m_rightDockActive && !m_leftPartnerDockGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer>().LeftDockActive)
				{
					this.FireMode = EFireMode.MID;
					m_lineControllerLeft.DrawLines(m_DockLeftTriggerScript.transform,m_leftPartnerDockGO.transform, SpaceshipLineController.ELineType.MID);
				}
				else
				{
					this.FireMode = EFireMode.STRONG;
					Invoke ("StrongDockingBehaviorRight",0.3f);
				}
			}
		}

		private void DeactivateRightDock (GameObject theSenderGO)
		{
			if(m_rightDockActive && (theSenderGO == m_rightPartnerDockGO))
			{ 
				m_rightPartnerDockGO = null;
				m_stopNewDocking = false;
//				Debug.Log (this.gameObject.name + " DeactivateRightDock");
				m_rightDockActive = false;
//				m_dockRightAnimator.SetBool("dockedRight",false);
				m_lineControllerLeft.StopDrawing();
				m_lineControllerRight.StopDrawing();
				if(!m_leftDockActive)
				{
					this.FireMode = EFireMode.WEAK;
				}
				else
				{
					this.FireMode = EFireMode.MID;
					if(m_leftPartnerDockGO != null)
					{
						m_leftPartnerDockGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer>().m_stopNewDocking = false;
						m_lineControllerLeft.DrawLines(m_DockLeftTriggerScript.transform,m_leftPartnerDockGO.transform, SpaceshipLineController.ELineType.MID);
						
					}
				}
			}
		}
		
		private void DeactivateLeftDock (GameObject theSenderGO)
		{
			if(m_leftDockActive && (theSenderGO == m_leftPartnerDockGO))
			{ 
				GameObject oldLeftPartnerDockGo = m_leftPartnerDockGO;
				m_leftPartnerDockGO = null;
				m_stopNewDocking = false;
//				Debug.Log (this.gameObject.name + " DeactivateLeftDock");
				m_leftDockActive = false;
//				m_dockLeftAnimator.SetBool("dockedLeft",false);
				m_lineControllerLeft.StopDrawing();
				m_lineControllerRight.StopDrawing();
				if(!m_rightDockActive)
				{
					this.FireMode = EFireMode.WEAK;

					if(!oldLeftPartnerDockGo.transform.parent.gameObject.GetComponent<SpaceshipPlayer>().LeftDockActive)
					{
					}
				}
				else
				{
					this.FireMode = EFireMode.NONE;
					if(m_rightPartnerDockGO != null)
					{
						m_rightPartnerDockGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer>().FireMode = EFireMode.MID;
						m_rightPartnerDockGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer>().m_stopNewDocking = false;
						m_rightPartnerDockGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer>().m_lineControllerLeft.DrawLines(m_rightPartnerDockGO.transform, this.m_DockRightTriggerScript.transform, SpaceshipLineController.ELineType.MID);

					}
				}
			}
		}

		private void StrongDockingBehaviorRight ()
		{
			if(m_rightPartnerDockGO == null || m_leftPartnerDockGO == null) return;
			// TODO: partner's firemode has to be set to none, but it must be asured that it happens after the partner changes his firemode on his own
			m_rightPartnerDockGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer> ().FireMode = EFireMode.NONE;
			m_rightPartnerDockGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer> ().m_stopNewDocking = true;
			m_leftPartnerDockGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer> ().m_stopNewDocking = true;
			m_rightPartnerDockGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer> ().m_lineControllerLeft.StopDrawing();
			m_stopNewDocking = true;
			m_lineControllerLeft.DrawLines(m_DockLeftTriggerScript.transform,m_leftPartnerDockGO.transform, SpaceshipLineController.ELineType.STRONG);
			m_lineControllerRight.DrawLines(m_DockRightTriggerScript.transform,m_rightPartnerDockGO.transform, SpaceshipLineController.ELineType.STRONG);

		}

		private void CheckDocks()
		{
			if(!LeftDockActive && !RightDockActive)
			{
				m_stopNewDocking = false;
				FireMode = EFireMode.WEAK;
			}
		}

		private void OnDestroy()
		{
			NotifyDocksOfDestruction();
		}

		private void NotifyDocksOfDestruction ()
		{
			if(m_rightPartnerDockGO != null)
			{
				m_rightPartnerDockGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer> ().DeactivateLeftDock(m_DockRightTriggerScript.gameObject);
			}
			if(m_leftPartnerDockGO != null)
			{
				m_leftPartnerDockGO.transform.parent.gameObject.GetComponent<SpaceshipPlayer> ().DeactivateRightDock(m_DockLeftTriggerScript.gameObject);
			}

		}

	}
}

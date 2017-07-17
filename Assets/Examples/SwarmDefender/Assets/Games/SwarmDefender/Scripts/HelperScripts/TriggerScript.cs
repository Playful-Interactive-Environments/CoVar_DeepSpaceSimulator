using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SwarmDefender
{
	[RequireComponent(typeof(Collider2D))]
	public class TriggerScript : MonoBehaviour {

		//events
		public delegate void TriggerEvent(GameObject theGameObject);
		public event TriggerEvent FireTriggerEnterEvent;
		public event TriggerEvent FireTriggerExitEvent;

		// public members
		public bool m_isActive = true;
		public List<string> m_tagsToCheck;
		public List<string> m_layersToCheck;
		public Collider2D[] m_collidersToIgnore;

		private void OnTriggerEnter2D(Collider2D otherCol)
		{
			// is this collider in our ignore list?
			for (int i = 0; i < m_collidersToIgnore.Length; i++)
			{
				if(m_collidersToIgnore[i] == otherCol)	return;
			}

			foreach (string tag in m_tagsToCheck) {
				if(otherCol.gameObject.tag == tag){
					if(FireTriggerEnterEvent != null)
					{
						FireTriggerEnterEvent(otherCol.gameObject);
					}
					return;
				}
			}

			foreach (string layer in m_layersToCheck) {
				if (otherCol.gameObject.tag == layer) {
					if(FireTriggerEnterEvent != null)
					{
						FireTriggerEnterEvent(otherCol.gameObject);
					}
					return;
				}
			}
		}

		private void OnTriggerExit2D(Collider2D otherCol)
		{
			foreach (string tag in m_tagsToCheck) {
				if(otherCol.gameObject.tag == tag){
					if(FireTriggerExitEvent != null) 
					{
						FireTriggerExitEvent(otherCol.gameObject);
					}
					return;
				}
			}
			
			foreach (string layer in m_layersToCheck) {
				if (otherCol.gameObject.tag == layer) {
					if(FireTriggerExitEvent != null)
					{
						FireTriggerExitEvent(otherCol.gameObject);
					}
					return;
				}
			}
		}
	}
}

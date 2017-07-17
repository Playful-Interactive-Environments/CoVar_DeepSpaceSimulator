using UnityEngine;
using System.Collections;

namespace SwarmDefender
{
	public class DebugSpaceshipControls : MonoBehaviour 
	{

		public bool _active = false;
		public float _speed = 10;

		void Update()
		{
			if(!_active) return;

			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");

			Vector3 newPos = this.transform.position;
			newPos.x += _speed * horizontal * Time.deltaTime;
			newPos.y += _speed * vertical * Time.deltaTime;
			this.transform.position = newPos;
		}
	}
}
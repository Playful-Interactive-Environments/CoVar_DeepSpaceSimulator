using UnityEngine;
using System.Collections;

public class RotateScript : MonoBehaviour {

	[Range(-3,3)]
	public float m_rotateSpeed = 1.0f;
	public bool m_startAtRandomRotation = false;

	private void Start()
	{
		if(m_startAtRandomRotation) RotateOnceRandom();
	}

	private void Update()
	{
		RotateMe();
	}
	
	private void RotateMe ()
	{
		this.transform.localRotation = Quaternion.Euler(0,0,this.transform.localRotation.eulerAngles.z + (1*Time.deltaTime*m_rotateSpeed));
	}

	private void RotateOnceRandom ()
	{
		float rotation = Random.Range(0,360);
		this.transform.localRotation = Quaternion.Euler(0,0,this.transform.localRotation.eulerAngles.z + rotation);
	}
}

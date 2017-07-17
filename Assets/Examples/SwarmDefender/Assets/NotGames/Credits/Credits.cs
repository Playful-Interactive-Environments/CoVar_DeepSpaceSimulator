using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour 
{
	public GameObject m_creditsText;
	public float m_animTime;
	public float m_animateToLocalYPos = 1500f;
	public float m_quitTime;
	public bool m_autoQuit = false;

	private void Start()
	{
		Hashtable m_iTweenHash = new Hashtable();
		m_iTweenHash.Add("y", m_animateToLocalYPos);
		m_iTweenHash.Add("time", m_animTime);
		m_iTweenHash.Add ("islocal", true);
		m_iTweenHash.Add("easetype", iTween.EaseType.linear);

		iTween.MoveTo (m_creditsText, m_iTweenHash);

		if(m_autoQuit)
		{
			Invoke ("QuitApplication", m_animTime + m_quitTime);
		}
	}

	private void Update()
	{
		if(Input.GetButtonDown("Quit Game") || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
		{
			QuitApplication();
		}
	}

	private void QuitApplication()
	{
		Debug.Log ("--- Quit Application ---");
		Application.Quit();
	}
}

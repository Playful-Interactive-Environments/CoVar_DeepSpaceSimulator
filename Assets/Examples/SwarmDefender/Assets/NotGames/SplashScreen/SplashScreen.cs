using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SplashScreen : MonoBehaviour 
{
	[Range(0f,5f)]
	public float itsFadeInDuration;
	[Range(0f,10f)]
	public float itsStayDuration;
	[Range(0f,5f)]
	public float itsFadeOutDuration;
	[Range(0f,10f)]
	public float itsStayAfterFadeDuration;
	public MaskableGraphic itsSplashScreen;
	public Color itsFadeOutColor = Color.white;
	public Color itsFadedInColor = Color.white;
	public bool itsOnlyFadeAlpha = true;
	
	void Awake()
	{
		itsSplashScreen.color = itsFadeOutColor;
	}
	
	void Start()
	{
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
		StartCoroutine(SplashImage(itsFadeInDuration, itsStayDuration, itsFadeOutDuration, OnSplashScreenDone));
	}
	
	private IEnumerator SplashImage(float theFadeInDuration, float theStayDuration, float theFadeOutDuration, Action theCallback = null)
	{
		Color aFadeOutColor = itsFadeOutColor;
		Color aFadeInColor = itsFadedInColor;
		
		// fade in
		float aLerpProgress = 0;
		while(aLerpProgress < 1.0f)
		{
			aLerpProgress += (Time.deltaTime / theFadeInDuration);
			
			if(itsOnlyFadeAlpha)
			{
				Color color = aFadeOutColor;
				color.a = Color.Lerp(aFadeOutColor, aFadeInColor, aLerpProgress).a;
				itsSplashScreen.color = color;
			}
			else
			{
				itsSplashScreen.color = Color.Lerp(aFadeOutColor, aFadeInColor, aLerpProgress);
			}
			
			yield return 0;
		}
		
		yield return new WaitForSeconds(theStayDuration);
		
		
		// fade out
		aLerpProgress = 0;
		while(aLerpProgress < 1.0f)
		{
			aLerpProgress += (Time.deltaTime / theFadeOutDuration);
			
			if(itsOnlyFadeAlpha)
			{
				Color color = aFadeInColor;
				color.a = Color.Lerp(aFadeInColor, aFadeOutColor, aLerpProgress).a;
				itsSplashScreen.color = color;
			}
			else
			{
				itsSplashScreen.color = Color.Lerp(aFadeInColor, aFadeOutColor, aLerpProgress);
			}
			
			yield return 0;
		}

		yield return new WaitForSeconds(itsStayAfterFadeDuration);
		
		if(theCallback != null) theCallback();
	}
	
	private void OnSplashScreenDone()
	{
		Application.LoadLevel(Application.loadedLevel+1);
	}
}

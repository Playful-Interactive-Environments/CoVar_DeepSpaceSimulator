using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCamera : MonoBehaviour {

    static public Texture2D renderedTexture;
    public Material mat;

	void Awake () {
        renderedTexture = new Texture2D(Screen.width, Screen.height);
        mat.mainTexture = renderedTexture;
	}
	
	void OnPostRender () {
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        renderedTexture.Apply();
	}
}

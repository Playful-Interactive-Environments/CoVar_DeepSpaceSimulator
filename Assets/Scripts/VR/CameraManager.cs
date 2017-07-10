using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("displays connected: " + Display.displays.Length);

        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
/*        Display.displays[0].Activate();
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();*/
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

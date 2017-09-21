using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerMenu : MonoBehaviour {
    SteamVR_TrackedObject obj;
    public MenuScript menueHolder;
    public bool menuEnabled;

	void Start () {
        obj = GetComponent<SteamVR_TrackedObject>();
        menuEnabled = true;
        toggleMenu();
//        menuEnabled = false;
//        menueHolder.SetActive(menuEnabled);
	}
	
	void Update () {
        var device = SteamVR_Controller.Input((int)obj.index);
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            toggleMenu();
        }
	}

    public void toggleMenu()
    {
        menuEnabled = !menuEnabled;
        menueHolder.gameObject.SetActive(menuEnabled);
        GetComponent<VRUIInput>().enabled = menuEnabled;
        GetComponent<SteamVR_TrackedController>().enabled = menuEnabled;
        GetComponent<SteamVR_LaserPointer>().enabled = menuEnabled;
        GetComponent<LaserPointer>().enabled = !menuEnabled;
        GetComponent<ControllerGrabObject>().enabled = !menuEnabled;

        if (menuEnabled)
        {
            menueHolder.activeController = this;
        }
    }
}

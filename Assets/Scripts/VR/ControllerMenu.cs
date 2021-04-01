using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

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
        //TODO:Replace with new system
        //var device = SteamVR_Controller.Input((int)obj.index);
        //if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))//.ApplicationMenu))
        //{
        //    toggleMenu();
        //}
	}

    public void toggleMenu()
    {
        menuEnabled = !menuEnabled;
        menueHolder.gameObject.SetActive(menuEnabled);
        GetComponent<VRUIInput>().enabled = menuEnabled;
        //TODO:Replace GetComponent<SteamVR_TrackedController>().enabled = menuEnabled;
        GetComponent<SteamVR_LaserPointer>().enabled = menuEnabled;
        GetComponent<LaserPointer>().enabled = !menuEnabled;

        if (menuEnabled)
        {
            menueHolder.activeController = this;
        }
    }
}

using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

public class ControllerMenu : MonoBehaviour {
    SteamVR_TrackedObject obj;
    public MenuScript menueHolder;
    public bool menuEnabled;

    public SteamVR_Action_Boolean MenuOnOff;

    public SteamVR_Input_Sources HandType;

    void Start () {
        obj = GetComponent<SteamVR_TrackedObject>();
        menuEnabled = true;
        toggleMenu();
        this.MenuOnOff.AddOnStateDownListener(this.OnStateDown, HandType);
    }

    private void OnStateDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        this.toggleMenu();
        Debug.Log("Toggle Menu");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : AManager<GameManager>
{
    public Text Text1;
    public Text Text2;
    public Text RedPoints;
    public Text BluePoints;
    public Text GreenPoints;
    public Text YellowPoints;
    public GameObject Floor;
    public Camera MainCam;
    public GameObject TrackingManager;
    public GameObject GarbagePrefab;
    public float Mass;
    public float Damping;
    public float Stiftness;
    public float LineMax;
    public float LineLow;
    public float WidthMult;
    public float AddedForce;
    public float ForceHeight;
    public float RotateFactor;
    public int RedPoint;
    public int BluePoint;
    public int YellowPoint;
    public int GreenPoint;

    void Start () {
		MainCam = Camera.main;
    }
	
	void Update ()
	{
	    RedPoints.text = "" + RedPoint;
        BluePoints.text = "" + BluePoint;
        GreenPoints.text = "" + GreenPoint;
        YellowPoints.text = "" + YellowPoint;

    }
}

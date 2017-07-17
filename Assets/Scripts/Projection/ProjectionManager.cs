using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionManager : MonoBehaviour {

    public GameObject wallProjectionPlane;
    public GameObject floorProjectionPlane;
    public Material wallProjectionCamera;
    public Material floorProjectionCamera;
    public bool duplicateWallProjection;

	// Use this for initialization
	void Start () {
        wallProjectionPlane.GetComponent<Renderer>().material = wallProjectionCamera;

        if (!duplicateWallProjection)
            floorProjectionPlane.GetComponent<Renderer>().material = floorProjectionCamera;
        else
            floorProjectionPlane.GetComponent<Renderer>().material = wallProjectionCamera;
    }
}

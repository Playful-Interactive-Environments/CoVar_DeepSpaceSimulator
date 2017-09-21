using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MediaType
{
    None,
    SceneProjection,
    Video
}

public class ProjectionManager : MonoBehaviour {

    public GameObject wallProjectionPlane;
    public GameObject floorProjectionPlane;
    public Material wallProjectionCamera;
    public Material floorProjectionCamera;
    public bool duplicateWallProjection;

    public VideoController wallVideoPlane;
    public VideoController floorVideoPlane;

    public MediaType wallDisplayType;
    public MediaType floorDisplayType;

    // Use this for initialization
    void Start () {
        wallProjectionPlane.GetComponent<Renderer>().material = wallProjectionCamera;

        if (!duplicateWallProjection)
            floorProjectionPlane.GetComponent<Renderer>().material = floorProjectionCamera;
        else
            floorProjectionPlane.GetComponent<Renderer>().material = wallProjectionCamera;

        SyncDisplayTypes();
    }

    public void SyncDisplayTypes()
    {
        SyncDisplayType(wallDisplayType, wallProjectionPlane, wallVideoPlane);
        SyncDisplayType(floorDisplayType, floorProjectionPlane, floorVideoPlane);
    }

    private void SyncDisplayType(MediaType displayType, GameObject sceneProjection, VideoController video)
    {
        switch (displayType)
        {
            case MediaType.None:
                sceneProjection.SetActive(false);
                video.gameObject.SetActive(false);
                break;
            case MediaType.SceneProjection:
                video.gameObject.SetActive(false);
                sceneProjection.SetActive(true);
                break;
            case MediaType.Video:
                sceneProjection.SetActive(false);
                video.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityPharus;
using UnityTuio;
using PharusTransmission;

using UnityEngine.Video;

public enum ProjectionSpace
{
    none,
    wall,
    floor,
    both
}

public enum MenuItem
{
    None,
    Main,
    Projection,
    Lighting,
    Media,
    Scene,
    Video
}

public class MenuScript : MonoBehaviour {

    public ProjectionManager projectionManager;
    public GameObject MainMenuItems;
    public GameObject ProjectionMenuItems;
    public GameObject SceneMenuItems;
    public GameObject MediaTypeMenuItem;
    public GameObject VideoMenuItem;
    public GameObject LightingMenuItem;

    public VideoClip[] videoClips;

    public ControllerMenu activeController;

    public bool simulateTracking = false;
    //public UnityPharusManager pharus;
    //public UnityTuioManager tuio;

    public SimulatePharus headPos;

    public GameObject[] lightSetup;

    private ProjectionSpace projectionSpace;
    private MediaType mediaType;

    private Camera activeFloorProjectionCamera;
    private Camera activeWallProjectionCamera;

    private Scene activeFloorScene;
    private Scene activeWallScene;

    private TrackRecord trackRecord;

    void Start () {
    }

    void Update () {
    }

    private void setActiveMenuItem(MenuItem mi)
    {
        MainMenuItems.SetActive(mi == MenuItem.Main);
        ProjectionMenuItems.SetActive(mi == MenuItem.Projection);
        LightingMenuItem.SetActive(mi == MenuItem.Lighting);
        MediaTypeMenuItem.SetActive(mi == MenuItem.Media);
        SceneMenuItems.SetActive(mi == MenuItem.Scene);
        VideoMenuItem.SetActive(mi == MenuItem.Video);
    }

    private void OnEnable()
    {
        setActiveMenuItem(MenuItem.Main);
    }

    public void ShowProjectionMenu()
    {
        setActiveMenuItem(MenuItem.Projection);
    }

    public void SetProjectionSpace(ProjectionSpace projectionSpace)
    {
        this.projectionSpace = projectionSpace;
        setActiveMenuItem(MenuItem.Media);
    }

    public void SetProjectionSpaceWall()
    {
        SetProjectionSpace(ProjectionSpace.wall);
    }

    public void SetProjectionSpaceFloor()
    {
        SetProjectionSpace(ProjectionSpace.floor);
    }

    public void SetProjectionSpaceBoth()
    {
        SetProjectionSpace(ProjectionSpace.both);
    }

    public void SetMediaType(MediaType mediaType)
    {
        this.mediaType = mediaType;

        switch (mediaType)
        {
            case MediaType.None:
                setActiveMenuItem(MenuItem.None);
                break;
            case MediaType.SceneProjection:
                setActiveMenuItem(MenuItem.Scene);
                break;
            case MediaType.Video:
                setActiveMenuItem(MenuItem.Video);
                break;
            default:
                break;
        }
    }

    public void SetMediaTypeUnityScene()
    {
        SetMediaType(MediaType.SceneProjection);
    }

    public void SetMediaTypeVideo()
    {
        SetMediaType(MediaType.Video);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(AsyncLoadScene(sceneName));
    }

    IEnumerator AsyncLoadScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded)
        {
            //yield controll must be visibible until loading finished
            //load scene: scene must added to build window
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            scene = SceneManager.GetSceneByName(sceneName);
            foreach (var item in scene.GetRootGameObjects())
            {
                var cams = item.GetComponentsInChildren<Camera>(true);
                foreach (var cam in cams)
                {
                    cam.gameObject.SetActive(false);
                }
            }
        }

        UnloadUnusedScenes(projectionSpace);

        foreach (var item in scene.GetRootGameObjects())
        {
            var cams = item.GetComponentsInChildren<Camera>(true);
            foreach (var cam in cams)
            {
                if (!cam.targetTexture) continue;

                SetActiveProjectionCamera(projectionSpace, cam, scene);
            }
        }

        SyncDisplayType();
        ToggleMenu();
    }

    private void SetActiveProjectionCamera(ProjectionSpace projectionSpace, Camera cam, Scene scene)
    {
        switch (projectionSpace)
        {
            case ProjectionSpace.wall:
                if (cam.targetTexture.Equals(projectionManager.wallProjectionCamera.mainTexture))
                {
                    activeWallProjectionCamera = setActiveProjection(activeWallProjectionCamera, cam);
                    activeWallScene = scene;
                }
                break;
            case ProjectionSpace.floor:
                if (cam.targetTexture.Equals(projectionManager.floorProjectionCamera.mainTexture))
                {
                    activeFloorProjectionCamera = setActiveProjection(activeFloorProjectionCamera, cam);
                    activeFloorScene = scene;
                }
                break;
            case ProjectionSpace.both:
                SetActiveProjectionCamera(ProjectionSpace.wall, cam, scene);
                SetActiveProjectionCamera(ProjectionSpace.floor, cam, scene);
                break;
            default:
                break;
        }
    }

    private void UnloadUnusedScenes(ProjectionSpace projectionSpace)
    {
        switch (projectionSpace)
        {
            case ProjectionSpace.wall:
                if (activeWallScene.name != null && activeWallScene.name != activeFloorScene.name)
                {
                    SceneManager.UnloadSceneAsync(activeWallScene);
                }
                activeWallScene = new Scene();
                break;
            case ProjectionSpace.floor:
                if (activeFloorScene.name != null && activeWallScene.name != activeFloorScene.name)
                {
                        SceneManager.UnloadSceneAsync(activeFloorScene);
                }
                activeFloorScene = new Scene();
                break;
            case ProjectionSpace.both:
                UnloadUnusedScenes(ProjectionSpace.wall);
                UnloadUnusedScenes(ProjectionSpace.floor);
                break;
            default:
                break;
        }
    }

    private Camera setActiveProjection(Camera activeProjectionCamera, Camera newProjectionCamera)
    {
        if (activeProjectionCamera != null) activeProjectionCamera.gameObject.SetActive(false);
        newProjectionCamera.gameObject.SetActive(true);
        return newProjectionCamera;
    }

    public void ToggleMenu()
    {
        activeController.toggleMenu();
    }

    public void LoadVideo(int videoId)
    {
        LoadVideo(videoId, projectionSpace);
    }

    public void LoadVideo(int videoId, ProjectionSpace projectionSpace)
    {
        if (this.videoClips[videoId] == null)
        {
            return;
        }

        SyncDisplayType();
        UnloadUnusedScenes(projectionSpace);

        switch (projectionSpace)
        {
            case ProjectionSpace.none:
                break;
            case ProjectionSpace.wall:
                projectionManager.wallVideoPlane.SetVideoClip(this.videoClips[videoId]);
                projectionManager.wallVideoPlane.PlayVideo();
                break;
            case ProjectionSpace.floor:
                projectionManager.floorVideoPlane.SetVideoClip(this.videoClips[videoId]);
                projectionManager.floorVideoPlane.PlayVideo();
                break;
            case ProjectionSpace.both:
                LoadVideo(videoId, ProjectionSpace.wall);
                LoadVideo(videoId, ProjectionSpace.floor);
                break;
            default:
                break;
        }

        ToggleMenu();
    }

    public void LoadEmpty()
    {
        SetMediaType(MediaType.None);
        SyncDisplayType();
        UnloadUnusedScenes(projectionSpace);
        ToggleMenu();
    }

    private void SyncDisplayType()
    {
        switch (projectionSpace)
        {
            case ProjectionSpace.none:
                break;
            case ProjectionSpace.wall:
                projectionManager.wallDisplayType = mediaType;
                break;
            case ProjectionSpace.floor:
                projectionManager.floorDisplayType = mediaType;
                break;
            case ProjectionSpace.both:
                projectionManager.wallDisplayType = mediaType;
                projectionManager.floorDisplayType = mediaType;
                break;
            default:
                break;
        }
        projectionManager.SyncDisplayTypes();
    }

    public void ToggleSimulateTrackingPoint()
    {
        simulateTracking = !simulateTracking;
        headPos.enabled = simulateTracking;
        ToggleMenu();
    }

    public void ShowLightingMenu()
    {
        setActiveMenuItem(MenuItem.Lighting);
    }

    public void SetLight(int LightIndex)
    {
        for (int i = 0; i < lightSetup.Length; i++)
        {
            lightSetup[i].SetActive(i == LightIndex);
        }
        ToggleMenu();
    }
}

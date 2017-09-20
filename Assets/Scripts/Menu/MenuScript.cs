using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public enum ProjectionSpace
{
    none,
    wall,
    floor
}

public class MenuScript : MonoBehaviour {

    public ProjectionManager projectionManager;
    public GameObject MainMenuItems;
    public GameObject SceneMenuItems;

    public string[] sceneDir;

    public ControllerMenu activeController;
    private ProjectionSpace projectionSpace;

    private Camera activeFloorProjectionCamera;
    private Camera activeWallProjectionCamera;

    private AssetBundle myLoadedAssetBundle;
    private List<string> scenePaths;

    void Start () {
        scenePaths = new List<string>();

        foreach (var path in sceneDir)
        {
            string[] sp = System.IO.Directory.GetFiles(path);
            foreach (var s in sp)
            {
                if (s.EndsWith(".unity"))
                    scenePaths.Add(s.Replace("\\", "/"));
            }
        }
    }
	
	void Update () {
		
	}

    private void OnEnable()
    {
        MainMenuItems.SetActive(true);
        SceneMenuItems.SetActive(false);
    }

    public void SetProjectionSpace(ProjectionSpace projectionSpace)
    {
        this.projectionSpace = projectionSpace;
        MainMenuItems.SetActive(false);
        SceneMenuItems.SetActive(true);
    }

    public void SetProjectionSpaceWall()
    {
        SetProjectionSpace(ProjectionSpace.wall);
    }

    public void SetProjectionSpaceFloor()
    {
        SetProjectionSpace(ProjectionSpace.floor);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(AsyncLoadScene(sceneName));
        //string scenePath = scenePaths.First(s => s.EndsWith(sceneName+".unity"));

        //Scene scene = SceneManager.GetSceneByName(sceneName);
        //if (!scene.isLoaded)
        //{
        //    SceneManager.LoadScene(scenePath, LoadSceneMode.Additive);
        //    scene = SceneManager.GetSceneByName(sceneName);
        //    foreach (var item in scene.GetRootGameObjects())
        //    {
        //        var cams = item.GetComponentsInChildren<Camera>(true);
        //        foreach (var cam in cams)
        //        {
        //            Debug.LogWarning("SceneCam: " + cam);
        //            cam.gameObject.SetActive(false);
        //        }
        //    }
        //}

        //foreach (var item in scene.GetRootGameObjects())
        //{
        //    var cams = item.GetComponentsInChildren<Camera>(true);
        //    foreach (var cam in cams)
        //    {
        //        switch (projectionSpace)
        //        {
        //            case ProjectionSpace.wall:
        //                if (cam.targetTexture == projectionManager.wallProjectionCamera)
        //                    activeWallProjectionCamera = setActiveProjection(activeWallProjectionCamera, cam);
        //                break;
        //            case ProjectionSpace.floor:
        //                if (cam.targetTexture == projectionManager.floorProjectionCamera)
        //                    activeFloorProjectionCamera = setActiveProjection(activeFloorProjectionCamera, cam);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        ToggleMenu();
    }

    IEnumerator AsyncLoadScene(string sceneName)
    {
        string scenePath = scenePaths.First(s => s.EndsWith(sceneName + ".unity"));

        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded)
        {
            //DOTO Why Why Why? Verareitung scheint nach load zu stoppen!!!
            yield return SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            scene = SceneManager.GetSceneByName(sceneName);
            Debug.LogWarning("SceneCam: " + scene.isLoaded);
            Debug.LogWarning("SceneCam: " + scene.name);
            foreach (var item in scene.GetRootGameObjects())
            {
                var cams = item.GetComponentsInChildren<Camera>(true);
                Debug.LogWarning("SceneCam: " + cams.Length);
                foreach (var cam in cams)
                {
                    Debug.LogWarning("SceneCam: " + cam);
                    cam.gameObject.SetActive(false);
                }
            }
        }

        Debug.LogWarning("AsyncLoadScene Part2 start");

        foreach (var item in scene.GetRootGameObjects())
        {
            var cams = item.GetComponentsInChildren<Camera>(true);
            foreach (var cam in cams)
            {
                if (!cam.targetTexture) continue;

                switch (projectionSpace)
                {
                    case ProjectionSpace.wall:
                        if (cam.targetTexture.Equals(projectionManager.wallProjectionCamera.mainTexture))
                            activeWallProjectionCamera = setActiveProjection(activeWallProjectionCamera, cam);
                        break;
                    case ProjectionSpace.floor:
                        if (cam.targetTexture.Equals(projectionManager.floorProjectionCamera.mainTexture))
                            activeFloorProjectionCamera = setActiveProjection(activeFloorProjectionCamera, cam);
                        break;
                    default:
                        break;
                }
            }
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
}

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

    private Scene activeFloorScene;
    private Scene activeWallScene;

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
    }

    IEnumerator AsyncLoadScene(string sceneName)
    {
        string scenePath = scenePaths.First(s => s.EndsWith(sceneName + ".unity"));

        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded)
        {
            //yield controll must be visibible until loading finished
            //load scene: scene must added to build window
            yield return SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
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
                        {
                            activeWallProjectionCamera = setActiveProjection(activeWallProjectionCamera, cam);
                            if (activeWallScene.name != null && activeWallScene.name != activeFloorScene.name)
                            {
                                SceneManager.UnloadSceneAsync(activeWallScene);
                            }
                            activeWallScene = scene;
                        }
                        break;
                    case ProjectionSpace.floor:
                        if (cam.targetTexture.Equals(projectionManager.floorProjectionCamera.mainTexture))
                        {
                            activeFloorProjectionCamera = setActiveProjection(activeFloorProjectionCamera, cam);
                            if (activeFloorScene.name != null && activeWallScene.name != activeFloorScene.name)
                            {
                                SceneManager.UnloadSceneAsync(activeFloorScene);
                            }
                            activeFloorScene = scene;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        ToggleMenu();
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

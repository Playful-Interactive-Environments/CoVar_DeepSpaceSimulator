using UnityEngine;
using Valve.VR;

public class LaserPointer : MonoBehaviour
{
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    public Transform cameraRigTransform;
    public GameObject teleportReticlePrefab;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Transform headTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask;
    private bool shouldTeleport;

    public SteamVR_Action_Boolean ClickLaserOnOff;
    public SteamVR_Input_Sources HandType;


    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        if (teleportReticlePrefab)
        {
            reticle = Instantiate(teleportReticlePrefab);
            teleportReticleTransform = reticle.transform;
        }

        this.ClickLaserOnOff.AddOnUpdateListener(OnUpdated, HandType);
        this.ClickLaserOnOff.AddOnStateUpListener(this.OnStateUp, HandType);
    }

    private void OnUpdated(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        if (fromAction.state)
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                hitPoint = hit.point;
                ShowLaser(hit);
                if (reticle)
                {
                    reticle.SetActive(true);
                    teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                }
                shouldTeleport = true;
            }
        }
        else
        {
            laser.SetActive(false);
            if (reticle) reticle.SetActive(false);
        }
    }

    public void OnStateUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (fromAction.stateUp)
        {
            Teleport();
        }
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(this.transform.position, hitPoint, .5f);

        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    private void Teleport()
    {
        shouldTeleport = false;
        if (reticle) reticle.SetActive(false);
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        difference.y = 0;
        //hitPoint.y = cameraRigTransform.position.y;
        cameraRigTransform.position = hitPoint + difference;
    }
}
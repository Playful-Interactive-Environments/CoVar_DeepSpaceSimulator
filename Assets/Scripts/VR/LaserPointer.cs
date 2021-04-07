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
        this.laser = Instantiate(this.laserPrefab);
        this.laserTransform = this.laser.transform;
        if (this.teleportReticlePrefab)
        {
            this.reticle = Instantiate(this.teleportReticlePrefab);
            this.teleportReticleTransform = this.reticle.transform;
        }

        this.ClickLaserOnOff.AddOnUpdateListener(this.OnUpdated, this.HandType);
        this.ClickLaserOnOff.AddOnStateUpListener(this.OnStateUp, this.HandType);
    }

    private void OnUpdated(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        if (fromAction.state)
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 100, this.teleportMask))
            {
                this.hitPoint = hit.point;
                this.ShowLaser(hit);
                if (this.reticle)
                {
                    this.reticle.SetActive(true);
                    this.teleportReticleTransform.position = this.hitPoint + this.teleportReticleOffset;
                }
                this.shouldTeleport = true;
            }
        }
        else
        {
            this.laser.SetActive(false);
            if (this.reticle)
            {
                this.reticle.SetActive(false);
            }
        }
    }

    public void OnStateUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        this.Teleport();

        Debug.Log("Teleport");
    }

    private void ShowLaser(RaycastHit hit)
    {
        this.laser.SetActive(true);
        this.laserTransform.position = Vector3.Lerp(this.transform.position, this.hitPoint, .5f);

        this.laserTransform.LookAt(this.hitPoint);
        this.laserTransform.localScale = new Vector3(this.laserTransform.localScale.x, this.laserTransform.localScale.y,
            hit.distance);
    }

    private void Teleport()
    {
        this.shouldTeleport = false;
        if (this.reticle)
        {
            this.reticle.SetActive(false);
        }

        //Vector3 difference = this.cameraRigTransform.position - this.headTransform.position;
        //difference.y = 0;
        //hitPoint.y = cameraRigTransform.position.y;
        //this.cameraRigTransform.position = this.hitPoint + difference;
        this.cameraRigTransform.position = this.hitPoint;
    }
}
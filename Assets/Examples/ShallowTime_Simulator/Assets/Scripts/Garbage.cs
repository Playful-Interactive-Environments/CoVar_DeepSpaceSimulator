using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Garbage : MonoBehaviour
{
    public TestPlayer Parent;
    public float MoveSpeed = .1f;
    private float Radius = .1f;
    public bool released;
    private float stiftness = 1f;
    private float damping = .5f;
    private float mass = .5f;
    private Vector3 velocity;
    public Rigidbody rigidbody;
    public GameObject Particles;
    public GameObject[] GarbageRepr;
    private GameObject newGarbage;
    public int GarbageType;
    public Material[] Mats;
    private bool pointCounted;
    private float RotateFactor;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        int garbageRepr = GarbageType = Mathf.RoundToInt(Random.Range(0, GarbageRepr.Length));
        newGarbage = Instantiate(GarbageRepr[garbageRepr], transform.position, Quaternion.identity);
        newGarbage.transform.parent = transform;

        GarbageType = Mathf.RoundToInt(Random.Range(0, 3.5f));
        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
        {
            rend.material = Mats[GarbageType];
        }
    }

    void Update()
    {
        stiftness = GameManager.Instance.Stiftness;
        damping = GameManager.Instance.Damping;
        mass = GameManager.Instance.Mass;
        RotateFactor = GameManager.Instance.RotateFactor;
        if (!released && Parent != null)
        {
            Vector3 stretch = transform.position - Parent.transform.position;
            Vector3 force = -stiftness * stretch - damping * velocity;
            Vector3 acceleration = force / mass;
            velocity += acceleration * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
            newGarbage.transform.Rotate(Vector3.up, Time.deltaTime * (acceleration.x + acceleration.z) * RotateFactor);
            newGarbage.transform.Rotate(Vector3.forward, Time.deltaTime * (acceleration.x + acceleration.z) * RotateFactor);
            newGarbage.transform.Rotate(Vector3.right, Time.deltaTime * (acceleration.x + acceleration.z) * RotateFactor);

            //newGarbage.transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * 100f);
        }
        if (released && rigidbody.isKinematic)
        {
            transform.position += velocity * Time.deltaTime;
        }
    }

    public void Release()
    {
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        //rigidbody.AddForce(velocity * GameManager.Instance.AddedForce);
        rigidbody.AddRelativeTorque(velocity);
        rigidbody.AddRelativeForce(new Vector3(velocity.x, velocity.y + GameManager.Instance.ForceHeight, velocity.z) * GameManager.Instance.AddedForce);
        released = true;
        Parent.myGarbage = null;
        Parent = null;
        // Invoke("DestroyGarbage", 5f);
    }

    public void Remove()
    {
        released = true;
        Parent.myGarbage = null;
        Parent = null;
        Invoke("DestroyGarbage", 5f);
    }
    void DestroyGarbage()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("DropArea"))
        {
            Release();
        }

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("GarbageBin") && !pointCounted)
        {
            if (other.transform.name == "binBlue" && GarbageType == 0)
            {
                GameManager.Instance.BluePoint += 1;
                pointCounted = true;
                GameObject particles = Instantiate(Particles, transform.position, Quaternion.identity);
                StartCoroutine(DestroyParticles(particles));
            }
            if (other.transform.name == "binRed" && GarbageType == 1)
            {
                GameManager.Instance.RedPoint += 1;
                pointCounted = true;
                GameObject particles = Instantiate(Particles, transform.position, Quaternion.identity);
                StartCoroutine(DestroyParticles(particles));
            }
            if (other.transform.name == "binGreen" && GarbageType == 2)
            {
                GameManager.Instance.GreenPoint += 1;
                pointCounted = true;
                GameObject particles = Instantiate(Particles, transform.position, Quaternion.identity);
                StartCoroutine(DestroyParticles(particles));
            }
            if (other.transform.name == "binYellow" && GarbageType == 3)
            {
                GameManager.Instance.YellowPoint += 1;
                pointCounted = true;
                GameObject particles = Instantiate(Particles, transform.position, Quaternion.identity);
                StartCoroutine(DestroyParticles(particles));
            }
        }
    }

    IEnumerator DestroyParticles(GameObject obj)
    {
        yield return new WaitForSeconds(3f);
        Destroy(obj);
    }
}

/*
 CODE GRAVEYARD
     //private float restingLength = .5f;
    //private float springConstant = .1f;
    //private float dampingValue = .1f;
    //private float parentVelocity;

             //rotate around object
            //_angle += MoveSpeed * Time.deltaTime;
            //_centre = Parent.transform.position;
            //Vector3 offset = new Vector3(Mathf.Sin(_angle), 0, Mathf.Cos(_angle)) * Radius;
            //transform.position = _centre + offset;


                 //float displacedLength = Vector3.Distance(transform.position, Parent.transform.position) - restingLength;
            //Vector3 displacement = (transform.position - Parent.transform.position).normalized;
            //transform.position += -springConstant * displacedLength * displacement;
     */

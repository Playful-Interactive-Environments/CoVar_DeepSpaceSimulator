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
    public int GarbageType;
    public Material[] Mats;
    private bool pointCounted;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        GarbageType = Mathf.RoundToInt(Random.Range(0, 3.5f));
        if (GarbageType == 0)
            GetComponentInChildren<Renderer>().material = Mats[0];
        if (GarbageType == 1)
            GetComponentInChildren<Renderer>().material = Mats[1];
        if (GarbageType == 2)
            GetComponentInChildren<Renderer>().material = Mats[2];
        if (GarbageType == 3)
            GetComponentInChildren<Renderer>().material = Mats[3];
    }

    void Update()
    {
        stiftness = GameManager.Instance.Stiftness;
        damping = GameManager.Instance.Damping;
        mass = GameManager.Instance.Mass;
        if (!released && Parent != null)
        {
            Vector3 stretch = transform.position - Parent.transform.position;
            Vector3 force = -stiftness * stretch - damping * velocity;
            Vector3 acceleration = force / mass;
            velocity += acceleration * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
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
                Instantiate(Particles, transform.position, Quaternion.identity);
            }
            if (other.transform.name == "binRed" && GarbageType == 1)
            {
                GameManager.Instance.RedPoint += 1;
                pointCounted = true;
                Instantiate(Particles, transform.position, Quaternion.identity);
            }
            if (other.transform.name == "binGreen" && GarbageType == 2)
            {
                GameManager.Instance.GreenPoint += 1;
                pointCounted = true;
                Instantiate(Particles, transform.position, Quaternion.identity);
            }
            if (other.transform.name == "binYellow" && GarbageType == 3)
            {
                GameManager.Instance.YellowPoint += 1;
                pointCounted = true;
                Instantiate(Particles, transform.position, Quaternion.identity);
            }
        }
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

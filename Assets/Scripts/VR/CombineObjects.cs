using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineObjects : MonoBehaviour
{
    private class CombinedObject
    {
        public GameObject Object { get; set; }
        public Vector3 RelativParentPosition { get; set; }
        public GameObject Parent { get; set; }

        public CombinedObject(GameObject obj, GameObject parent)
        {
            Object = obj;
            Parent = parent;
            SetRelativPosition();
        }

        public void SetRelativPosition()
        {
            Vector3 difference = Parent.transform.position - Object.transform.position;
            RelativParentPosition = difference;
        }

        public override bool Equals(object obj)
        {
            GameObject go = obj as GameObject;
            if (go != null) return go.Equals(Object);
            CombinedObject co = obj as CombinedObject;
            if (co != null) return co.Object.Equals(Object);
            return false;
        }

        public override int GetHashCode()
        {
            return Object.GetHashCode();
        }
    }


    public Transform cameraRigTransform;
    private Vector3 cameraRigPos;
    private List<CombinedObject> CombinedObj;

    void Start()
    {
        CombinedObj = new List<CombinedObject>();
        cameraRigPos = cameraRigTransform.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickable") && other.gameObject.GetComponent<Rigidbody>())
        {
            //Debug.Log("Trigger Enter: " + CombinedObj.Count);
            var obj = HasConnection(other.gameObject);
            if (obj == null)
            {
                CombinedObj.Add(new CombinedObject(other.gameObject, gameObject));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("Trigger Exit: " + CombinedObj.Count);
        if (!CheckPositionChange())
        {
            var obj = HasConnection(other.gameObject);
            if (obj != null)
            {
                CombinedObj.Remove(obj);
            }
        }
    }

    private CombinedObject HasConnection(GameObject obj)
    {
        foreach (var item in CombinedObj)
        {
            if (item.Equals(obj)) return item;
        }

        return null;
    }

    /*
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Tag: " + other.tag);
        var link = HasConnection(other.gameObject);
        if (!link)
        {
            if (other.CompareTag("Pickable") && other.gameObject.GetComponent<Rigidbody>())
            {
                //Debug.Log("Obj: " + other.gameObject);
                var joint = AddFixedJoint();
                joint.connectedBody = other.gameObject.GetComponent<Rigidbody>();
            }
        }
    }
    

    void OnTriggerExit(Collider other)
    {
        var link = HasConnection(other.gameObject);
        if (link)
        {
            link.connectedBody = null;
            Destroy(link);
        }

        //var rb = other.gameObject.GetComponent<Rigidbody>();
        //if (other.CompareTag("Pickable") && rb)
        //{
        //    var links = GetComponents<FixedJoint>();
        //    foreach (var link in links)
        //    {
        //        if (link.connectedBody == rb)
        //        {
        //            link.connectedBody = null;
        //            Destroy(link);
        //            //rb.velocity = Controller.velocity;
        //            //rb.angularVelocity = Controller.angularVelocity;
        //        }
        //    }
        //}
    }
    */


    /*
    private FixedJoint HasConnection(GameObject obj)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (obj.CompareTag("Pickable") && rb)
        {
            var links = GetComponents<FixedJoint>();
            foreach (var link in links)
            {
                if (link.connectedBody == rb)
                {
                    return link;
                }
            }
        }

        return null;
    }
    

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
    */

    void EnableRigidbody(Rigidbody rb)
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
    }
    void DisableRigidbody(Rigidbody rb)
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;
    }

    private bool CheckPositionChange()
    {
        if (cameraRigTransform.transform.position != cameraRigPos)
        {
            //Debug.Log("Postion Changed: " + CombinedObj.Count);
            foreach (var item in CombinedObj)
            {
                Vector3 difference = gameObject.transform.position - item.RelativParentPosition;
                //Debug.Log("ObjPrev: " + item.Object.name + " = " + item.Object.transform.position + " = " + difference + " = " + item.RelativParentPosition + " = " + item.Parent.transform.position);
                DisableRigidbody(item.Object.GetComponent<Rigidbody>());
                item.Object.transform.position = difference;
                item.SetRelativPosition();
                //Debug.Log("ObjNew: " + item.Object.name + " = " + item.Object.transform.position + " = " + difference + " = " + item.RelativParentPosition + " = " + item.Parent.transform.position);
                EnableRigidbody(item.Object.GetComponent<Rigidbody>());
            }

            cameraRigPos = cameraRigTransform.transform.position;
            return true;
        }
        return false;
    }

    void Update()
    {
        if (CheckPositionChange())
        {
            //Debug.Log("Update");
        }
        else
        {
            foreach (var item in CombinedObj)
            {
                item.SetRelativPosition();
                //Debug.Log("Obj: " + item.Object.name + " = " + item.Object.transform.position);
            }
        }
    }
}

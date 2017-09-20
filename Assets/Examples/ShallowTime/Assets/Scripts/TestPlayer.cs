using System.Collections;
using UnityEngine;

public class TestPlayer : ATrackingEntity
{
    public Vector3[] positions;
    public float currentDistance;
    public float oldDistance;

    public Garbage myGarbage;
    private Vector3 movementDirection;
    public Vector3 PlayerDirection;
    public LineRenderer line;
    public float lineLength;
    public Material[] LineMats;
    public float lineMax;
    public float lineLow;
    public float widthMultiplier;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void OnDestroy()
    {
        if (myGarbage != null)
        {
            myGarbage.Remove();
            myGarbage = null;
        }
    }

    void Update()
    {
        lineMax = GameManager.Instance.LineMax;
        lineLow = GameManager.Instance.LineLow;
        widthMultiplier = GameManager.Instance.WidthMult;

        if (myGarbage != null)
        {
            StartCoroutine(speedFromMotion());
            line.enabled = true;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, myGarbage.transform.position);
            lineLength = Vector3.Distance(transform.position, myGarbage.transform.position);
            line.widthMultiplier = widthMultiplier / ( lineLength - widthMultiplier);
            if (line.widthMultiplier < 0)
                line.widthMultiplier = 0;
            if (line.widthMultiplier > widthMultiplier)
                line.widthMultiplier = widthMultiplier;
            //line.startWidth = lineLength / widthMultiplier;
            if (lineLength >= lineMax)
            {
                line.material = LineMats[0];

            }
            if (lineLength < lineMax && lineLength >= lineLow)
            {
                line.material = LineMats[1];
            }
            if (lineLength < lineLow)
            {
                line.material = LineMats[2];
            }
        }
        else
        {
            line.enabled = false;
        }
    }

    private float AngleTo(Vector2 pos, Vector2 target)
    {
        Vector2 diference = Vector2.zero;

        if (target.x > pos.x)
            diference = target - pos;
        else
            diference = pos - target;

        return Vector2.Angle(Vector2.right, diference);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("PickupArea") && myGarbage == null)
        {
            GameObject newGarbage = Instantiate(GameManager.Instance.GarbagePrefab, transform.position, Quaternion.identity);
            myGarbage = newGarbage.GetComponent<Garbage>();
            myGarbage.Parent = this;
        }
    }

    public override void SetPosition(Vector2 theNewPosition)
    {
        transform.position = new Vector3(theNewPosition.x, .5f, theNewPosition.y);
    }

    IEnumerator speedFromMotion()
    {
        oldDistance = currentDistance;
        Vector3 oldPos = transform.position;
        yield return new WaitForSeconds(.1f);
        PlayerDirection = (transform.position - oldPos).normalized;
        yield return new WaitForSeconds(.9f);
        currentDistance = Vector3.Distance(transform.position, oldPos);
    }
}




//float distanceCenter = Vector3.Distance(transform.position, new Vector3(0, transform.position.y, 0));
//float distanceZ = Vector3.Distance(transform.position, new Vector3(transform.position.x, transform.position.y, 0));
////float angle = AngleTo(new Vector2(transform.position.x, transform.position.z), new Vector2(0, 0));
//float angle = Vector2.Angle(Vector2.left, new Vector2(transform.position.x, transform.position.z) - new Vector2(0,0));
//GameManager.Instance.Text1.text = angle.ToString("F0") + " " + distanceCenter.ToString("F2") + " " + distanceZ.ToString("F2");
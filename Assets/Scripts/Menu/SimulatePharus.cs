using PharusTransmission;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPharus;
using UnityTuio;

public class SimulatePharus : MonoBehaviour {

    public UnityPharusManager pharus;
    public UnityTuioManager tuio;

    public Transform offsetPosition;

    private TrackRecord trackRecord;
    //private TUIO.TuioObject tuioObj;

    public float minX, maxX, minY, maxY;

    //private long si = -10;
    //private int sym = -10;

    void Start () {
	}

    private void OnEnable()
    {
        trackRecord = new TrackRecord();
        setTrackingPos();
        pharus.EventProcessor.PharusListener.EventQueue.Enqueue(new UnityPharusListener.PharusEvent(ETrackState.TS_NEW, trackRecord));
        //tuio.EventProcessor.TuioListener.EventQueue.Enqueue(new UnityTuioListener.TuioEvent(UnityTuioListener.ETuioEventType.ADD_OBJECT, tuioObj));

    }

    private void OnDisable()
    {
        pharus.EventProcessor.PharusListener.EventQueue.Enqueue(new UnityPharusListener.PharusEvent(ETrackState.TS_OFF, trackRecord));
        //tuio.EventProcessor.TuioListener.EventQueue.Enqueue(new UnityTuioListener.TuioEvent(UnityTuioListener.ETuioEventType.REMOVE_OBJECT, tuioObj));
    }

    private float normValue(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

    private void setTrackingPos()
    {
        float x = transform.position.x - offsetPosition.position.x;
        float y = transform.position.z - offsetPosition.position.z;

        y = -y;

        x = normValue(x, minX, maxX);
        y = normValue(y, minY, maxY);

        trackRecord.currentPos.x = x;
        trackRecord.currentPos.y = y;

        trackRecord.expectPos.x = x;
        trackRecord.expectPos.y = y;

        trackRecord.relPos.x = x;
        trackRecord.relPos.y = y;

        //tuioObj = new TUIO.TuioObject(si, sym, x, y, 0);
    }

    void Update () {
        setTrackingPos();

        pharus.EventProcessor.PharusListener.EventQueue.Enqueue(new UnityPharusListener.PharusEvent(ETrackState.TS_CONT, trackRecord));
        //tuio.EventProcessor.TuioListener.EventQueue.Enqueue(new UnityTuioListener.TuioEvent(UnityTuioListener.ETuioEventType.UPDATE_OBJECT, tuioObj));
    }
}

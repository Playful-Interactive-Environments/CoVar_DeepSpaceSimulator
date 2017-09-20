using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityTuio;
using UnityPharus;

abstract public class AGameChangerPlayerManager : AManager<AGameChangerPlayerManager> {

	protected List<AGameChangerPlayer> m_playerList;
	public GameObject m_playerPrefab;
	[SerializeField] private bool m_addUnknownPlayerOnUpdate = true;

	private bool TRACK_TUIO_CURSORS = true;
	private bool TRACK_TUIO_OBJECTS = false;
	private bool TRACK_TUIO_BLOBS = false;

	public List<AGameChangerPlayer> PlayerList
	{
		get { return m_playerList; }
	}

	protected override void Awake()
	{
		base.Awake();
		m_playerList = new List<AGameChangerPlayer>();
	}

	void OnEnable()
	{
		if(UnityTuioManager.Instance != null)
		{
			if (UnityTuioManager.Instance.EventProcessor == null)
			{
				UnityTuioManager.Instance.OnTrackingInitialized += SubscribeTuioTrackingEvents;
			}
			else
			{
				SubscribeTuioTrackingEvents(this, null);
			}
		}
		if (UnityPharusManager.Instance != null)
		{
			if (UnityPharusManager.Instance.EventProcessor == null)
			{
				UnityPharusManager.Instance.OnTrackingInitialized += SubscribePharusTrackingEvents;
			}
			else
			{
				SubscribePharusTrackingEvents(this, null);
			}
		}
	}
	
	void OnDisable()
	{
		if(UnityTuioManager.Instance != null && UnityTuioManager.Instance.EventProcessor != null)
		{
			if(TRACK_TUIO_CURSORS)
			{
				UnityTuioManager.Instance.EventProcessor.CursorAdded -= OnCursorAdded;
				UnityTuioManager.Instance.EventProcessor.CursorUpdated -= OnCursorUpdated;
				UnityTuioManager.Instance.EventProcessor.CursorRemoved -= OnCursorRemoved;
			}
			if(TRACK_TUIO_OBJECTS)
			{
				UnityTuioManager.Instance.EventProcessor.ObjectAdded -= OnObjectAdded;
				UnityTuioManager.Instance.EventProcessor.ObjectUpdated -= OnObjectUpdated;
				UnityTuioManager.Instance.EventProcessor.ObjectRemoved -= OnObjectRemoved;
			}
			if(TRACK_TUIO_BLOBS)
			{
				UnityTuioManager.Instance.EventProcessor.BlobAdded -= OnBlobAdded;
				UnityTuioManager.Instance.EventProcessor.BlobUpdated -= OnBlobUpdated;
				UnityTuioManager.Instance.EventProcessor.BlobRemoved -= OnBlobRemoved;
			}
		}
		if (UnityPharusManager.Instance != null && UnityPharusManager.Instance.EventProcessor != null)
		{
			UnityPharusManager.Instance.EventProcessor.TrackAdded -= OnTrackAdded;
			UnityPharusManager.Instance.EventProcessor.TrackUpdated -= OnTrackUpdated;
			UnityPharusManager.Instance.EventProcessor.TrackRemoved -= OnTrackRemoved;
		}
	}
	
	#region tuio event handlers
	void OnObjectAdded (object sender, UnityTuioEventProcessor.TuioEventObjectArgs e)
	{
		AddPlayer(e.tuioObject.SessionID, UnityTuioManager.GetScreenPositionFromRelativePosition(e.tuioObject.Position));
	}
	void OnCursorAdded (object sender, UnityTuioEventProcessor.TuioEventCursorArgs e)
	{
		AddPlayer(e.tuioCursor.SessionID, UnityTuioManager.GetScreenPositionFromRelativePosition(e.tuioCursor.Position));
	}
	void OnBlobAdded (object sender, UnityTuioEventProcessor.TuioEventBlobArgs e)
	{
		AddPlayer(e.tuioBlob.SessionID, UnityTuioManager.GetScreenPositionFromRelativePosition(e.tuioBlob.Position));
	}
	
	void OnObjectUpdated (object sender, UnityTuioEventProcessor.TuioEventObjectArgs e)
	{
		UpdatePlayerPosition(e.tuioObject.SessionID, UnityTuioManager.GetScreenPositionFromRelativePosition(e.tuioObject.Position));
	}
	void OnCursorUpdated (object sender, UnityTuioEventProcessor.TuioEventCursorArgs e)
	{
		UpdatePlayerPosition(e.tuioCursor.SessionID, UnityTuioManager.GetScreenPositionFromRelativePosition(e.tuioCursor.Position));
	}
	void OnBlobUpdated (object sender, UnityTuioEventProcessor.TuioEventBlobArgs e)
	{
		UpdatePlayerPosition(e.tuioBlob.SessionID, UnityTuioManager.GetScreenPositionFromRelativePosition(e.tuioBlob.Position));
	}
	
	void OnObjectRemoved (object sender, UnityTuioEventProcessor.TuioEventObjectArgs e)
	{
		RemovePlayer(e.tuioObject.SessionID);
	}
	void OnCursorRemoved (object sender, UnityTuioEventProcessor.TuioEventCursorArgs e)
	{
		RemovePlayer(e.tuioCursor.SessionID);
	}
	void OnBlobRemoved (object sender, UnityTuioEventProcessor.TuioEventBlobArgs e)
	{
		RemovePlayer(e.tuioBlob.SessionID);
	}
	#endregion

	#region pharus event handlers
	private void OnTrackAdded(object sender, UnityPharusEventProcessor.PharusEventTrackArgs e)
	{
		AddPlayer(e.trackRecord.trackID, UnityPharusManager.GetScreenPositionFromRelativePosition(e.trackRecord.relPos));
	}
	private void OnTrackUpdated(object sender, UnityPharusEventProcessor.PharusEventTrackArgs e)
	{
		UpdatePlayerPosition(e.trackRecord.trackID, UnityPharusManager.GetScreenPositionFromRelativePosition(e.trackRecord.relPos));
	}
	private void OnTrackRemoved(object sender, UnityPharusEventProcessor.PharusEventTrackArgs e)
	{
		RemovePlayer(e.trackRecord.trackID);
	}
	#endregion

	#region private methods
	private void SubscribePharusTrackingEvents(object theSender, System.EventArgs e)
	{
		UnityPharusManager.Instance.EventProcessor.TrackAdded += OnTrackAdded;
		UnityPharusManager.Instance.EventProcessor.TrackUpdated += OnTrackUpdated;
		UnityPharusManager.Instance.EventProcessor.TrackRemoved += OnTrackRemoved;
	}

	private void SubscribeTuioTrackingEvents (object theSender, System.EventArgs e)
	{
		if(TRACK_TUIO_CURSORS)
		{
			UnityTuioManager.Instance.EventProcessor.CursorAdded += OnCursorAdded;
			UnityTuioManager.Instance.EventProcessor.CursorUpdated += OnCursorUpdated;
			UnityTuioManager.Instance.EventProcessor.CursorRemoved += OnCursorRemoved;
		}
		if(TRACK_TUIO_OBJECTS)
		{
			UnityTuioManager.Instance.EventProcessor.ObjectAdded += OnObjectAdded;
			UnityTuioManager.Instance.EventProcessor.ObjectUpdated += OnObjectUpdated;
			UnityTuioManager.Instance.EventProcessor.ObjectRemoved += OnObjectRemoved;
		}
		if(TRACK_TUIO_BLOBS)
		{
			UnityTuioManager.Instance.EventProcessor.BlobAdded += OnBlobAdded;
			UnityTuioManager.Instance.EventProcessor.BlobUpdated += OnBlobUpdated;
			UnityTuioManager.Instance.EventProcessor.BlobRemoved += OnBlobRemoved;
		}
	}
	#endregion

	#region player management
	public virtual void AddPlayer (long trackID, Vector2 position)
	{
		AGameChangerPlayer player = (GameObject.Instantiate(m_playerPrefab, new Vector3(position.x,position.y,0), Quaternion.identity) as GameObject).GetComponent<AGameChangerPlayer>();
		player.TrackID = trackID;
		m_playerList.Add(player);
	}

	public virtual void UpdatePlayerPosition (long trackID, Vector2 position)
	{
		foreach (AGameChangerPlayer player in m_playerList) 
		{
			if(player.TrackID.Equals(trackID))
			{
				player.SetPosition(position);
				return;
			}
		}
		
		if(m_addUnknownPlayerOnUpdate)
		{
			AddPlayer(trackID, position);
		}
	}

	public virtual void RemovePlayer (long trackID)
	{
		foreach (AGameChangerPlayer player in m_playerList.ToArray()) 
		{
			if(player.TrackID.Equals(trackID))
			{
				GameObject.Destroy(player.gameObject);
				m_playerList.Remove(player);
			}	
		}
	}
	#endregion
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Derive your player scripts from this.
/// Each GameChangerPlayer has a SessionID which relates to a TuioContainer / TrackLink entity
/// </summary>
public abstract class AGameChangerPlayer : MonoBehaviour
{
	private long _trackID;

	#region properties
	/// <summary>
	/// TrackID corresponds to the entityId of the tracking service (TUIO sessionID / Pharus trackID).
	/// </summary>
	public long TrackID 
	{
		get { return _trackID; }
		set { _trackID = value; }
	}
	#endregion

	#region public methods
	public virtual void SetPosition (Vector2 theNewPosition)
	{
		this.transform.position = theNewPosition;
	}
	#endregion

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAnimEventReceiver : MonoBehaviour
{
	#region constants
	
	#endregion
	
	#region vars

	private PusherEntity mParentPusher;

	#endregion
	
	#region properties
	
	#endregion
	
	#region init

	protected void Awake()
	{
		Transform t = transform;
		while (mParentPusher == null && t.parent != null)
		{
			t = t.parent;
			mParentPusher = t.GetComponent<PusherEntity>();
		}
	}
	
	#endregion

	#region update
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods
	
	public void OnAnimEvent_Push()
	{
		mParentPusher.OnAnimEventPush();
	}

	public void OnAnimEvent_PushEnd()
	{
		mParentPusher.OnAnimEventPushEnded();
	}

	#endregion
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawGrid : MonoBehaviour
{
	#region constants
	
	#endregion
	
	#region vars
	
	#endregion
	
	#region properties
	
	#endregion
	
	#region init

	protected virtual void Awake()
	{
	
	}
	
	// Use this for initialization
	protected virtual void Start()
	{
	
	}
	
	#endregion

	#region update
	
	// Update is called once per frame
	protected virtual void Update()
	{
	
	}
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods

	public void OnDrawGizmos()
	{
		int nLines = 100;
		Gizmos.color = Color.white;
		for (int i = -nLines; i <= nLines; ++i)
		{
			Gizmos.DrawLine(new Vector3(64 * i, 64 * -nLines, 0f), new Vector3(64 * i, 64 * nLines, 0f));
		}

		for (int j = -nLines; j <= nLines; ++j)
		{
			Gizmos.DrawLine(new Vector3(64 * -nLines, 64 * j, 0f), new Vector3(64 * nLines, 64 * j, 0f));
		}
	}
	
	#endregion
}

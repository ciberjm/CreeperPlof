using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public abstract class FastTestEditor : MonoBehaviour
{
	#region constants
	
	#endregion
	
	#region vars

	public bool execute = false;

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
		if (execute)
		{
			execute = false;
			Execute();
		}
	}

	#endregion

	#region protected methods

	protected abstract void Execute();

	#endregion

	#region public methods

	#endregion
}

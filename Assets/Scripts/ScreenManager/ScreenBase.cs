using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ScreenBase : MonoBehaviour
{
	#region constants

	#endregion
	
	#region vars

	public abstract ScreenID ID { get; }

	public float TimeOpened { get; set; }

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

	public virtual void OnOpen()
	{
		TimeOpened = 0f;
	}

	public virtual void OnClose()
	{

	}

	public virtual void OnExit()
	{

	}

	#endregion

	#region update
	
	// Update is called once per frame
	protected virtual void Update()
	{
		TimeOpened += Time.deltaTime;
	}
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods
	
	#endregion
}

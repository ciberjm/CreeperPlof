using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TestCollisions2D : MonoBehaviour
{
	#region constants
	
	#endregion
	
	#region vars

	public float move = 0f;

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
		transform.Translate(Vector3.down * move);
	}
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods
	
	protected void OnTriggerEnter2D(Collider2D c)
	{
		Debug.Log("Colliding with: " + c.name);
	}

	#endregion
}

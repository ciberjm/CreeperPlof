using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreObj : MonoBehaviour
{
	#region constants
	
	#endregion
	
	#region vars

	public float Duration = 5f;
	public int Score = 1000;

	private float mTimer = 0f;

	#endregion
	
	#region properties
	
	#endregion
	
	#region init
	
	// Use this for initialization
	protected virtual void Start()
	{
		GetComponentInChildren<JUIText>().Text = Score.ToString();
		mTimer = Duration;
	}
	
	#endregion

	#region update

	protected virtual void Update()
	{
		mTimer -= Time.deltaTime;
		if (mTimer <= 0)
		{
			Destroy(this.gameObject);
		}
	}

	#endregion
	
	#region protected methods

	public void OnDestroy()
	{
		ScorePopupsManager.Inst.NotifyDestroy(this);
	}
	
	#endregion
	
	#region public methods

	#endregion
}

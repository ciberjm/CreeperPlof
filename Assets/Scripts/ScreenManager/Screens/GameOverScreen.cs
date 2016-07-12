using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameOverScreen : ScreenBase
{
	#region constants
	
	#endregion
	
	#region vars

	public JUIText TotalScoreText;

	public float Duration = 5f;

	#endregion
	
	#region properties

	public override ScreenID ID
	{
		get { return ScreenID.GameOver; }
	}

	#endregion
	
	#region init

	protected override void Awake()
	{
		base.Awake();
	}
	
	// Use this for initialization
	protected override void Start()
	{
		base.Start();
	}

	public override void OnOpen()
	{
		base.OnOpen();

		TotalScoreText.Text = "TOTAL SCORE : " + GameSessionManager.Inst.Players[0].Data.Score.ToString("00000000");
	}

	#endregion

	#region update
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		if (TimeOpened > Duration)
		{
			World.Inst.ExitGame();
		}
	}
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods
	
	#endregion
}

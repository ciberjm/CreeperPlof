using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelCompletedScreen : ScreenBase
{
	#region constants
	
	#endregion
	
	#region vars

	public JUIText EnemiesKilledText;
	public JUIText EnemiesEscapedText;
	public JUIText EnemiesTrappedText;
	public JUIText TimeText;
	public JUIText TotalScoreText;

	public float Duration = 5f;

	#endregion
	
	#region properties

	public override ScreenID ID
	{
		get { return ScreenID.LevelCompleted; }
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

		EnemiesKilledText.Text	= "ENEMIES KILLED : " + GameSessionManager.Inst.EnemiesKilled.ToString("00");
		EnemiesEscapedText.Text	= "ENEMIES ESCAPED: " + GameSessionManager.Inst.EnemiesEscaped.ToString("00");
		EnemiesTrappedText.Text	= "ENEMIES TRAPPED: " + GameSessionManager.Inst.EnemiesTrapped.ToString("00");
		TimeText.Text			= "TIME:        " + Mathf.FloorToInt(GameSessionManager.Inst.TimeInLevel / 60).ToString("000") + ":" + (GameSessionManager.Inst.TimeInLevel % 60).ToString("00");
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
			World.Inst.LoadNextLevel();
		}
	}
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods
	
	#endregion
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScorePopupsManager : MonoBehaviour
{
	#region constants

	public class ScorePopupInfo
	{
		public Vector3 Pos;
		public float Delay;
		public int Score;
	}

	#endregion
	
	#region vars

	public ScoreObj ScoreObject;

	public List<ScoreObj> SPopups;

	public List<ScorePopupInfo> SPopupsToSpawn;

	private Transform mParentObj;

	#endregion
	
	#region properties

	public static ScorePopupsManager Inst { get; private set; }

	#endregion
	
	#region init

	protected virtual void Awake()
	{
		Inst = this;
		SPopups = new List<ScoreObj>();
		SPopupsToSpawn = new List<ScorePopupInfo>();

		mParentObj = transform.GetChildForced("Popups");
	}
	
	// Use this for initialization
	protected virtual void Start()
	{
	
	}
	
	#endregion

	#region update

	protected virtual void Update()
	{
		if (World.Inst != null)
		{
			if (World.Inst.State != World.GameState.Paused)
			{
				for (int i = 0; i < SPopupsToSpawn.Count; ++i)
				{
					var sp = SPopupsToSpawn[i];
					sp.Delay -= Time.deltaTime;
					if (sp.Delay <= 0)
					{
						SpawnPopupObj(sp.Score, sp.Pos);
						SPopupsToSpawn.RemoveAt(i);
						--i;
					}
				}
			}
		}
	}

	#endregion
	
	#region protected methods

	private void SpawnPopupObj(int _score, Vector3 _pos)
	{
		ScoreObj so = (ScoreObj)Instantiate(ScoreObject, _pos, Quaternion.identity);
		so.transform.parent = mParentObj;
		so.transform.position = _pos;
		so.Score = _score;

		SPopups.Add(so);
	}

	#endregion
	
	#region public methods

	public void SpawnPopup(int _score, Vector3 _pos, float _delay)
	{
		ScorePopupInfo so = new ScorePopupInfo() { Score = _score, Pos = _pos, Delay = _delay };
		SPopupsToSpawn.Add(so);
	}

	public void ClearAllPopups()
	{
		SPopupsToSpawn.Clear();
		mParentObj.DestroyAllChilds();
		SPopups.Clear();
	}

	public void NotifyDestroy(ScoreObj so)
	{
		SPopups.Remove(so);
	}

	#endregion

	//void OnGUI()
	//{
	//	if (GUILayout.Button("Show random score popup"))
	//	{
	//		SpawnPopup(1234567890, Vector3.zero, 0f);
	//	}
	//}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ParticleSpawnerSystem : MonoBehaviour
{
	#region constants

	public enum ParticleType
	{
		None,
		BlockDestroyed,
		EnemySpawn,
		EnemySmashed,
		EnemySmashedByOutsider,
	}

	#endregion

	#region vars

	public GameObject[] Prefabs;

	public Dictionary<ParticleType, GameObject> PrefabsDict;

	#endregion

	#region properties

	public static ParticleSpawnerSystem Inst { get; private set; }

	#endregion

	#region init

	protected virtual void Awake()
	{
		Inst = this;
		RebuildFactory();
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
		if (!Application.isPlaying)
		{
			Inst = this;
			if (PrefabsDict == null || Prefabs == null || Prefabs.Length != PrefabsDict.Count)
			{
				RebuildFactory();
			}
		}
	}

	#endregion

	#region protected methods

	protected void RebuildFactory()
	{
		PrefabsDict = new Dictionary<ParticleType, GameObject>();
		foreach (var v in Prefabs)
		{
			PrefabsDict[v.GetComponent<ParticleObj>().PartType] = v;
		}
	}

	#endregion

	#region public methods

	public static ParticleObj SpawnParticle(ParticleType _type, Vector3 _pos)
	{
		GameObject prefab = Inst.PrefabsDict[_type];

		GameObject go = (GameObject)Instantiate(prefab, _pos, Quaternion.identity);
		return go.GetComponent<ParticleObj>();
	}

	#endregion
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleObj : MonoBehaviour
{
	#region constants
	
	#endregion
	
	#region vars

	public float Duration = 1.5f;

	public ParticleSpawnerSystem.ParticleType PartType;

	#endregion
	
	#region properties
	
	#endregion
	
	#region init

	// Use this for initialization
	protected virtual IEnumerator Start()
	{
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "Particles";
		if (Duration >= 0)
		{
			yield return new WaitForSeconds(Duration);
			Destroy(gameObject);
		}
		yield return 0;
	}
	
	#endregion

	#region update
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods

	#endregion
}

using UnityEngine;

using System.Collections;

[ExecuteInEditMode]
public class PixelPerfect : MonoBehaviour
{
	public float textureSize = 100f;

	public bool update = false;

	void Update()
	{
		float unitsPerPixel = 1f / textureSize;

		Camera.main.orthographicSize = (Screen.height / 2f) * unitsPerPixel;

		if (Application.isPlaying)
			Destroy(this);
	}
}
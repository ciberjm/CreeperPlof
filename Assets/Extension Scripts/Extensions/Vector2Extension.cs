using UnityEngine;
using System.Collections.Generic;

public static class Vector2Extension
{
	public static string Dump(this Vector2 _vector)
	{
		return ("(" + _vector.x.ToString("0.0#######") + ", " + _vector.y.ToString("0.0#######") + ")");
	}

	public static Vector3 ToVector3_XY(this Vector2 v)
	{
		return new Vector3(v.x, v.y, 0f);
	}

	public static Vector3 ToVector3_XZ(this Vector2 v)
	{
		return new Vector3(v.x, 0f, v.y);
	}

	public static Vector3 ToVector3_YZ(this Vector2 v)
	{
		return new Vector3(0f, v.x, v.y);
	}

	public static Vector2 CoordinateProduct(this Vector2 v, Vector2 w)
	{
		return new Vector2(v.x * w.x, v.y * w.y);
	}
}
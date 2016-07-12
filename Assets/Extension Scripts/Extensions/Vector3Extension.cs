using UnityEngine;
using System.Collections.Generic;

public static class Vector3Extension
{
	public static string Dump(this Vector3 _vector)
	{
		return ("(" + _vector.x.ToString("0.0#######") + ", " + _vector.y.ToString("0.0#######") + ", " + _vector.z.ToString("0.0#######") + ")");
	}

	public static Vector2 ToVector2_XY(this Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector2 ToVector2_XZ(this Vector3 v)
	{
		return new Vector2(v.x, v.z);
	}

	public static Vector2 ToVector2_YZ(this Vector3 v)
	{
		return new Vector2(v.y, v.z);
	}

	public static Vector3 CoordinateProduct(this Vector3 v, Vector3 w)
	{
		return new Vector3(v.x * w.x, v.y * w.y, v.z * w.z);
	}

	public static Vector3 CoordinateDivide(this Vector3 v, Vector3 w)
	{
		return new Vector3(v.x / w.x, v.y / w.y, v.z / w.z);
	}
}
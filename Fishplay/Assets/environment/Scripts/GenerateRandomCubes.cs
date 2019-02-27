using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// random cube generator
/// for the convenience of creating spacial variant objects
/// </summary>
public class GenerateRandomCubes : MonoBehaviour
{
	public int Count = 50;
	public int RangeRadius = 100;
	public bool ColliderOn = false;
	public float ScaleMax = 20.0f;

	void Start ()
	{
		int count = Count;
		while (count > 0)
		{
			GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
			g.transform.parent = transform;
			g.transform.rotation = Quaternion.Euler(RandomDegree(), RandomDegree(), RandomDegree());
			g.transform.localScale = Random.value*Vector3.one * ScaleMax;
			g.transform.position = RandomRange(-RangeRadius, RangeRadius);
			g.GetComponent<BoxCollider>().enabled = ColliderOn;
			g.layer = LayerMask.NameToLayer(Rayhit.LAYER_CAN_RAY_HIT);
			count--;
		}
	}

	float RandomDegree()
	{
		return 360.0f*Random.value;
	}

	Vector3 RandomEulerAngle()
	{
		return new Vector3(Random.value, Random.value, Random.value);
	}

	Vector3 RandomRange(float min, float max)
	{
		return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
	}
}

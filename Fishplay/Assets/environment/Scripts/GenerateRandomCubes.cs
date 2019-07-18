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

	public bool CargoLootTest = false;
	public GameObject CargoPrefab;

	void Start ()
	{
		int count = Count;
		while (count > 0)
		{
			GameObject g = SetupInstance(GameObject.CreatePrimitive(PrimitiveType.Cube),transform, true);
			g.GetComponent<Collider>().enabled = ColliderOn;
			g.layer = LayerMask.NameToLayer(Rayhit.LAYER_CAN_RAY_HIT);

			if(CargoLootTest)
				SetupInstance(GameObject.Instantiate(CargoPrefab), null, false);
			count--;
		}
	}

	GameObject SetupInstance(GameObject g, Transform parent_t, bool setScale)
	{
		if(parent_t != null)
			g.transform.parent = parent_t;
		g.transform.rotation = Quaternion.Euler(RandomDegree(), RandomDegree(), RandomDegree());
		if(setScale)
			g.transform.localScale = Random.value*Vector3.one * ScaleMax;
		g.transform.position = RandomRange(-RangeRadius, RangeRadius);
		return g;
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

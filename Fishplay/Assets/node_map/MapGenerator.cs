using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 用于生成一个有主体路径方向的随机星图
/// 类似FTL的那种结构，不过分支更可控
/// TODO: 预计可能用于不同星系的大地图用。配合星系内图（九宫格生成）来形成整体结构
/// </summary>

public class MapGenerator : MonoBehaviour
{

	[SerializeField] private MapData _mData;

	// Use this for initialization
	void Start () {
		
	}

	[ContextMenu("Generate")]
	void Generate()
	{
		if(_mData == null)
			_mData = new MapData();

		_mData.Generate(transform);
	}

	void OnDrawGizmos()
	{
		if (_mData == null)
			return;
		Gizmos.color = Color.magenta;
		_mData.Draw();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[Serializable]
public class MapData
{
	public int LevelCount = 6;
	public int BranchMax = 2;

	public float StepDistance = 3.0f;
	public float StepDistanceOffset = 0.5f;

	private List<List<GameObject>> Nodes = new List<List<GameObject>>();

	public void  Generate(Transform parent)
	{
		Nodes.Clear();
		while (parent.childCount > 0)
		{
			GameObject.DestroyImmediate(parent.GetChild(0).gameObject);
		}

		Vector3 lastCenter = Vector3.zero;
		for (int i = 0; i < LevelCount; i++)
		{
			List<GameObject> n = new List<GameObject>();

			int branch = i== 0 || i == LevelCount - 1 ? 1 : Random.Range(1, BranchMax + 1);
			float degreeBase = 0;
			Vector3 pFoo = Vector3.zero;
			for (int j = 0; j < branch; j++)
			{
				float degreeDelta = 180.0f/(branch + 1);
				degreeBase = degreeDelta * j;
				Vector3 p = lastCenter + RandomVectorOnlyPositive(degreeDelta, degreeBase).normalized*
				            StepDistance;
				p.y = Random.Range(-StepDistance, StepDistance);
				GameObject g = new GameObject(string.Format("{0}-{1}", i,j));
				g.transform.parent = parent;
				g.transform.position = p;
				n.Add(g);

				if (j == 0)
					pFoo = p;
				else if (Random.value > 0.5f)
					pFoo = p;
			}

			lastCenter = pFoo;

			Debug.DrawRay(lastCenter, Vector3.up, Color.blue, 10.0f);

			Nodes.Add(n);
		}
	}

	public void Draw()
	{
		if (Nodes == null || Nodes.Count == 0)
			return;
		Gizmos.DrawSphere(Vector3.zero, 0.2f);
		Gizmos.DrawLine(Vector3.zero, Nodes[0][0].transform.position);
		for (int i = 0; i < Nodes.Count; i++)
		{
			for (int j = 0; j < Nodes[i].Count; j++)
			{
				Vector3 v = Nodes[i][j].transform.position;
				if (i > 0)
				{
					foreach (GameObject n in Nodes[i-1])
					{
						Gizmos.DrawLine(n.transform.position, v);
					}
				}
				Gizmos.DrawWireSphere(v, 0.1f);
			}
		}
	}

	Vector3 RandomVectorOnlyPositive(float degreeOffset, float baseVal)
	{
		Vector3 v = new Vector3(0, 0, 1);
		v = Quaternion.Euler(0, Random.Range(baseVal,  baseVal + degreeOffset), 0)*v;

		return v;
	}
}

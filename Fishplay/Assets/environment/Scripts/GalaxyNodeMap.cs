using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyNodeMap : MonoBehaviour
{
	public SpaceNode[] AllNodes;
	public int NodeCount = 8;
	public float Radius = 500;

	private GameObject[] _indicators;

	private Transform _playerT;

	// Use this for initialization
	void Start () {
		AllNodes = new SpaceNode[NodeCount];
		_indicators = new GameObject[NodeCount];

		_playerT = GameObject.FindGameObjectWithTag("Player").transform;

		for (int i = 0; i < NodeCount; i++)
		{
			AllNodes[i] = new SpaceNode(RandomVector(Radius));
			_indicators[i] = CreateIndicator(AllNodes[i]);
		}

	}

	GameObject CreateIndicator(SpaceNode node)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		go.name = "galaxyIndicators";
		go.tag = "target";
		RefreshNodePos(node, go);
		return go;
	}

	void RefreshNodePos(SpaceNode node, GameObject go)
	{
		go.transform.position = _playerT.position + node.Direction()*500;
	}

	Vector3 RandomVector(float radius)
	{
		return new Vector3(Random.Range(-radius, radius)
			, Random.Range(-radius, radius)
			, Random.Range(-radius, radius));
	}

	void OnDrawGizmos()
	{
		if (_playerT == null)
			return;

		if(_indicators == null || _indicators.Length == 0)
			return;

		for (int i = 0; i < NodeCount; i++)
		{
			Gizmos.DrawLine(_playerT.position, _indicators[i].transform.position);
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < NodeCount; i++)
		{
			RefreshNodePos(AllNodes[i], _indicators[i]);
		}
	}
}

[SerializeField]
public class SpaceNode
{
	public Vector3 SpaceVector;

	public SpaceNode(Vector3 p)
	{
		SpaceVector = p;
	}

	public Vector3 Direction()
	{
		return SpaceVector.normalized;
	}
}

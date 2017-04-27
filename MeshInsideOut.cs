using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 用于内外翻转模型
/// </summary>

[RequireComponent(typeof(MeshFilter))]
public class MeshInsideOut : MonoBehaviour
{
	private Mesh _mesh;
	
	[ContextMenu("Inverse")]
	void InverseMesh()
	{
		if (_mesh == null)
			_mesh = GetComponent<MeshFilter>().mesh;

		_mesh.triangles = _mesh.triangles.Reverse().ToArray();
	}
}

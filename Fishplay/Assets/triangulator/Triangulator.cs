using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// COMPUTING CONSTRAINED DELAUNAY TRIANGULATIONS
//  尝试用分治法
//  http://www.geom.uiuc.edu/~samuelp/del_project.html
/// </summary>
public class Triangulator : MonoBehaviour {

	private List<Point> _p;

	private List<Point[]> _groups;

	// Use this for initialization
	void Start () {
		int count = 10;
		_p = new List<Point>();
		while(count > 0)
		{
			Vector3 n = UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(1,5);
			n.y = 0;
			_p.Add(new Point(n));
			count--;
		}

		TriangulatorPoints(_p);
	}

	// show points
	static void PrintPoints(IList<Point> t)
	{
		string s = "";
		for(int i=0;i<t.Count;i++)
		{
			if(i>0)
				s += '|';
			s +=string.Format("{0:f1},{1:f1}", t[i].Pos.x, t[i].Pos.z);
		}

		Debug.Log(s);
	}
	
	private void OnDrawGizmos() {
		if(_p == null || _p.Count == 0)
			return;

		for(int i=0;i<_p.Count;i++)
		{
			Gizmos.color = new Color(i / 10.0f, 0, 0, 1);
			Gizmos.DrawWireSphere(_p[i].Pos, 0.1f);
		}

		Gizmos.color = Color.white;
		for(int n=0;n<_groups.Count;n++)
		{
			Point[] ps = _groups[n];
			for(int i=0;i< ps.Length - 1;i++)
				Gizmos.DrawLine(ps[i].Pos, ps[i+1].Pos);
			Gizmos.DrawLine(ps[ps.Length - 1].Pos, ps[0].Pos);
		}
	}

	public Mesh TriangulatorPoints(List<Point> points)
	{
		Mesh m = new Mesh();

		// Debug.Log(string.Format("before: {0}", points));
		PrintPoints(points);

		points.Sort();

/* 		points = new List<Vector3>(points.OrderBy(m,n=>
		{
			if(m.x == n.x)
				m.x;
			else
				x.y;
		})); */

		PrintPoints(points);

		_groups = new List<Point[]>();

		DivideHalfs(points, ref _groups);

		return m;
	}

	/// <summary>
	/// Once the points are ordered, the ordered set is successively divided into halves until 
	// 		we are left with subsets containing no more than three points. 
	// 	These subsets may be instantly triangulated as a segment in the case of two points and
	// 		 a triangle in the case of three points. 
	/// </summary>
	void DivideHalfs(List<Point> points, ref List<Point[]> group)
	{
		int count = points.Count;
		if(count > 3)
		{
			int half = Mathf.CeilToInt(points.Count / 2.0f);
			DivideHalfs(points.GetRange(0, half), ref group);
			DivideHalfs(points.GetRange(half, points.Count - half), ref group);
		}
		else
		{
			group.Add(points.ToArray());
		}
	}
}

public class Point : IComparable<Point>
{
	public Vector3 Pos;

	public Point(Vector3 p) 
	{
		Pos = p;
	}

	/// <summary>
	/// The divide and conquer algorithm only computes the Delaunay triangulation for the convex 
	// 		hull of the point set. 
	// 	The first step is to put all of the points into order of increasing x-coordinates 
	// 		(when two points have the same x-coordinate, their order is determined by their y-coordinates). 
	/// </summary>
	public int CompareTo(Point other)
	{
		if(Mathf.Abs(this.Pos.x - other.Pos.x) < 0.05f)
			return this.Pos.z < other.Pos.z ? -1 : 1;
		return this.Pos.x < other.Pos.x ? -1 : 1;
	}
}
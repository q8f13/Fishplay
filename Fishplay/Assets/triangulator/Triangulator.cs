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

		List<Vector3> points = new List<Vector3>();

		while(count > 0)
		{
			Vector3 n = UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(1,5);
			n.y = 0;
			_p.Add(new Point(n));
			points.Add(n);
			count--;
		}

/* 		Vector3[] supert = GetSuperTriangle(_p);
		Debug.DrawLine(supert[2], supert[0], Color.green,99);
		Debug.DrawLine(supert[0], supert[1], Color.green,99);
		Debug.DrawLine(supert[1], supert[2], Color.green,99);
 */
		// TriangulatorPoints(_p);

		List<Triangle> tris = TriangulatorBowyerWatson(points);
		for(int i=0;i<tris.Count;i++)
		{
			tris[i].DrawGizmo(Color.white, 99.0f);
		}

		// Test_CenterFromThreePoint();
	}

	void Test_CenterFromThreePoint()
	{
		Vector3 p1 = new Vector3(1.0f, 0.0f, 0.0f);
		Vector3 p2 = new Vector3(2.0f, 0.0f, 0.0f);
		Vector3 p3 = new Vector3(3.0f, 0.5f, 1.0f);

		Vector3 t = GetCircleCenter(p1,p2,p3);
		Vector3 normal = Vector3.Cross(p2 - p1, p3 - p1);
		float r = (t-p1).magnitude;

		int count = 360;
		while(count > 0)
		{
			Debug.DrawRay(t,  Quaternion.AngleAxis(count, normal) * (p2 - p1).normalized * r, Color.magenta, 99);
			count--;
		}
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
/* 		for(int n=0;n<_groups.Count;n++)
		{
			Point[] ps = _groups[n];
			for(int i=0;i< ps.Length - 1;i++)
				Gizmos.DrawLine(ps[i].Pos, ps[i+1].Pos);
			Gizmos.DrawLine(ps[ps.Length - 1].Pos, ps[0].Pos);
		} */
	}

	public List<Triangle> TriangulatorBowyerWatson(List<Vector3> points)
	{
		Mesh msh = new Mesh();

		// points = new List<Vector3>(points.OrderBy(x=>x.x));

		Vector3[] supert = GetSuperTriangle(points);

		// PrintPoints(points);
		// List<Triangle> triangleList = new List<Triangle>();
		List<Triangle> tempTriangleList = new List<Triangle>();

		tempTriangleList.Add(new Triangle(supert[0], supert[1], supert[2]));

		foreach(Vector3 p in points)
		{
			List<Edge> edgeBuffer = new List<Edge>();
			// List<Vector3> vectorBuffer = new List<Vector3>();
			for(int i=tempTriangleList.Count - 1;i>=0;i--)
			{
				if(i > tempTriangleList.Count - 1)
					continue;
				Triangle tri = tempTriangleList[i];
				if(tri.Contains(p))
				{
					edgeBuffer.Add(new Edge(tri.A, tri.B));
					edgeBuffer.Add(new Edge(tri.B, tri.C));
					edgeBuffer.Add(new Edge(tri.A, tri.C));

					tempTriangleList.Remove(tri);
				}
			}

			// remove duplicate edge
			for(int m=edgeBuffer.Count - 1;m>=0;m--)
			{
				for(int n=edgeBuffer.Count - 1;n>=0;n--)
				{
					if(m==n)
						continue;
					if(m > edgeBuffer.Count - 1 || n > edgeBuffer.Count - 1)
						continue;
					if(edgeBuffer[m].SameWith(edgeBuffer[n]))
					{
						edgeBuffer.RemoveAt(m);
						edgeBuffer.RemoveAt(n);
						break;
					}
				}
			}

			// connect p to every pt from tris
			List<Triangle> new_tris = new List<Triangle>();
			foreach(Edge ed in edgeBuffer)
			{
				new_tris.Add(new Triangle(ed.A, ed.B, p));
			}

			// TODO: 这里需要针对新形成的三角形们做一下LOP
			List<Triangle> new_tris_addition = new List<Triangle>();
			for(int m=new_tris.Count - 1;m>=0;m--)
			{
				for(int n=new_tris.Count - 1;n>=0;n--)
				{
					if(m==n)
						continue;
					if(m > new_tris.Count - 1 || n > new_tris.Count - 1)
						continue;
					List<Vector3> pt_in_two_tris = new List<Vector3>(new_tris[m].Points.Concat(new_tris[n].Points).Distinct());
					bool first_in_circle_check = new_tris[m].Contains(ExtractOtherPoint(pt_in_two_tris, new_tris[m]));
					bool second_in_circle_check = new_tris[n].Contains(ExtractOtherPoint(pt_in_two_tris, new_tris[n]));
					Edge share = new_tris[m].ShareEdge(new_tris[n]);
					if(share == null)
						continue;
					if(!first_in_circle_check && !second_in_circle_check)
					{
						// need to flip edge
						List<Vector3> pt_others = new List<Vector3>();
						foreach(Vector3 ppp in pt_in_two_tris)
						{
							if(share.A != ppp && share.B != ppp)
							{
								pt_others.Add(ppp);
							}
						}

						new_tris_addition.Add(new Triangle(pt_others[0],share.A, share.B));
						new_tris_addition.Add(new Triangle(pt_others[1],share.A, share.B));
						new_tris.RemoveAt(m);
						new_tris.RemoveAt(n);
						break;
					}
				}
			}

			tempTriangleList.AddRange(new_tris.Concat(new_tris_addition));
		}

		List<Triangle> allin = tempTriangleList;
		for(int t=allin.Count - 1;t>=0;t--)
		{
			if(t > allin.Count - 1)
				continue;
			Triangle tri = allin[t];
			foreach(Vector3 pt in tri.Points)
			{
/* 				if(pt == supert[0]
					|| pt == supert[1]
					|| pt == supert[2]) */
				if(Vector3.Distance(pt, supert[0]) < 0.01f
				|| Vector3.Distance(pt, supert[1]) < 0.01f
				|| Vector3.Distance(pt, supert[2]) < 0.01f)
				{
					allin.Remove(tri);
					break;
				}
			}
		}

		// return msh;
		return allin;
	}

	Vector3 ExtractOtherPoint(List<Vector3> points, Triangle tri)
	{
		foreach(Vector3 p in points)
		{
			if(!tri.Points.Contains(p))
				return p;
		}

		return default(Vector3);
	}

	public Mesh TriangulatorPointsDivideAndConquer(List<Point> points)
	{
		throw new NotImplementedException();
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

	public static Vector3 GetCircleCenter(Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 center = Vector3.zero;
		Vector3 normal = Vector3.Cross(p1 - p2, p1-p3);

		Vector3 m12 = (p1 + p2)/2;
		Vector3 m13 = (p1 + p3)/2;

		Vector3 pp12 = Vector3.Cross((p2-p1),normal);
		Vector3 pp13 = Vector3.Cross((p3-p1),normal);

		// Debug.DrawRay()
		Vector3 two_direction_cross = Vector3.Cross(pp12, pp13);
		float t = Vector3.Dot(Vector3.Cross((m13 - m12), pp13) , two_direction_cross) / two_direction_cross.sqrMagnitude;

		// Vector3 pp_center = Vector3.Cross(m12 + pp12, m13 + pp13);
		Vector3 pp_center = m12 + pp12 * t;
		center = Vector3.ProjectOnPlane(pp_center, normal);

		Debug.DrawLine(p1, p2, Color.red, 99.0f);
		Debug.DrawLine(p2, p3, Color.red, 99.0f);
		Debug.DrawLine(p1, p3, Color.red, 99.0f);
/* 		Debug.DrawRay(m12, pp12, Color.red, 99.0f);
		Debug.DrawRay(m13, pp13, Color.red, 99.0f); */
		// Debug.DrawRay(pp_center, normal, Color.green, 99.0f);
		Debug.DrawRay(center, normal, Color.green, 99.0f);

		return center;
	}

	public static Vector3[] GetSuperTriangle(List<Vector3> points)
	{
		Vector3 left = points[0];
		Vector3 top = left;
		Vector3 bottom = left;
		Vector3 right = left;
		float average_y = 0.0f;

		foreach(Vector3 p in points)
		{
			average_y += p.y;

			if(p.x < left.x)
				left = p;
			else if(p.x > right.x)
				right = p;

			if(p.z > top.z)
				top = p;
			else if(p.z < top.z)
				bottom = p;

			average_y += p.y;
		}

		average_y = average_y / points.Count;
		float x_center = (left.x + right.x)/2.0f;
		float z_center = (bottom.z + top.z)/2.0f;

		float side = Mathf.Abs(left.x - right.x);
		float side_v = Mathf.Abs(top.x - bottom.x);
		float d = side > side_v ? side : side_v;
		Vector3 a = new Vector3(x_center - d*2, average_y, z_center - 1.5f*d);
		Vector3 b = new Vector3(x_center + d*2, average_y, z_center - 1.5f*d);
		Vector3 c = new Vector3(x_center, average_y, z_center + 1.5f*d);

		return new Vector3[]{a,b,c};
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

public class Triangle
{
	public Vector3 A;
	public Vector3 B;
	public Vector3 C;

	public Vector3[] Points{
		get{
			if(_points == null)
				_points = new Vector3[]{A,B,C};
			return _points;
		}
	}

	private Vector3[] _points;

	private Vector3 _center;
	private float _radius;

	public Triangle(Vector3 a, Vector3 b, Vector3 c)
	{
		A = a; B = b; C=c;
	}

	public float Radius()
	{
		if(_radius <= 0.0f)
		{
			_radius = (GetCenter() - A).magnitude;
		}

		return _radius;
	}

	public Edge ShareEdge(Triangle tri)
	{
		int count = 0;
		Vector3[] shared_points = new Vector3[2];
		foreach(Vector3 pa in this.Points)
		{
			foreach(Vector3 pb in tri.Points)
			{
				if(pa == pb)
				{
					shared_points[count] = pb;
					count++;
				}
			}
		}

		if(shared_points[0] == default(Vector3)
			|| shared_points[1] == default(Vector3))
			return null;

		return new Edge(shared_points[0], shared_points[1]);
	}

	public bool Contains(Vector3 p)
	{
		return Vector3.Distance(GetCenter(), p) <= Radius();
	}

	public Vector3 GetCenter()
	{
		if(_center != default(Vector3))
			return _center;

		Vector3 center = Vector3.zero;
		Vector3 normal = Vector3.Cross(A - B, A-C);

		Vector3 m12 = (A + B)/2;
		Vector3 m13 = (A + C)/2;

		Vector3 pp12 = Vector3.Cross((B-A),normal);
		Vector3 pp13 = Vector3.Cross((C-A),normal);

		// Debug.DrawRay()
		Vector3 two_direction_cross = Vector3.Cross(pp12, pp13);
		float t = Vector3.Dot(Vector3.Cross((m13 - m12), pp13) , two_direction_cross) / two_direction_cross.sqrMagnitude;

		// Vector3 pp_center = Vector3.Cross(m12 + pp12, m13 + pp13);
		Vector3 pp_center = m12 + pp12 * t;
		center = Vector3.ProjectOnPlane(pp_center, normal);

/* 		Debug.DrawLine(A, B, Color.red, 99.0f);
		Debug.DrawLine(B, C, Color.red, 99.0f);
		Debug.DrawLine(A, C, Color.red, 99.0f); */
/* 		Debug.DrawRay(m12, pp12, Color.red, 99.0f);
		Debug.DrawRay(m13, pp13, Color.red, 99.0f); */
		// Debug.DrawRay(pp_center, normal, Color.green, 99.0f);
		// Debug.DrawRay(center, normal, Color.green, 99.0f);

		_center = center;
		return _center;
	}

	public void DrawGizmo(Color c, float duration)
	{
		Debug.DrawLine(A,B, c, duration);
		Debug.DrawLine(A,C, c, duration);
		Debug.DrawLine(B,C, c, duration);
	}

	public Vector3 GetTheOther(Vector3[] points)
	{
		Vector3[] this_p = this.Points.OrderBy(x=>x.x).ToArray();
		Vector3[] all_p = points.OrderBy(x=>x.x).ToArray();
		for(int i=0;i<3;i++)
		{
			if(all_p[i] != this_p[i])
				return all_p[i];
		}
		return all_p[3];
	}
}

public class Edge
{
	public Vector3 A;
	public Vector3 B;

	public Edge(Vector3 a, Vector3 b)
	{
		A = a; B = b;
	}

	public bool SameWith(Edge e)
	{
		return (A == e.A && B == e.B)
			|| (A == e.B && B == e.A);
	}
}
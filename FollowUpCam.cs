using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// cam that following specific target
/// note that it's rotating via mouse offset from screen center
/// </summary>
public class FollowUpCam : MonoBehaviour
{
	public Transform Target;
	public float FollowSpeed = 5.0f;

	private Camera _cam;

	private Vector3 _offsetDir;
	private float _offsetDistance;
	public float OffsetDistance { get { return _offsetDistance; } }

	private Vector3 _dir;

	void Start () {
		_cam = GetComponent<Camera>();

		// get cam2char offset
		_offsetDir = (Target.InverseTransformPoint(transform.position)).normalized;
		_offsetDistance = (Target.position - transform.position).magnitude;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// tracking 
		_cam.transform.position = Target.position + Target.TransformDirection(_offsetDir)*_offsetDistance;

		if (_dir != default (Vector3))
		{
			_cam.transform.rotation = Quaternion.Slerp(_cam.transform.rotation, Quaternion.LookRotation(_dir, _cam.transform.up),
				Time.fixedDeltaTime*FollowSpeed);
		}
//			_cam.transform.forward = Vector3.Slerp(_cam.transform.forward, _dir, Time.deltaTime*5.0f);
	}

	public void SetTargetForward(Vector3 dir)
	{
		_dir = dir;
	}
}

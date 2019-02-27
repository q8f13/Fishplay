using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionPlay : MonoBehaviour {

	public Transform TargetT;
	public float Angle = 90.0f;
	public float RotateSpeed = 5.0f;

#region InverseQuaternion
	[SerializeField]
	private float _currentDelta;
	[SerializeField]
	private Quaternion _rot;

	private int _rotateDirection = 1;
	[SerializeField]
	private Quaternion _targetRot;
#endregion

#region Quaternion Cross
	[SerializeField]
	private Transform _pointCube;
	[SerializeField]
	private Transform _axisOne;
	[SerializeField]
	private Transform _axisTwo;
	private LineRenderer _lr;

	[SerializeField, Range(0.0f, 1.0f)]
	private float _exponentPercent = 0.0f;
#endregion


	// Use this for initialization
	void Start () {
		_targetRot = Quaternion.AngleAxis(Angle, Vector3.up);

		_lr = gameObject.GetComponent<LineRenderer>();
		if(_lr == null)
			_lr = gameObject.AddComponent<LineRenderer>();
		_lr.widthCurve = AnimationCurve.Constant(0, 1, 1f);
	}

	void DoQuaternionCross()
	{
		_lr.SetPosition(0, _axisOne.position);
		_lr.SetPosition(1, _axisTwo.position);

		Vector3 axis = _axisOne.position - _axisTwo.position;
		// Quaternion rot = Quaternion.AngleAxis(Angle, axis);
		Quaternion rot = Quaternion.AngleAxis(Angle, TargetT.up);
		Vector3 point = _pointCube.position;

		Quaternion p = new Quaternion(point.x, point.y, point.z, 0);

		Quaternion next_rot = rot * p * Quaternion.Inverse(rot);

		// _pointCube.transform.rotation = next_rot;
		// next_rot = QuaternionExp(next_rot, _exponentPercent);
		Quaternion exponent_test = QuaternionExp(rot, _exponentPercent);

		// Vector3 next_p = new Vector3(next_rot.x, next_rot.y, next_rot.z);
		// _pointCube.transform.position = next_p;
		_pointCube.transform.rotation = exponent_test;
	}

	void DoInverseRotation()
	{
		_targetRot.w = Mathf.Abs(_targetRot.w) * _rotateDirection;
		_currentDelta = (1.0f - Mathf.Exp(-RotateSpeed*Time.deltaTime));

		TargetT.transform.rotation = Quaternion.Slerp(TargetT.transform.rotation, _targetRot, _currentDelta);
		_rot = TargetT.transform.rotation;
		if(Input.GetKeyUp(KeyCode.Backspace))
			_rotateDirection *= -1;
	}
	
	// Update is called once per frame
	void Update () {
		// DoInvers invereRotation();
		DoQuaternionCross();
	}

	/// <summary>
	/// 四元数指数计算
	/// 可用来算取百分比后的某个角位移的四元数是多少
	/// </summary>
	public static Quaternion QuaternionExp(Quaternion q, float exponent)
	{
		if(Mathf.Abs(q.w) < 0.9999f)
		{
			float alpha = Mathf.Acos(q.w);
			float new_alpha = alpha * exponent;
			float w = Mathf.Cos(new_alpha);
			float multi = Mathf.Sin(new_alpha) / Mathf.Sin(alpha);

			q.x*=multi;
			q.y*=multi;
			q.z*=multi;
		}

		return q;
	}
}

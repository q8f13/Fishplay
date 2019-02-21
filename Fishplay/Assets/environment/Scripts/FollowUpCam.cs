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
	public float FollowSpeedMin = 1.0f;

	private Camera _cam;

	private Vector3 _offsetDir;
	private float _offsetDistance;
	public float OffsetDistance { get { return _offsetDistance; } }

	private Vector3 _dir;

	private float _fovRangeMax = 80.0f;
	private float _fovRangeNormal = 60.0f;
	private float _fovCurrentInterpolate = 0.0f;

	[SerializeField]
	private ShipParameter _shipParamPlay;

	private ParticleChildControl _psOnTarget;
	private UnitMotor _motor;
	private UnitController _unitControl;

	private Vector3 _weaponAimingPoint;			// 当前本体的武器瞄点。测试是否raycast到物体，如没有则由武器口到预瞄方向最大距离打射线

	private Rayhit _rayHitControl;

	void Start () {
		_cam = GetComponent<Camera>();

		// get cam2char offset
		_offsetDir = (Target.InverseTransformPoint(transform.position)).normalized;
		_offsetDistance = (Target.position - transform.position).magnitude;

		_psOnTarget = Target.GetComponent<ParticleChildControl>();
		_motor = Target.GetComponent<UnitMotor>();
		_unitControl = Target.GetComponent<UnitController>();

		_rayHitControl = new Rayhit(_cam, LayerMask.GetMask("object"));
	}

	private void Update() {
		
	}

	// Update is called once per frame
	void FixedUpdate () {
		// tracking 
		_cam.transform.position = Target.position + Target.TransformDirection(_offsetDir)*_offsetDistance;

		if (_dir != default (Vector3))
		{
			float rotateSpeedInterpolated = Mathf.Lerp(FollowSpeedMin, FollowSpeed, (1.0f - _motor.SpeedPercent));
//			_cam.transform.rotation = Quaternion.Slerp(_cam.transform.rotation, Quaternion.LookRotation(_dir, _cam.transform.up),
//				Time.fixedDeltaTime*FollowSpeed);
			_cam.transform.rotation = Quaternion.Slerp(_cam.transform.rotation, Quaternion.LookRotation(_dir, _cam.transform.up),
				Time.fixedDeltaTime*rotateSpeedInterpolated);
		}
//			_cam.transform.forward = Vector3.Slerp(_cam.transform.forward, _dir, Time.deltaTime*5.0f);

		AdjustFOVAccordingToBoost(_psOnTarget.IsOn);
	}

	void AdjustFOVAccordingToBoost(bool boostOn)
	{
		if (boostOn)
			_fovCurrentInterpolate += Time.deltaTime;
		else
			_fovCurrentInterpolate -= Time.deltaTime;

		_fovCurrentInterpolate = Mathf.Clamp01(_fovCurrentInterpolate);

		_cam.fieldOfView = Mathf.Lerp(_fovRangeNormal, _fovRangeMax, _fovCurrentInterpolate);
	}

	public void SetTargetForward(Vector3 dir)
	{
		_dir = dir;
	}

	private void OnDrawGizmos() {
		if(_unitControl == null)
			return;
		Gizmos.color = Color.green;
		// left weapon
		Vector3 phit = _rayHitControl.GetTargetPoint(_unitControl.WeaponLeft, Input.mousePosition, _shipParamPlay.CurrentWeaponMaxRange);
		Gizmos.DrawLine(_cam.transform.position, Target.position);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(_unitControl.WeaponLeft.position, phit);
		Gizmos.DrawWireSphere(phit, 0.1f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(_cam.transform.position, phit);

	}
}

[System.Serializable]
public class ShipParameter
{
	public float CurrentWeaponMaxRange = 280.0f;
}
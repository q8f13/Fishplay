using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Motor控制
/// </summary>
public class UnitMotor : MonoBehaviour
{
	public float RotationSpeed = 1f;
	public float RotationSpeedMin = 0.5f;

	public float RollSpeed = 1f;

	private Rigidbody _rig;

	private Vector3 _currentForwardGoing;

	private float throttle;
	private float throttleSpeed = 2.0f;
	private float maxThrottle = 100.0f;
	private float minThrottle = 0.0f;
	private float enginePower = 5.0f;

	private float _startDrag = 0.0f;

	// TODO: 临时值
	private float _boosterForce = 50.0f;
	private float _maxSpeedWithBoost = 100.0f;

	[SerializeField] private float _rigSpeed;

	[SerializeField]
	private float _euler_x_offset;

//	private FollowUpCam _cam;

	public float SpeedPercent { get { return _rigSpeed/_maxSpeedWithBoost; } }

	// Use this for initialization
	void Start ()
	{
		_rig = GetComponent<Rigidbody>();
		_startDrag = _rig.drag;
//		_cam = Camera.main.GetComponent<FollowUpCam>();
	}

	public void SpeedUp(float xInput, float yInput)
	{
		// natural slowing. for gameplay purposes not realistic
		throttle += yInput * throttleSpeed * Time.fixedDeltaTime;
		throttle = Mathf.Clamp(throttle, minThrottle, maxThrottle);

		// press 's' to deaccelerate
		if (yInput < 0.0f)
		{
			_rig.drag = Mathf.Lerp(_startDrag, 1.0f, -yInput);
		}

		_rigSpeed = _rig.velocity.magnitude;
		if(_rigSpeed < _maxSpeedWithBoost)
			_rig.AddRelativeForce(0, 0, throttle*enginePower);

		throttle *= (1.0f - (Time.fixedDeltaTime*throttleSpeed/4.0f));

		// 计算本地坐标系横向的偏移量。用于roll角度
		_euler_x_offset= Vector3.Dot(transform.forward - _currentForwardGoing, transform.right);
		
		// roll角度旋转
		// TODO: 目前横滚后距离相机中线距离有点大，需要继续调整
		Quaternion rollQuat = Quaternion.AngleAxis(_euler_x_offset*60.0f, _currentForwardGoing);
		// 朝向相机正前方的旋转
		Quaternion targetQuat = Quaternion.LookRotation(_currentForwardGoing, Camera.main.transform.up);

		targetQuat = rollQuat*targetQuat;

		float rotateSpeedInterpolated = Mathf.Lerp(RotationSpeedMin, RotationSpeed, (1.0f - SpeedPercent));

//		_rig.rotation =
//			Quaternion.Slerp(_rig.rotation,
//				targetQuat,
//				Time.fixedDeltaTime*RotationSpeed);
		_rig.rotation =
			Quaternion.Slerp(_rig.rotation,
				targetQuat,
				Time.fixedDeltaTime*rotateSpeedInterpolated);
	}

	public void BoostUp(bool on)
	{
		if(on && _rigSpeed < _maxSpeedWithBoost)
		{
			_rig.AddRelativeForce(0,0,_boosterForce * enginePower * Time.fixedDeltaTime, ForceMode.Impulse);
		}
	}

	public void ChangeForward(Vector3 fwd)
	{
		_currentForwardGoing = fwd;
	}
}

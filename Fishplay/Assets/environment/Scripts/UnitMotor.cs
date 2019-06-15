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

	public float PilotSensitivity = 5.0f;

	private Rigidbody _rig;

	private Vector3 _currentForwardGoing;
	private float _angleOffTarget;

	private float throttle;
	private float throttleSpeed = 500.0f;
	private float maxThrottle = 100.0f;
	private float minThrottle = 0.0f;
	private float enginePower = 5.0f;

	[SerializeField, Header("引擎推力。水平垂直前后四个值")]
	private Vector4 _enginePower = new Vector4(2.5f, 1.2f, 5.0f, 0.2f);

	private Vector2 _currInput;

	private float _startDrag = 0.0f;

	// TODO: 临时值
	private float _boosterForce = 20.0f;
	private float _maxSpeedWithBoost = 100.0f;

	[Header("Input")]
	[SerializeField, Range(-1f, 1f)]
	private float _pitch = 0f;
	[SerializeField, Range(-1f, 1f)]
	private float _yaw = 0f;
	[SerializeField, Range(-1f, 1f)]
	private float _roll = 0f;

	public float Pitch{set{_pitch = Mathf.Clamp(value, -1f, 1f);}get{return _pitch;}}
	public float Yaw{set{_yaw = Mathf.Clamp(value, -1f, 1f);}get{return _yaw;}}
	public float Roll{set{_roll = Mathf.Clamp(value, -1f, 1f);}get{return _roll;}}

	[Tooltip("Pitch, Yaw, Roll")] public Vector3 turnTorque = new Vector3(90f, 25f, 45f);
	[Tooltip("Multiplier for all forces")] public float forceMult = 1000f;

	[SerializeField] private float _rigSpeed;

	[SerializeField]
	private float _euler_x_offset;

//	private FollowUpCam _cam;

	public float SpeedPercent { get { return _rigSpeed/_maxSpeedWithBoost; } }
	public float ThrottlePercent{get{return throttle / maxThrottle; }}

	// Use this for initialization
	void Start ()
	{
		_rig = GetComponent<Rigidbody>();
		_startDrag = _rig.drag;
//		_cam = Camera.main.GetComponent<FollowUpCam>();
	}

	public void SpeedUp(float xInput, float yInput)
	{
		_currInput.x = xInput;
		_currInput.y = yInput;

		// natural slowing. for gameplay purposes not realistic
		throttle += yInput * throttleSpeed * Time.fixedDeltaTime;
		throttle = Mathf.Clamp(throttle, minThrottle, maxThrottle);

		// press 's' to deaccelerate
		if (yInput < 0.0f)
		{
			_rig.drag = Mathf.Lerp(_startDrag, 1.0f, -yInput);
		}

		// TODO: 垂直输入还没考虑（everspace的空格LCtrl）
		_rigSpeed = _rig.velocity.magnitude;
		if(_rigSpeed < _maxSpeedWithBoost)
			// FIXME: hack了横向的杆量
			// _rig.AddRelativeForce(xInput * 5.0f* _enginePower.x, 0, throttle*_enginePower.z);

		throttle *= (1.0f - (Time.fixedDeltaTime*throttleSpeed/4.0f));

		// 计算本地坐标系横向的偏移量。用于roll角度
		_euler_x_offset= Vector3.Dot(_currentForwardGoing, transform.right);
		// _euler_x_offset= Vector3.Dot(_currentForwardGoing, transform.right);
		// _euler_x_offset= Vector3.Dot(transform.forward - _currentForwardGoing, transform.right);
		
		// roll角度旋转
		// TODO: 目前横滚后距离相机中线距离有点大，需要继续调整
		// Quaternion rollQuat = Quaternion.AngleAxis(_euler_x_offset*60.0f, transform.forward);
		// Quaternion rollQuat = Quaternion.AngleAxis(-_euler_x_offset*60.0f, _currentForwardGoing);
		// 朝向相机正前方的旋转
		// Quaternion targetQuat = Quaternion.RotateTowards(_rig.rotation, Quaternion.LookRotation(_currentForwardGoing, Camera.main.transform.up), 1.0f);
		Quaternion targetQuat = Quaternion.LookRotation(_currentForwardGoing, Camera.main.transform.up);
		// Quaternion targetQuat = Quaternion.Slerp(_rig.rotation)

		// targetQuat = rollQuat*targetQuat;

		float rotateSpeedInterpolated = Mathf.Lerp(RotationSpeedMin, RotationSpeed, Mathf.Clamp01(1.0f - SpeedPercent));

		Debug.DrawRay(transform.position, _currentForwardGoing, Color.magenta);

		float autoYaw = 0f;
		float autoPitch = 0f;
		float autoRoll = 0f;
		// RunPilot(_currentForwardGoing, out autoYaw, out autoPitch, out autoRoll);

		Yaw = autoYaw;
		Pitch = autoPitch;
		Roll = autoRoll;

		// transform.localRotation = Quaternion.Slerp(transform.localRotation, rollQuat, Time.fixedDeltaTime * rotateSpeedInterpolated);


		// _rig.rotation = targetQuat;
		// _rig.rotation =
		// 	Quaternion.Slerp(_rig.rotation,
		// 		targetQuat,
		// 		Time.fixedDeltaTime*RotationSpeed);
		// _rig.rotation =
		// 	Quaternion.Slerp(transform.rotation,
		// 	// Quaternion.Slerp(_rig.rotation,
		// 		targetQuat,
		// 		1 - Mathf.Exp(-rotateSpeedInterpolated * Time.deltaTime));
	}

	private void OnGUI() {
		GUI.Label(new Rect(0,0,100,60), string.Format("input {0:F2}, {1:F2}", _currInput.x, _currInput.y));
	}

	void RunPilot(Vector3 fly_dir_local, out float yaw, out float pitch, out float roll)
	{
		yaw = Mathf.Clamp(fly_dir_local.x, -1f, 1f);
		pitch = -Mathf.Clamp(fly_dir_local.y, -1f, 1f);
		// roll = 0;
		roll = transform.right.y;
	}	

	private void FixedUpdate() {
 		_rig.AddRelativeForce(-Vector3.forward*throttle*forceMult, ForceMode.Force);
		_rig.AddRelativeTorque(new Vector3(turnTorque.x * _pitch
											, turnTorque.y * _yaw
											, -turnTorque.z * _roll) * forceMult, ForceMode.Force);
 	}

	public void BoostUp(bool on)
	{
		if(on && _rigSpeed < _maxSpeedWithBoost)
		{
			_rig.AddRelativeForce(0,0,_boosterForce * enginePower * Time.fixedDeltaTime, ForceMode.Impulse);
		}
	}

	public void ChangeForward(Vector3 dir, Vector3 up)
	// public void ChangeForward(Vector3 target_point)
	{
		_currentForwardGoing = dir;
		_rig.transform.rotation = Quaternion.LookRotation(_currentForwardGoing, up);
		// _currentForwardGoing = (target_point - transform.position).normalized;

		// _currentForwardGoing = transform.InverseTransformPoint(target_point).normalized * PilotSensitivity;
		// _angleOffTarget = Vector3.Angle(transform.forward, target_point - transform.position);
	}
}

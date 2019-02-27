using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// cam that following specific target
/// note that it's rotating via mouse offset from screen center
/// </summary>
public class FollowUpCam : MonoBehaviour
{
	[SerializeField]
	private Transform _cameraRig;

	[SerializeField]
	private UnityEngine.UI.Text _indicator;

	[SerializeField, Range(0.0f, 5.0f), Header("相机本体在左右旋转镜头时的水平偏移量")]
	private float _cameraHorizonOffset;
	[SerializeField, Range(0.0f, 5.0f), Header("相机本体在左右旋转镜头时的垂直偏移量")]
	private float _cameraVertOffset;

	[SerializeField]
	private float _aimDistance = 500.0f;

	public float MouseSensitivity = 5;

	public Transform Target;
	// public float FollowSpeed = 5.0f;
	[SerializeField]
	private float _currentFollowSpeed;
	public float FollowSpeedMin = 1.0f;
	public float FollowSpeedMax = 5.0f;

	[SerializeField, Header("横向鼠标屏幕空间用于输入值的采样曲线")]
	private AnimationCurve _horizontalSampleCurve;
	[SerializeField, Header("纵向鼠标屏幕空间用于输入值的采样曲线")]
	private AnimationCurve _verticalSampleCurve;
	private float _camLocalStartY;

	private Camera _cam;
	public Camera Cam {get{return _cam;}}

	private Vector3 _offsetDir;
	private float _offsetDistance;
	private Vector3 _offset;
	public float OffsetDistance { get { return _offsetDistance; } }

	private Vector3 _dir;

	private float _fovRangeMax = 80.0f;
	private float _fovRangeNormal = 60.0f;
	private float _fovCurrentInterpolate = 0.0f;

	private Vector3 _camStartForward;

	[Range(1.0f, 5.0f)]
	public float RotateSenseMulti = 1.0f;

	[SerializeField]
	private ShipParameter _shipParamPlay;

	[SerializeField]
	private Transform _mouseAim;

	private ParticleChildControl _psOnTarget;
	private UnitMotor _motor;
	private UnitController _unitControl;

	private Vector3 _weaponAimingPoint;			// 当前本体的武器瞄点。测试是否raycast到物体，如没有则由武器口到预瞄方向最大距离打射线

	private Rayhit _rayHitControl;

	private Quaternion _rotOffset;

	Vector3 _continuousInputVec;
	private const float INPUT_CLAMP = 2.0f;
	private Vector3 _scrCenter;

void Start () {
		_cam = GetComponentInChildren<Camera>();

		_camStartForward = _cameraRig.transform.forward;

		// get cam2char offset
		// _offset = Target.InverseTransformVector(Target.position - transform.position);
		// _offset = transform.InverseTransformVector(Target.position - transform.position);
		// _offsetDir = (Target.InverseTransformPoint(transform.position)).normalized;
		// _offsetDistance = (Target.position - transform.position).magnitude;

		_psOnTarget = Target.GetComponent<ParticleChildControl>();
		_motor = Target.GetComponent<UnitMotor>();
		_unitControl = Target.GetComponent<UnitController>();

		_rayHitControl = new Rayhit(_cam, LayerMask.GetMask(Rayhit.LAYER_CAN_RAY_HIT));

		_camLocalStartY = _cam.transform.localPosition.y;

		_scrCenter = new Vector3(Screen.width*0.5f, Screen.height*0.5f, 0);
	}

	public Vector3 MouseAimPos()
	{
		return _mouseAim == null 
			? transform.forward * _aimDistance
			: _mouseAim.forward * _aimDistance + _mouseAim.position;
	}

	public Transform MouseAim()
	{
		return _mouseAim;
	}

	public Vector3 CameraRigUp()
	{
		return _cameraRig.up;
	}

	private void UpdateInput()
	{
		Vector3 input_vec = Input.mousePosition;
		Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
		// float input_x = input_vec.x - center.x - 0.5f;
		// float input_y = input_vec.y - center.y - 0.5f;
		float input_x = Mathf.InverseLerp(0, Screen.width, input_vec.x) * 2.0f - 1.0f;
		float input_y = Mathf.InverseLerp(0, Screen.height, input_vec.y) * 2.0f - 1.0f;

		float abs_x = Mathf.Abs(input_x);
		float abs_y = Mathf.Abs(input_y);
		Debug.Assert(abs_x >= 0.0f && abs_x <= 1.0f);
		Debug.Assert(abs_y >= 0.0f && abs_y <= 1.0f);

		if(abs_x != 0.0f)
			input_x = _horizontalSampleCurve.Evaluate(Mathf.Abs(input_x)) * ((Mathf.Abs(input_x) / input_x));
		if(abs_y != 0.0f)
			input_y = _verticalSampleCurve.Evaluate(Mathf.Abs(input_y)) * ((Mathf.Abs(input_y) / input_y));

		_indicator.text = string.Format("input x: {0}\ninput y: {1}", input_x, input_y);

		_mouseAim.Rotate(_cam.transform.right, -input_y*RotateSenseMulti, Space.World);
		_mouseAim.Rotate(_cam.transform.up, input_x*RotateSenseMulti, Space.World);
		Debug.DrawRay(_mouseAim.position, _mouseAim.forward, Color.green);
		// _mouseAim.transform.forward = _cam.ScreenPointToRay(Input.mousePosition).direction;
		// _mouseAim.transform.forward = new Vector3(input_x, input_y, 1.0f);

		Vector3 upVec = _cameraRig.up;

		float percent_cursor_center_to_edge = Mathf.Max(Mathf.Abs(input_x), Mathf.Abs(input_y)) / 0.5f;
		// float percent_cursor_center_to_edge_y = input_y / (Screen.height * 0.5f);

		// 相机水平偏移
		Vector3 cam_local = _cam.transform.localPosition;
		cam_local.x = input_x * _cameraHorizonOffset;
		cam_local.y = _camLocalStartY - abs_x * _cameraVertOffset;
		_cam.transform.localPosition = Vector3.Slerp(_cam.transform.localPosition, cam_local, (1.0f - Mathf.Exp(-5f* Time.deltaTime)));

		_currentFollowSpeed = Mathf.Lerp(FollowSpeedMin, FollowSpeedMax, percent_cursor_center_to_edge);

		_cameraRig.rotation = Damp(_cameraRig.rotation
				, Quaternion.LookRotation(MouseAimPos() - _cameraRig.position, upVec)
				, _currentFollowSpeed
				, Time.deltaTime);
	}

	public static Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt)
	{
		return Quaternion.Slerp(a,b,1.0f - Mathf.Exp(-lambda * dt));
	}

	private void Update() {

		transform.position = Target.position;

		UpdateInput();

		Debug.DrawRay(_cameraRig.position, _cameraRig.forward * 5.0f, Color.magenta);
		Debug.DrawRay(Target.position, Target.forward * 5.0f, Color.red);

		// tracking 
		Vector3 p=Target.position + Target.TransformVector(-_offset);
		// _cam.transform.position = Target.position + Target.TransformDirection(_offsetDir)*_offsetDistance;
		// _cam.transform.position = p;
		// _cam.transform.position = p;
		// _cam.transform.position = Vector3.Lerp(_cam.transform.position, p, Time.fixedDeltaTime*FollowSpeed);
		// _cam.transform.position = 

		// if (_dir != default (Vector3))
		// {
		// float rotateSpeedInterpolated = Mathf.Lerp(FollowSpeedMin, FollowSpeed, (1.0f - _motor.SpeedPercent));
		// Quaternion rot = Quaternion.LookRotation(_dir, _cam.transform.up);
//			_cam.transform.rotation = Quaternion.Slerp(_cam.transform.rotation, Quaternion.LookRotation(_dir, _cam.transform.up),
//				Time.fixedDeltaTime*FollowSpeed);
		Quaternion offset_rot = Quaternion.LookRotation(Target.TransformDirection(_camStartForward),Target.up);
		// _cameraRig.transform.rotation =  offset_rot;
		// _cam.transform.rotation = Quaternion.LookRotation(Target.forward,  transform.up);
		// _cam.transform.rotation = Quaternion.LookRotation(Target.forward,  transform.up);
		// _cam.transform.rotation = Quaternion.Slerp(_cam.transform.rotation, rot,
		// 	Time.fixedDeltaTime*rotateSpeedInterpolated);
		// _cam.transform.rotation = Quaternion.Slerp(_cam.transform.rotation, Quaternion.LookRotation(_dir, _cam.transform.up),
		// 	Time.fixedDeltaTime*rotateSpeedInterpolated);
		// }
//			_cam.transform.forward = Vector3.Slerp(_cam.transform.forward, _dir, Time.deltaTime*5.0f);

		AdjustFOVAccordingToBoost(_psOnTarget.IsOn);


	}

	// Update is called once per frame
	void FixedUpdate () {
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
/* 		if(_unitControl == null)
			return;
		Gizmos.color = Color.green;
		// left weapon
		Vector3 phit = _rayHitControl.GetTargetPoint(_unitControl.WeaponLeft, Input.mousePosition, _shipParamPlay.CurrentWeaponMaxRange);
		Gizmos.DrawLine(_cam.transform.position, Target.position);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(_unitControl.WeaponLeft.position, phit);
		Gizmos.DrawWireSphere(phit, 0.1f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(_cam.transform.position, phit); */

	}
}

[System.Serializable]
public class ShipParameter
{
	public float CurrentWeaponMaxRange = 280.0f;
}
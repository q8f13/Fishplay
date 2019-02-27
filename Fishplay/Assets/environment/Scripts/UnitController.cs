using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit输入控制
/// </summary>
[RequireComponent(typeof(UnitMotor))]
public class UnitController : MonoBehaviour
{
	public Transform WeaponLeft;
	public Transform WeaponRight;

	private UnitMotor _motor;

	[SerializeField] private Vector3 _mousePos;

	private FollowUpCam _cam;

	private ParticleSystem _engineTail;
	private ParticleSystem.ForceOverLifetimeModule _tailModule;
	private ParticleSystem.MinMaxCurve _emitterValue;

	private Vector3 _targetPt;

	private ParticleChildControl _ps;

	private Ray _ray;

	void Start()
	{
		_motor = GetComponent<UnitMotor>();
		_cam = Camera.main.GetComponentInParent<FollowUpCam>();
		_ps = GetComponent<ParticleChildControl>();
		_ps.Toggle(false);

		_engineTail = transform.Find("engineTail").GetComponent<ParticleSystem>();
		_tailModule = _engineTail.forceOverLifetime;
		_emitterValue = _tailModule.z;
	}

	void UpdateTailFx(float percent)
	{
		_emitterValue.constant = Mathf.Lerp(0.0f, -1.0f, percent);
		_tailModule.z = _emitterValue;
	}

	private void Update() {
		_mousePos = Input.mousePosition;
		// change mouse offset to target forward dir
		// pivot to screen center
		// calc offset
		// _mousePos.x = Mathf.Clamp(_mousePos.x, 0, Screen.width);
		// _mousePos.y = Mathf.Clamp(_mousePos.y, 0, Screen.height);
		// _mousePos.z = _cam.Cam.nearClipPlane;
		_ray = Camera.main.ScreenPointToRay(_mousePos);

		float val = _cam.OffsetDistance * 100.0f;
		_targetPt = _cam.transform.position + _ray.direction*val;

		Vector3 dirToGo = (_targetPt - transform.position).normalized;

		// speedup motor
		_cam.SetTargetForward(_ray.direction);

		_motor.ChangeForward(_cam.MouseAim().forward, _cam.CameraRigUp());
		// _motor.ChangeForward(_cam.MouseAimPos());
		_motor.SpeedUp(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		// _motor.ChangeForward(dirToGo);
		bool boostUpOn = Input.GetKey(KeyCode.LeftShift);
		// _ps.Toggle(boostUpOn);
		if(Input.GetKey(KeyCode.LeftShift))
			_ps.Emit();
		_motor.BoostUp(boostUpOn);

		UpdateTailFx(_motor.ThrottlePercent);
	}

	private void LateUpdate() {
		// Debug.DrawRay(_ray.origin, _ray.direction * 10.0f, Color.green);
		// Debug.DrawRay(transform.position, _ray.direction * 10.0f, Color.red);
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
	}

	void OnDrawGizmos()
	{
		if (_cam == null)
			return;

		Gizmos.color = Color.white;
		// Gizmos.DrawWireSphere(_targetPt, 0.2f);
//		Ray r = Camera.main.ScreenPointToRay(_mousePos);
//		float val = _cam.OffsetDistance * 4;
//		Gizmos.DrawRay(Camera.main.transform.position, r.direction * val);
////		Gizmos.DrawWireSphere(_targetPt, 0.2f);
//		Gizmos.color = Color.red;
//		Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward*val);
//		Gizmos.color = Color.green;
//		Gizmos.DrawLine(Camera.main.transform.position + r.direction*val
//			, Camera.main.transform.position + Camera.main.transform.forward*val);
	}
}

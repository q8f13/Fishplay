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

	private Vector3 _targetPt;

	private ParticleChildControl _ps;

	void Start()
	{
		_motor = GetComponent<UnitMotor>();
		_cam = Camera.main.GetComponent<FollowUpCam>();
		_ps = GetComponent<ParticleChildControl>();
		_ps.Toggle(false);
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		_mousePos = Input.mousePosition;
		// change mouse offset to target forward dir
		// pivot to screen center
		// calc offset
		_mousePos.x = Mathf.Clamp(_mousePos.x, 0, Screen.width);
		_mousePos.y = Mathf.Clamp(_mousePos.y, 0, Screen.height);
		Ray r = Camera.main.ScreenPointToRay(_mousePos);
		float val = _cam.OffsetDistance * 2.0f;
		_targetPt = _cam.transform.position + r.direction*val;

		Vector3 dirToGo = (_targetPt - transform.position).normalized;

		// speedup motor
		_cam.SetTargetForward(r.direction);
		_motor.ChangeForward(dirToGo);
		_motor.SpeedUp(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		bool boostUpOn = Input.GetKey(KeyCode.LeftShift);
		// _ps.Toggle(boostUpOn);
		if(Input.GetKey(KeyCode.LeftShift))
			_ps.Emit();
		_motor.BoostUp(boostUpOn);
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

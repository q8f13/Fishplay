using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMotor : MonoBehaviour
{
	public float RotationSpeed = 1f;

	private Rigidbody _rig;

	private Vector3 _currentForwardGoing;
	private float throttle;
	private float throttleSpeed = 2.0f;
	private float maxThrottle = 100.0f;
	private float minThrottle = 0.0f;
	private float enginePower = 5.0f;

	// Use this for initialization
	void Start ()
	{
		_rig = GetComponent<Rigidbody>();
	}

	void LateUpdate()
	{
	}
	
	public void SpeedUp(float xInput, float yInput)
	{
		// natural slowing. for gameplay purposes not realistic
		throttle += Input.GetAxis("Vertical") * throttleSpeed * Time.fixedDeltaTime;
		throttle = Mathf.Clamp(throttle, minThrottle, maxThrottle);

		_rig.AddRelativeForce(0, 0, throttle*enginePower);
		throttle *= (1.0f - (Time.fixedDeltaTime*throttleSpeed/4.0f));

//		transform.forward = Vector3.Slerp(transform.forward, _currentForwardGoing, Time.fixedDeltaTime);
		_rig.rotation = Quaternion.Slerp(transform.rotation,
			Quaternion.LookRotation(_currentForwardGoing, Camera.main.transform.up),
			Time.fixedDeltaTime*RotationSpeed);
//		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_currentForwardGoing, Camera.main.transform.up),
//			Time.fixedDeltaTime * RotationSpeed);
	}

	public void ChangeForward(Vector3 fwd)
	{
		_currentForwardGoing = fwd;
	}
}

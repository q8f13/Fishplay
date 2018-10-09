using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallInput : MonoBehaviour {

	public float ForceScale = 10.0f;
	public UnityEngine.UI.Text Console;

	private Rigidbody _rig;

	private Clock _clock;

	// Use this for initialization
	void Start () {
		_rig = GetComponent<Rigidbody>();
		Console.text = "";
		_clock = FindObjectOfType<Clock>();
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyUp(KeyCode.W))
			InputForceAndDisplay(KeyCode.W, Vector3.up);
		else if(Input.GetKeyUp(KeyCode.S))
			InputForceAndDisplay(KeyCode.S, Vector3.down);
		else if(Input.GetKeyUp(KeyCode.A))
			InputForceAndDisplay(KeyCode.A, Vector3.left);
		else if(Input.GetKeyUp(KeyCode.D))
			InputForceAndDisplay(KeyCode.D, Vector3.right);
	}

	void InputForceAndDisplay(KeyCode kcode, Vector3 direction)
	{
		Console.text += kcode.ToString() + "\n";
		if(_clock.Scale < 1.0f)
			return;
		_rig.AddForce(direction * ForceScale, ForceMode.Impulse);
	}
}

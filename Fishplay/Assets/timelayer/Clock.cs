using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour {

	public Rigidbody Ball;
	[Range(0.0f, 1.0f)]
	public float Scale = 1.0f;

	// Use this for initialization
	void Start () {
		Physics.autoSimulation = false;
	}
	
	void FixedUpdate()
	{
		Physics.Simulate(Time.fixedDeltaTime * Scale);
	}

	private void OnGUI()
	{
		Scale = GUI.HorizontalSlider(new Rect(Screen.width - 140, Screen.height - 40,140, 40)
			, Scale
			, 0.0f
			, 1.0f)	;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleChildControl : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem _ps;

	public bool IsOn { get { return _ps.isPlaying; } }

	public void Toggle(bool on)
	{
		if(on)
			_ps.Play();
		else
			_ps.Stop();
	}
}

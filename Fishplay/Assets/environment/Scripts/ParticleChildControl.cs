using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleChildControl : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem _ps;

	public bool IsOn { get { return _ps.isPlaying; } }

	private void Start() {
		ParticleSystem.EmissionModule em = _ps.emission;
		em.enabled = false;
		// _ps.emission.enabled = false;
	}

	public void Toggle(bool on)
	{
		if(on)
			_ps.Play();
		else
			_ps.Stop();
	}

	public void Emit()
	{
		_ps.Emit(5);
	}
}

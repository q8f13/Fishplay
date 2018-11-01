using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFaceTrigger : MonoBehaviour {

	private int _targetLayer;
	private RaycastHit _hit;

	private void Start() {
		_targetLayer = LayerMask.NameToLayer("trigger");
	}

	// Update is called once per frame
	void Update () {
		if(Physics.BoxCast(transform.position, Vector3.one *0.5f, transform.forward, out _hit,  Quaternion.identity, 0.5f, 1<< _targetLayer));
		{
			// Debug.Log(string.Format("boxcast triggered, layer: {0}", _targetLayer));
			if(_hit.collider != null && Input.GetKeyUp(KeyCode.E))
			{
				FacilityBase target = _hit.collider.GetComponentInParent<FacilityBase>();
				target.TriggerDispatch();
			}
		}
	}
}

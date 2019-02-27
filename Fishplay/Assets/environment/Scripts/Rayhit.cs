using UnityEngine;

public class Rayhit {
	public const string LAYER_CAN_RAY_HIT = "object";
	private RaycastHit _hit;
	private RaycastHit _weaponHit;
	private Camera _cam;
	private Ray _ray;
	private Vector3 _result;

	private int _layerMask;

	public Rayhit(Camera cam, LayerMask mask)
	{
		_cam = cam;
		_layerMask = mask;
	}

	public Vector3 GetTargetPoint(Transform weapon, Vector3 mouseScreenPos, float weaponMaxRange)
	{
		int mask = _layerMask;
		_ray = _cam.ScreenPointToRay(mouseScreenPos);
		Vector3 cam2weapon = weapon.position - _cam.transform.position;
		float max_distance = cam2weapon.sqrMagnitude + weaponMaxRange;
		bool cam_hit = Physics.Raycast(_ray,out _hit, max_distance, mask);
		bool in_range = false;
		Vector3 cam_pointing = _ray.origin + _ray.direction * max_distance;
		if(cam_hit)
		{
			Vector3 phit = _hit.point;
			in_range = Vector3.Distance(phit, weapon.position) < weaponMaxRange;
			cam_pointing = phit;
		}

		bool weapon_hit = Physics.Raycast(weapon.position,(cam_pointing - weapon.position).normalized, out _weaponHit, weaponMaxRange, mask);
		if(weapon_hit)
			_result = _weaponHit.point;
		else
			_result = cam_pointing;

		return _result;
	}
}
using UnityEngine;

public class CargoLoot : MonoBehaviour {
	private CargoData _data;

	private Transform _trackingTarget;
	private bool _collected = false;

	private const float DeltaSpeeder = 3.0f; 
	private const float LootInRange = 0.5f; 

	public System.Action<CargoData> OnCollectedAction;

	private void Update() {
		if(_trackingTarget == null)
			return;

		if(_collected)
			return;

		transform.position = Vector3.Lerp(_trackingTarget.position, transform.position, Time.deltaTime * DeltaSpeeder);

		if(Vector3.Distance(_trackingTarget.position, transform.position) < LootInRange)
		{
			_collected = true;
			OnCollectedAction(_data);
		}
	}

	private void SelfDestroy()
	{
		// play fx
		// self destory or pool in
		// FIXME: pool in
		Destroy(gameObject);
	}
}

[System.Serializable]
public class CargoData
{
	public CargoType Category;
	public uint Count;
	public int ID; 
	public bool Checked = false;
}
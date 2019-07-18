using UnityEngine;

public class CargoLoot : MonoBehaviour {
	private CargoData _data;

	private Transform _trackingTarget;
	private bool _collected = false;
	private bool _tracted = false;

	private const float DeltaSpeeder = 4.0f; 
	private const float LootInRange = 0.5f; 

	public System.Action<CargoData> OnCollectedAction;

	private Vector3 _currVel = Vector3.zero;
	private Vector3 _originPos;

	private float _tractedTimer = 0.0f;

	private Vector3 _constantEulerDelta;
	[Header("Self Rotate Delta(in euler"), SerializeField]
	private float _deltaRot = 0.05f;

	public static CargoData CreateDummy()
	{
		CargoData d = new CargoData();
		d.Category = (CargoType)Random.Range(1, 5);
		d.Checked = false;
		d.Count = (uint)Random.Range(1, 15);
		d.ID = Random.Range(10000, 100000);
		return d;
	}

	private void Start() {
		_constantEulerDelta = new Vector3(Random.Range(0.01f, _deltaRot) * Random.Range(-1, 1)
								, Random.Range(0.01f, _deltaRot) * Random.Range(-1, 1)
								, Random.Range(0.01f, _deltaRot) * Random.Range(-1, 1));

		_data = CreateDummy();
		LevelManager.Instance.AddPawn(this);
		_originPos = transform.position;
	}

	private void FixedUpdate()
	{
		if(_collected)
			return;

		transform.rotation *= Quaternion.Euler(_constantEulerDelta);
		if(_tracted)
		{
			_tractedTimer += Time.deltaTime * DeltaSpeeder;
			transform.position = Vector3.Lerp( _originPos, _trackingTarget.position, _tractedTimer);
			// transform.position = Vector3.SmoothDamp(_originPos, _trackingTarget.position, ref _currVel, 0.05f);

			if(Vector3.Distance(_trackingTarget.position, transform.position) < LootInRange)
			{
				_collected = true;
				OnCollectedAction(_data);
				SelfDestroy();
			}
		}
	}

	public void SetTraction(Transform t, System.Action<CargoData> handler)
	{
		if(_tracted == true)
			return;
		_trackingTarget = t;
		_tracted = true;

		OnCollectedAction = handler;
	}

	private void SelfDestroy()
	{
		LevelManager.Instance.RemovePawn(this);
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